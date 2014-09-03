using System;
using System.ComponentModel.DataAnnotations;

namespace DcmsMobile.Inquiry.Areas.Inquiry.CartonEntity
{

    /// <summary>
    /// Information about a carton which is currently on a pallet
    /// </summary>
    internal class CartonHeadline:CartonBase
    {
        /// <summary>
        /// Where is the carton currently located
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// The area of the carton
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// Short name of the carton area
        /// </summary>
        public string AreaShortName { get; set; }

        /// <summary>
        /// Building in which the carton exists
        /// </summary>
        public string BuildingId { get; set; }



        public DateTime? LastPulledDate { get; set; }

        public int? ReqProcessId { get; set; }

        public string BestRestockBuildingId { get; set; }

        public string BestRestockAreaId { get; set; }

        public string BestRestockLocationId { get; set; }

        public string BestRestockAreaShortName { get; set; }

        public string BestRestockAisleId { get; set; }
    }
}



//$Id: CartonHeadline.cs 25805 2014-07-29 10:39:07Z ssinghal $