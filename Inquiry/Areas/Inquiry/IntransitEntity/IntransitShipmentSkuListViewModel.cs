using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace DcmsMobile.Inquiry.Areas.Inquiry.IntransitEntity
{


    public class ShipmentSkuGroup
    {

        /// <summary>
        /// Key is InstransitType
        /// </summary>
        public string InstransitType { get; set; }

        /// <summary>
        /// Display name of InstransitType
        /// </summary>
        public string DisplayInstransitType
        {
            get
            {
                return (string.IsNullOrEmpty(InstransitType) || this.InstransitType == "IT") ? "Vendor Shipment" : (InstransitType == "ZEL" || this.InstransitType == "TR") ? " Building Transfer" : "Unknown";
            }
        }

        /// <summary>
        /// List of shipments per sku
        /// </summary>
        public IList<ShipmentDetailSkuModel> Shipments { get; set; }

    }

    public class IntransitShipmentSkuListViewModel
    {
        /// <summary>
        /// Shipment list group by InstransitType.
        /// </summary>
        public IList<ShipmentSkuGroup> ShipmentLists { get; set; }

        #region Filter
        public bool VarianceShipmentOnly { get; set; }

        public bool BuildingTransfers { get; set; }

        public bool VendorShipments { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? MinClosedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? MaxClosedDate { get; set; }

        public string FilterSewingPlant { get; set; }

        public IList<SelectListItem> SewingPlantList { get; set; }

        #endregion

        public int? TotalShipmentRows { get; set; }

        // [AdditionalMetadata(ExcelAttribute.BUTTON_NAME, ExcelAttribute.BUTTON_NAME)]
        [Display(ShortName = "Vendors", Name = "Vendor Shipments", Order = 20)]
        public IList<ShipmentDetailSkuModel> VendorShipmentList
        {
            get
            {
                var vendorGroup = this.ShipmentLists.FirstOrDefault(p => string.IsNullOrWhiteSpace(p.InstransitType));
                if (vendorGroup == null)
                {
                    // There are no transfer shipments. Return null so that Excel will ignore it
                    return new List<ShipmentDetailSkuModel>();
                }
                return vendorGroup.Shipments;
            }
        }

        // [AdditionalMetadata(ExcelAttribute.BUTTON_NAME, ExcelAttribute.BUTTON_NAME)]
        [Display(ShortName = "Transfers", Name = "Building Transfer", Order = 10)]
        public IList<ShipmentDetailSkuModel> TransferShipmentList
        {
            get
            {
                var transferGroup = this.ShipmentLists.FirstOrDefault(p => p.InstransitType == "ZEL");
                if (transferGroup == null)
                {
                    // There are no transfer shipments. Return null so that Excel will ignore it
                    return new List<ShipmentDetailSkuModel>(); ;
                }
                return transferGroup.Shipments;
            }
        }


        //[AdditionalMetadata(ExcelAttribute.BUTTON_NAME, ExcelAttribute.BUTTON_NAME)]
        [Display(ShortName = "Unknown", Name = "Unknown", Order = 10)]
        public IList<ShipmentDetailSkuModel> UnknownShipmentList
        {
            get
            {
                var transferGroup = this.ShipmentLists.FirstOrDefault(p => p.InstransitType == "Unknown");
                if (transferGroup == null)
                {
                    // There are no transfer shipments. Return null so that Excel will ignore it
                    return new List<ShipmentDetailSkuModel>();
                }
                return transferGroup.Shipments;
            }
        }


        public string ExcelFileName
        {
            get { return "InboundShipmentSkuSummary"; }
        }

        public int? MaxRowsToShow { get; set; }

        public string DisplayRowsToShow
        {
            get
            {
                if (MaxRowsToShow < TotalShipmentRows)
                {
                    return string.Format("Showing {0} of {1} shipments", this.MaxRowsToShow, this.TotalShipmentRows);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }


    public class ShipmentDetailSkuModel
    {
        [Display(ShortName = "Shipment", Order = 10)]
        public string ShipmentId { get; set; }

        [ScaffoldColumn(false)]
        public string MinOtherShipmentId { get; set; }

        [ScaffoldColumn(false)]
        public string MaxOtherShipmentId { get; set; }

        [ScaffoldColumn(false)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? CountOtherShipments { get; set; }

        [ScaffoldColumn(false)]
        public string MinBuddyShipmentId { get; set; }

        [ScaffoldColumn(false)]
        public string MaxBuddyShipmentId { get; set; }

        [ScaffoldColumn(false)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? CountBuddyShipments { get; set; }

        [Display(Name = "Shipment Date", Order = 13)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ShipmentDate { get; set; }

        [Display(Name = "Sewing Plant Code")]
        [ScaffoldColumn(false)]
        public string SewingPlantCode { get; set; }

        [Display(Name = "Sewing Plant Name")]
        [ScaffoldColumn(false)]
        public string SewingPlantName { get; set; }

        [Display(ShortName = "Sewing Plant", Order = 130)]
        public string SewingPlant
        {
            get
            {
                return string.Format("{0} {1}", this.SewingPlantCode, this.SewingPlantName);
            }
        }

        [Display(ShortName = "Sent to ERP", Order = 20)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? UploadDate { get; set; }

        [Display(ShortName = "Style", Order = 30)]
        public string Style { get; set; }

        [Display(ShortName = "Color", Order = 40)]
        public string Color { get; set; }

        [Display(ShortName = "Dimension", Order = 50)]
        public string Dimension { get; set; }

        [Display(ShortName = "SKU Size", Order = 60)]
        public string SkuSize { get; set; }

        [ScaffoldColumn(false)]
        public string VwhId { get; set; }


        [Display(ShortName = "Pcs Expected", Order = 70)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ExpectedPieces { get; set; }


       


        [Display(ShortName = "Ctns Expected", Order = 100)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ExpectedCartonCount { get; set; }

        [ScaffoldColumn(false)]
        public int? ReceivedPiecesMine { get; set; }

        [Display(ShortName = "Pcs Received", Order = 80)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ReceivedPieces
        {
            get
            {
                return (this.ReceivedPiecesMine ?? 0) + (this.ReceivedPiecesOfBuddies ?? 0);

            }
        }

        [ScaffoldColumn(false)]
        public int? ReceivedCartonsMine { get; set; }

        /// <summary>
        /// This is total received carton. Includes the received cartons of other shipment.
        /// </summary>
        [Display(ShortName = "Ctns Received", Order = 110)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ReceivedCartons
        {
            get
            {
                return (this.ReceivedCartonsMine ?? 0) + (this.ReceivedCtnOfBuddies ?? 0);

            }
        }

        [ScaffoldColumn(false)]
        public string IntransitType { get; set; }

        [ScaffoldColumn(false)]
        public int? ReceivedCtnByBuddies { get; set; }


        [Display(ShortName = "Ctns Overage", Order = 100)]
        [ScaffoldColumn(false)]
        public int? ReceivedCtnOfBuddies { get; set; }

        [ScaffoldColumn(false)]
        //[DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ReceivedPiecesByBuddies { get; set; }

        [Display(ShortName = "Pcs Overage", Order = 140)]
        [ScaffoldColumn(false)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ReceivedPiecesOfBuddies { get; set; }


        /// <summary>
        /// Cartons not received in my shipment.
        /// </summary>
        [Display(ShortName = "Ctns Not Received", Order = 90)]
        [ScaffoldColumn(false)]
        public int? CartonsNotReceived
        {
            get
            {
                return (this.ExpectedCartonCount ?? 0) - (this.ReceivedCartonsMine ?? 0);

            }
        }

        [Display(ShortName = "Pcs Not Received", Order = 130)]
        [ScaffoldColumn(false)]
        public int? PiecesNotReceived
        {
            get
            {
                return (this.ExpectedPieces ?? 0) - (this.ReceivedPiecesMine ?? 0);
            }
        }

        [Display(ShortName = "Carton Variance", Order = 120)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? TotalCartonVariance
        {
            get
            {
                return (this.ReceivedCartons ?? 0) - (this.ExpectedCartonCount ?? 0);
            }
        }

        [Display(ShortName = "Pieces Variance", Order = 90)]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? TotalPiecesVariance
        {
            get
            {
                return (this.ReceivedPieces ?? 0) - (this.ExpectedPieces ?? 0);
            }
        }

        [Display(ShortName = "Variance Commentary", Order = 140)]
        [DataType(DataType.MultilineText)]
        public string VarianceCommentsExcel
        {
            get
            {
                string x = string.Empty;
                if (this.CountBuddyShipments > 0)
                {

                    switch (this.CountBuddyShipments)
                    {
                        case 1:
                            x = string.Format("Received pieces include {0:N0} pieces of Shipment {1}.", this.ReceivedPiecesOfBuddies, this.MaxBuddyShipmentId);
                            break;
                        case 2:
                            x = string.Format("Received pieces include {0:N0} pieces of Shipments {1} and {2}.",
                                this.ReceivedPiecesOfBuddies, this.MaxBuddyShipmentId, this.MinBuddyShipmentId);
                            break;
                        default:
                            x = string.Format("Received pieces include {0:N0} pieces of Shipments {1}, {2} and {3} others.",
                                this.ReceivedPiecesOfBuddies, this.MaxBuddyShipmentId, this.MinBuddyShipmentId, this.CountBuddyShipments - 2);
                            break;
                    }
                }

                if (this.CountOtherShipments > 0)
                {

                    switch (this.CountOtherShipments)
                    {
                        case 1:
                            x = x + string.Format("{0:N0} pieces were received after closing against Shipment {1}.", this.ReceivedPiecesByBuddies, this.MaxOtherShipmentId);
                            break;
                        case 2:
                            x = x + string.Format("{0:N0} pieces were received after closing against Shipments {1} and {2}.",
                                this.ReceivedPiecesByBuddies, this.MaxOtherShipmentId, this.MinOtherShipmentId);
                            break;

                        default:
                            x = x + string.Format("{0:N0} pieces were received after closing against Shipments {1}, {2} and {3} others.",
                                this.ReceivedPiecesByBuddies, this.MaxOtherShipmentId, this.MinOtherShipmentId, this.CountOtherShipments - 2);
                            break;
                    }
                }

                return x;

            }
        }



    }
}