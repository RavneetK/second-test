using DcmsMobile.Inquiry.Areas.Inquiry.SharedViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DcmsMobile.Inquiry.Areas.Inquiry.IntransitEntity
{
    public class IntransitShipmentViewModel
    {

        [Display(Name = "Shipment ID")]
        [DataType("Alert")]
        [DisplayFormat(NullDisplayText = "None")]
        public string ShipmentId { get; set; }

        [Display(Name = "")]
        [DataType("Alert")]
        [DisplayFormat(NullDisplayText = "None")]
        public string SewingPlantCode { get; set; }

        [Display(Name = "Sewing Plant")]
        [DataType("Alert")]
        [DisplayFormat(NullDisplayText = "New Sewing Plant")]
        public string SewingPlantName { get; set; }

        [Display(Name = "Shipment Date")]
        [DataType(DataType.Date)]
        public DateTime? ShipmentDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MinReceiveDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MaxReceiveDate { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "VWh")]
        [DisplayFormat(NullDisplayText = "None")]
        public string MinVwh { get; set; }

        [DisplayFormat(NullDisplayText = "None")]
        public string MaxVwh { get; set; }

        [Display(Name = "Total Vwh")]
        [DisplayFormat(NullDisplayText = "None")]
        public int VwhCount { get; set; }

        [Display(Name = "Intransit")]
        [DisplayFormat(NullDisplayText = "None")]
        public int? IntransitId { get; set; }

        public IList<ShipmentSkuModel> ShipmentSku { get; set; }

        [Display(Name = "Shipment Status")]
        public string IsShipmentClosed { get; set; }

        [Display(Name = "Intransit Type")]
        [DisplayFormat(NullDisplayText = "None")]
        public string IntransitType { get; set; }

        [Display(Name = "ERP")]
        [DisplayFormat(NullDisplayText = "None")]
        public string ErpId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalExpectedCartonCount
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.ExpectedCartonCount ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalReceivedCartonCount
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.ReceivedCartonCount ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalUnderReceviedCartonCount
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.UnderReceviedCartonCount ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalOverReceviedCartonCount
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.OverReceviedCartonCount ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalExpectedPieces
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.ExpectedPieces ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalReceivedPieces
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.ReceivedPieces ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalUnderReceviedPieces
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.UnderReceviedPieces ?? 0);
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalOverReceviedPieces
        {
            get
            {
                return this.ShipmentSku.Sum(p => p.OverReceviedPieces ?? 0);
            }
        }

        ////public string RecevingLink { get; set; }
        //private readonly IList<DcmsLinkModel> _dcmsLinks = new List<DcmsLinkModel>();
        //public IList<DcmsLinkModel> DcmsLinks
        //{
        //    get
        //    {
        //        return _dcmsLinks;
        //    }
        //}

        public string UrlReceiveCarton { get; set; }
    }
}