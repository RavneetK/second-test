﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration.Provider;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Routing;
using DcmsMobile.Receiving.Models;

namespace DcmsMobile.Receiving.Repository
{

    public enum ScanResult
    {
        PalletScan,
        CartonReceived
    }

    public class ScanContext
    {
        /// <summary>
        /// In/Out: The pallet which is currently active
        /// </summary>
        /// <remarks>
        /// The pallet which should now be made active is returned. Otherwise left untouched.
        /// </remarks>
        public string PalletId { get; set; }

        /// <summary>
        /// In: Disposition of the currently active pallet
        /// </summary>
        /// <remarks>
        /// The disposition of the carton is returned. Otherwise left untouched.
        /// </remarks>
        public string DispositionId { get; set; }

        /// <summary>
        /// In: Process Id
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Out: How the scan was handled
        /// </summary>
        public ScanResult Result { get; set; }
       
        /// <summary>
        /// Total number of expected cartons against the process
        /// </summary>
        public int? ExpectedCartons { get; set; }

        /// <summary>
        /// Total number of expected cartons against the process
        /// </summary>
        public int? CartonsOnPallet { get; set; }
    }
    public class AlreadyReceivedCartonException : Exception
    {
        private readonly string _palletId;

        public string PalletId { get { return _palletId; } }

        public AlreadyReceivedCartonException()
        {

        }

        public AlreadyReceivedCartonException(string palletId)
        {
            _palletId = palletId;
        }
    }

    public class DispositionMismatchException : Exception
    {
        private readonly string _vwhId;
        private readonly string _areaId;

        public DispositionMismatchException()
        {

        }

        public DispositionMismatchException(string vwhId, string areaId)
        {
            _vwhId = vwhId;
            _areaId = areaId;

        }

        public string VwhId
        {
            get
            {
                return _vwhId;
            }
        }

        public string AreaId
        {
            get
            {
                return _areaId;
            }
        }
    }

    public class ReceivingService : IDisposable
    {
        private readonly ReceivingRepository _repos;

        /// <summary>
        /// This is an always increasing cache which expires every 60 min.
        /// This cache also stores a list of SKUs for which e-mail has been sent.
        /// </summary>
        private const string APPKEY_PROCESSINFO = "ReceivingService_ProcessInfo";

        /// <summary>
        /// Stores the destination area for each intransit carton. The destination will either be the receiving area or the spot check area.
        /// By storing the destination, we ensure that the destination does not change if the carton is scanned again. The entry is removed after the carton is received.
        /// </summary>
        private const string APPKEY_INTRANSIT = "ReceivingService_IntransitDestinationAreas";

        /// <summary>
        /// Stores disposition of scanned pallet.
        /// </summary>
        private const string APPKEY_PALLETDISPOS = "ReceivingService_PalletDisposition";

        /// <summary>
        /// For unit tests. 
        /// </summary>
        public ReceivingService(ReceivingRepository repos)
        {
            _repos = repos;
        }

        public int GetCartonsOfProcess(int? processId){
          return  _repos.GetCartonsOfProcess(processId);
    }

        /// <summary>
        /// Used to store destination area of intransit cartons until they are received
        /// </summary>
        //private readonly HttpSessionStateBase _session;
        public ReceivingService(RequestContext ctx)
        {
            _repos = new ReceivingRepository(ctx);
        }

        public void Dispose()
        {
            var dis = _repos as IDisposable;
            if (dis != null)
            {
                dis.Dispose();
            }
        }

        const int PALLET_LIMIT = 50;    // Factory default

        /// <summary>
        /// We generate new pallet. 
        /// </summary>
        /// <returns></returns>
        public string CreateNewPalletId()
        {
            int palletSequence = _repos.GetPalletSequence();
            return string.Concat("P", palletSequence);

        }

        /// <summary>
        /// Cache Pallet Dispostion
        /// Key is pallet_id, value is disposition
        /// </summary>
        private ConcurrentDictionary<string, string> CachedPalletDisposition
        {
            get
            {
                var palletDispos = MemoryCache.Default[APPKEY_PALLETDISPOS] as ConcurrentDictionary<string, string>;
                if (palletDispos == null)
                {
                    palletDispos = new ConcurrentDictionary<string, string>();
                    MemoryCache.Default.Add(APPKEY_PALLETDISPOS, palletDispos, new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(30)
                    });
                }
                return palletDispos;
            }
        }

        private class ProcessModelCollection : KeyedCollection<int, ReceivingProcess>
        {
            protected override int GetKeyForItem(ReceivingProcess item)
            {
                return item.ProcessId;
            }
        }

