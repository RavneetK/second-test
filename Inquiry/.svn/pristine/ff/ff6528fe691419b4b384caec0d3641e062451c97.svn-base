using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DcmsMobile.Inquiry.Areas.Inquiry.IntransitEntity
{
    /// <summary>
    /// Where is this shipment coming from ?
    /// </summary>
    public enum ShipmentSourceType
    {
        Vendor,

        Transfer
    }

    public enum ShipmentStatusType
    {
        Open,
        Closed,
        Dates
    }

    public class ShipmentListFilterModel
    {
        public ShipmentSourceType? Source { get; set; }

        public ShipmentStatusType Status { get; set; }

        public string SewingPlantCode { get; set; }

        public bool VariancesOnly { get; set; }

        /// <summary>
        /// This RegExp stolen from http://regexlib.com/REDetails.aspx?regexp_id=112
        /// </summary>
        /// <remarks>
        /// The following validates dates with and without leading zeros in the following formats: MM/DD/YYYY and it also takes YYYY (this can easily be removed).
        /// All months are validated for the correct number of days for that particular month except for February which can be set to 29 days. date day month year
        /// </remarks>
        //[RegularExpression(@"^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$")]
        public DateTime? MinClosedDate { get; set; }

        //[RegularExpression(@"^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$")]
        public DateTime? MaxClosedDate { get; set; }

        public string DisplayStatus
        {
            get
            {
                switch (Status)
                {
                    case ShipmentStatusType.Open:
                        return "Open";
                    case ShipmentStatusType.Closed:
                            return "Closed";   
                    case ShipmentStatusType.Dates:
                            if (MinClosedDate != null && MaxClosedDate != null)
                            {
                                return string.Format("From {0:d} To {1:d}",MinClosedDate,MaxClosedDate);
                            }
                            else if (MinClosedDate != null)
                            {
                                return string.Format("From {0:d}", MinClosedDate);
                            }
                            else
                            {
                                return string.Format("To {0:d}", MaxClosedDate);
                            }
                    default :
                            throw new NotImplementedException();                    
                }
            }
        }

        /// <summary>
        /// Returns true if any don default filter has been applied
        /// </summary>
        public bool HasFilters
        {
            get
            {
                return Source != null || Status != ShipmentStatusType.Open || !string.IsNullOrWhiteSpace(SewingPlantCode) ||
                    VariancesOnly || MinClosedDate != null || MaxClosedDate != null;
            }
        }

        public string DisplayFilters
        {
            get
            {
                //return "TODO: One line summary of applied filters";
                List<string> list = new List<string>();
                if (this.Status != ShipmentStatusType.Open)
                {
                    list.Add(this.Status.ToString());
                }
                if (this.Source.HasValue)
                {
                    list.Add(this.Source.ToString());
                }
                if (this.MinClosedDate != null || this.MaxClosedDate != null)
                {
                    list.Add("Dates");
                }
                if (!string.IsNullOrWhiteSpace(this.SewingPlantCode))
                {
                    list.Add(SewingPlantCode);
                }
                if (this.VariancesOnly)
                {
                    list.Add("Variances");
                }
                if (list.Count == 0)
                {
                    return "Filters";
                }
                return string.Join(";", list);
            }
        }

    }

    public class IntransitShipmentListViewModel
    {
        private ShipmentListFilterModel _filters;
        /// <summary>
        /// These filters will be posted
        /// </summary>
        public ShipmentListFilterModel Filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = new ShipmentListFilterModel();
                }
                return _filters;
            }
            set
            {
                _filters = value;
            }
        }

        public IList<IntransitShipmentModel> Shipments { get; set; }

        public IList<SelectListItem> SewingPlantList { get; set; }
    }

}