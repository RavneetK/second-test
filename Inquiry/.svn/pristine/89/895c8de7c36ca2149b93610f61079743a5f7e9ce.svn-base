using System;
using System.ComponentModel.DataAnnotations;

namespace DcmsMobile.Inquiry.Areas.Inquiry.CartonEntity
{
    public class CartonHeadlineModel
    {
        public CartonHeadlineModel()
        {

        }

        internal CartonHeadlineModel(CartonHeadline p)
        {
                CartonId = p.CartonId;
                LocationId = p.LocationId;
                AreaShortName = p.AreaShortName;
                Building = p.BuildingId;
                AreaId = p.AreaId;
                Style = p.Style;
                Color = p.Color;
                Dimension = p.Dimension;
                SkuSize = p.SkuSize;
                VwhId = p.VwhId;
                QualityCode = p.QualityCode;
                Pieces = p.Pieces;
                LastPulledDate = p.LastPulledDate;
                ReqProcessId = p.ReqProcessId;
                AssignedRestockWhId = p.BestRestockBuildingId;
                AssignedRestockArea = p.BestRestockAreaId;
                AssignedRestockLocationId = p.BestRestockLocationId;
                AssignedRestockAreaShortName = p.BestRestockAreaShortName;
                AssignedRestockAisle = p.BestRestockAisleId;
                SkuId = p.SkuId;
        }

        [Display(Name = "Carton",Order=1)]
        [DisplayFormat(NullDisplayText = "None")]
        public string CartonId { get; set; }

        [Display(Name = "Location",Order=3)]
        [DisplayFormat(NullDisplayText = "None")]        
        public string LocationId { get; set; }

        [Display(Name = "Area", Order = 2)]
        [DisplayFormat(NullDisplayText = "None")]        
        public string AreaShortName { get; set; }
                
        [ScaffoldColumn(false)]
        public string AreaId { get; set; }


        #region To Remove use of SkuInventoryItem entity

        public int? SkuId { get; set; }

        [Display(Name = "Style",Order=5)]
        public string Style { get; set; }

        [Display(Name = "Color",Order=6)]
        public string Color { get; set; }

        [Display(Name = "Dim", ShortName = "Dimension",Order=7)]
        public string Dimension { get; set; }

        [Display(Name = "Size",Order=8)]
        public string SkuSize { get; set; }

        [ScaffoldColumn(false)]
        public string DisplaySkuVwh
        {
            get
            {
                return string.Format("{4} - {0}, {1}, {2}, {3}", this.Style, this.Color, this.Dimension, this.SkuSize, this.VwhId);
            }
        }

        [Display(Name = "Pieces",Order=11)]
        [Range(1, int.MaxValue, ErrorMessage = "SKU Pieces are negative or 0")]
        [DisplayFormat(DataFormatString = "{0:N0}",NullDisplayText="Empty Carton")]
        public int? Pieces { get; set; }

        [Display(Name = "Quality",Order=10)]
        [DisplayFormat(NullDisplayText = "Not defined")]
        public string QualityCode { get; set; }

        [Display(Name = "VWH",Order=9)]
        [DisplayFormat(NullDisplayText="None")]
        public string VwhId { get; set; }
        #endregion


        [Display(Name = "Request",Order=12)]
        public int? ReqProcessId { get; set; }

        [Display(Name = "Last Pulled Date",Order=13)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? LastPulledDate { get; set; }

        [Display(Name = "Building")]
        [DisplayFormat(NullDisplayText = "Unknown")]
        [ScaffoldColumn(false)]
        public string Building { get; set; }

        //this property ios added to show Assigned restock aisle for carton's SKU on carton pallet scan
        // public CartonLocation AssignedRestockAisle { get; set; }
        #region to Remove use of Carton Location Entity

        [Display(Name = "Location", ShortName = "Restock Location",Order=17)]
        public string AssignedRestockLocationId { get; set; }

        [Display(Name = "Area",ShortName="Restock Area",Order=16)]
        public string AssignedRestockAreaShortName { get; set; }

        [ScaffoldColumn(false)]
        public string AssignedRestockArea { get; set; }

        [Display(Name = "Building", ShortName = "Restock Building",Order=15)]
        public string AssignedRestockWhId { get; set; }

        [Display(ShortName = "Restock Aisle", Order = 18)]
        public string AssignedRestockAisle { get; set; }

        #endregion

        /// <summary>
        /// Whether the quality of the Carton represents shippable quality. TODO: Get from database
        /// </summary>
        public bool IsShippableQuality { get; set; }
    }
}