        private ProcessModelCollection ProcessCache
        {
            get
            {
                var set = MemoryCache.Default[APPKEY_PROCESSINFO] as ProcessModelCollection;
                if (set == null)
                {
                    set = new ProcessModelCollection();
                    MemoryCache.Default.Add(APPKEY_PROCESSINFO, set, new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60)
                    });
                }
                return set;
            }
        }


        /// <summary>
        /// The session is only used for caching, so an expired session should not lead to any user visible problem.
        /// </summary>
        /// <param name="cartonId"></param>
        /// <param name="processId"></param>
        /// <returns>
        /// IntarnsitCarton
        /// </returns>
        /// <remarks>
        /// <para>
        /// The carton returned fron the repository is returned. Additionally, the destination of the carton is set based on the spot check percent or it's requirement in FPK.
        /// </para>
        /// </remarks>
        /// DB 2-04-2012: We first attempt to send the carton in spot check area. 
        /// Our second attempt is to send the carton in assigned area where this carton might be required.
        /// If a carton can not be send for spot check or to an assigned area then we send it to Receiving Area. 
        /// DB: 18-07-2012 Removed Intransit carton cache.
        private IntransitCarton GetIntransitCarton(string cartonId, int processId)
        {
            IntransitCarton ctn = _repos.GetIntransitCarton2(cartonId);
            if (ctn == null)
            {
                return ctn;
            }

            var rand = new Random();
            var process = GetProcessInfo(processId);
            if (ctn.IsSpotCheckEnabled && ctn.SpotCheckPercent.HasValue && rand.Next(100 * 100 - 1) < (Convert.ToInt32(ctn.SpotCheckPercent.Value * 100)))
            {
                // Needs spot check                 
                ctn.DestinationArea = process.SpotCheckAreaId;
            }
            else
            {
                var areaId = _repos.GetCartonDestination(cartonId);
                ctn.DestinationArea = string.IsNullOrEmpty(areaId) ? process.ReceivingAreaId : areaId;
            }

            return ctn;
        }

        public IEnumerable<Pallet> GetPalletsOfProcess(int processId)
        {
            var list = _repos.GetReceivedCartons2(null, processId, null);          
            var query = from carton in list
                        group carton by carton.PalletId into g
                        orderby g.Max(p => p.ReceivedDate) descending
                        select new Pallet
                        {
                            PalletId = g.Key,
                            PalletLimit = GetPalletLimit(processId), // ProcessId 
                            Cartons = g.ToList(),
                            ProcessId = processId
                        };
                        

            return query.Take(10);
        }

        /// <summary>
        /// Never returns null. If the pallet is empty, it will simply not have cartons.
        /// DB: 19-07-2012 Passing processId to GetReceivedCarton2 function. Earlier it was not being passed.
        /// </summary>
        /// <param name="palletId"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public Pallet GetPallet(string palletId, int? processId = null)
        {
            if (palletId == null) throw new ArgumentNullException("palletId");
            var pallet = new Pallet { PalletId = palletId };
            pallet.Cartons = _repos.GetReceivedCartons2(palletId, null, null);
            if (processId != null)
            {

                pallet.PalletLimit = pallet.Cartons.Count > 0
                                         ? GetPalletLimit(processId.Value)
                                         : PALLET_LIMIT;
            }

            return pallet;
        }



        public IEnumerable<ReceivingProcess> GetRecentProcesses()
        {
            return _repos.GetProcesses(null);
        }


        /// <summary>
        /// 
        /// We cache the process info since it is heavily used
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="clearCache">If true <paramref name="processId"/> will be removed from cache, and fresh info will be get through repos.</param>
        /// <returns></returns>
        public ReceivingProcess GetProcessInfo(int processId, bool clearCache = false)
        {
            //Removing the process info from cache because it has become stale.
            if (clearCache)
            {
                ProcessCache.Remove(processId);
            }


            if (ProcessCache.Contains(processId))
            {
                return ProcessCache[processId];
            }
            var process = _repos.GetProcesses(processId).FirstOrDefault();
            if (process != null)
            {
                ProcessCache.Add(process);
            }
            return process;
        }

        public int InsertProcess(ReceivingProcess processModel)
        {
            if (processModel.ProcessId != 0)
            {
                throw new ArgumentOutOfRangeException("processModel.ProcessId", "Process Id cannot be specified");
            }
            var processId = _repos.InsertProcess(processModel);

            processModel.ProcessId = processId;
            return processId;
        }

        /// <summary>
        /// Pass new values in processModel
        /// </summary>
        public void UpdateProcess(ReceivingProcess processModel)
        {


            if (processModel.ProcessId <= 0)
            {
                throw new ArgumentOutOfRangeException("processModel.ProcessId", "Process Id must be positive");
            }
            _repos.UpdateProcess(processModel);
            ProcessCache.Remove(processModel.ProcessId);

        }

        /// <summary>
        /// Returns palletId
        /// </summary>
        /// <param name="cartonId"></param>
        /// <param name="processId"></param>
        /// <param name="palletId"></param>
        /// <returns></returns>
        public Pallet RemoveFromPallet(string cartonId, int processId, out string palletId)
        {
            palletId = _repos.RemoveFromPallet(cartonId);

            var pallet = GetPallet(palletId, processId);
            if (pallet.Cartons != null && pallet.Cartons.Count == 0)
            {
                string dispos;
                CachedPalletDisposition.TryRemove(palletId, out dispos);

            }
            return pallet;
        }

        public void PrintCarton(string cartonId, string printer)
        {
            _repos.PrintCarton(cartonId, printer);
        }

        public IEnumerable<Printer> GetPrinters()
        {
            return _repos.GetPrinters();
        }


        private int GetPalletLimit(int processId)
        {

            int? limit = null;

            if (ProcessCache.Contains(processId))
            {
                limit = ProcessCache[processId].PalletLimit;
            }


            return limit.HasValue ? limit.Value : PALLET_LIMIT;

        }


        /// <summary>
        /// Returns outcome (Received, Pallet scan); List of cartons on pallet is returned; 
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="ctx"></param>
        /// <returns>Active pallet info. This will never be null.</returns>
        /// <exception cref="ProviderException">Generic message for any error condition</exception>
        /// <exception cref="DispositionMismatchException">The carton disposition does not match the disposition of active pallet</exception>
        /// <exception cref="AlreadyReceivedCartonException">The carton has already been received.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Pallet HandleScan(string scan, ScanContext ctx)
        {
            if (string.IsNullOrWhiteSpace(scan))
            {
                throw new ArgumentNullException("scan");
            }

            //var pallet = new Pallet();
            if (scan.StartsWith("P"))
            {
                return HandlePalletScan(scan, ctx);
            }
             
            if (ctx.PalletId == null)
            {
                throw new ProviderException(string.Format("Scan a Pallet first"));
            }
                  
            var cartonToReceive = this.GetIntransitCarton(scan, ctx.ProcessId);
            if (cartonToReceive == null)
            {
                throw new ProviderException(string.Format("Carton {0} not recognized", scan));
            }

            if (cartonToReceive.IsShipmentClosed)
            {
                // This carton is from an already closed shipment. check whether we can accept it.                 
                if (_repos.AcceptCloseShipmentCtn(scan, ctx.ProcessId))
                {
                    // Carton acceptable nothing to do. 

                }
                else {

                    throw new ProviderException(string.Format("Carton {0} belongs to a closed shipment.Scan after carton for open shipment or use blind receiving.", scan));
                }                                    
            }
            


            if (cartonToReceive.ReceivedDate != null)
            {
                // Carton already received.
                var carton = _repos.GetReceivedCartons2(null, null, cartonToReceive.CartonId).FirstOrDefault();
                // If the carton is recived and palletized we throw an error.
                if (carton == null)
                {
                    throw new ProviderException(string.Format("Carton {0} already received on {1} but does not exist in src_carton. Contact administrator", cartonToReceive.CartonId, cartonToReceive.ReceivedDate));
                }
                if (!string.IsNullOrEmpty(carton.PalletId))
                {
                    //carton has already been received throw exception with pallet Id on which carton was put.
                    throw new AlreadyReceivedCartonException(carton.PalletId);
                }
                // Put carton on pallet and check disposition. 
                if (HandleDisposition(ctx.PalletId, carton.DispositionId))
                {
                    // Dipos ok put carton on pallet
                    _repos.PutCartonOnPallet(ctx.PalletId, scan);
                }
                else
                {
                    // throw dispos mismatch exception.
                    throw new DispositionMismatchException(carton.VwhId, carton.DestinationArea);
                }
            }

            else
            {
                // Unreceived carton.
                _repos.ReceiveCarton(null, cartonToReceive.CartonId, cartonToReceive.DestinationArea, ctx.ProcessId);
                //Getting the pallet dispostion, if this pallet does not contain cartons then disposition will be null.
                if (HandleDisposition(ctx.PalletId, cartonToReceive.DispositionId))
                {
                    _repos.PutCartonOnPallet(ctx.PalletId, scan);

                }
                else
                {
                    throw new DispositionMismatchException(cartonToReceive.VwhId, cartonToReceive.DestinationArea);

                }

                ctx.Result = ScanResult.CartonReceived;
            }

            var process = GetProcessInfo(ctx.ProcessId);
            var pallet = this.GetPallet(ctx.PalletId, ctx.ProcessId);
            if (!pallet.Cartons.Any(p => p.CartonId == cartonToReceive.CartonId))
            {
                throw new ProviderException(string.Format("Internal Error: Received carton {0} not found on pallet {1}", cartonToReceive.CartonId, ctx.PalletId));
            }

            ctx.Result = ScanResult.CartonReceived;
            ctx.ExpectedCartons = process.ExpectedCartons;
            ctx.CartonsOnPallet = GetCartonsOfProcess(ctx.ProcessId);
            return pallet;
        }


        // PalletId, ProcessId, destArea, cartonId, 
        private bool HandleDisposition(string palletId, string cartonDispos)
        {

            if (string.IsNullOrEmpty(palletId))
            {
                throw new ArgumentNullException("palletId");

            }

            var palletDisposition = GetPalletDisposition(palletId);

            return (string.IsNullOrEmpty(palletDisposition) || palletDisposition == cartonDispos);
        }


        private Pallet HandlePalletScan(string scan, ScanContext ctx)
        {
            // Pallet was scanned. Ensure proper length of pallet
            // TODO: Handle this pallet length limit hardwiring in a more professional way
            if (scan.Length > 8)
            {
                var msg = string.Format("Pallet {0} exceeds 8 characters", scan);
                throw new ProviderException(msg);
            }
            ctx.Result = ScanResult.PalletScan;
            ctx.PalletId = scan;

            var palletInfo = this.GetPallet(scan, ctx.ProcessId);

            if (palletInfo.Cartons.Select(p => p.DispositionId).Distinct().Count() > 1)
            {
                throw new ProviderException(string.Format("Pallet contains cartons of multiple areas."));
            }

            //cache pallet Dispo
            if (palletInfo.Cartons.Select(p => p.DispositionId).Any())
            {
                CachedPalletDisposition.TryAdd(palletInfo.PalletId, palletInfo.Cartons.First().DispositionId);
            }
            return palletInfo;
        }

        /// <summary>
        /// Gives the disposition of passed pallet.
        /// </summary>
        /// <param name="palletId"></param>
        /// <returns></returns>
        private string GetPalletDisposition(string palletId)
        {
            string palletDispos;
            CachedPalletDisposition.TryGetValue(palletId, out palletDispos);

            if (string.IsNullOrEmpty(palletDispos))
            {
                var scannedPallet = GetPallet(palletId);
                if (scannedPallet.Cartons.Count > 0)
                {
                    palletDispos = scannedPallet.Cartons.First().DispositionId;
                    CachedPalletDisposition.TryAdd(palletId, palletDispos);
                }
            }
            return palletDispos;
        }


        public int QueryCount
        {
            get
            {
                return _repos.QueryCount;
            }
        }


        public IEnumerable<CartonArea> GetCartonAreas()
        {
            return _repos.GetCartonAreas();
        }


        /// <summary>
        /// Get the List of PriceSeasson Code.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CodeDescription> GetPriceSeasonCode()
        {
            return _repos.GetPriceSeasonCode();
        }


        public IEnumerable<ReceivedCarton> GetUnpalletizedCartons(int? processId)
        {
            return _repos.GetUnpalletizedCartons(processId);
        }

        public IList<ShipmentList> GetShipmentList()
        {
            return _repos.GetShipmentList();
        }

        /// <summary>
        /// Close passed Shipment
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <param name="poId"></param>
        public void CloseShipment(string shipmentId, long? poId)
        {
            _repos.CloseShipment(shipmentId, poId);
        }

        /// <summary>
        /// Reopen passed shipment
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <param name="poId"></param>
        public bool ReOpenShipment(string shipmentId, long? poId)
        {
            
            return _repos.ReOpenShipment(shipmentId, poId);
        }

        /// <summary>
        /// To validate carrier.
        /// </summary>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        public Carrier GetCarrier(string carrierId)
        {
            return _repos.GetCarrier(carrierId);
        }
       
    }
}


//$Id: ReceivingService.cs 25204 2014-07-04 09:20:39Z asharma $