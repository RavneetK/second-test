using DcmsMobile.Inquiry.Areas.Inquiry.SharedViews;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DcmsMobile.Inquiry.Areas.Inquiry.CartonEntity
{
    public class CartonPalletViewModel : ICartonListViewModel
    {
        [Display(Name = "Pallet")]
        [Required(ErrorMessage = "Pallet should have Pallet ID")]
        [RegularExpression(@"P\S{1,}", ErrorMessage = "Pallet ID must begin with a P")]
        [DataType("Alert")]
        public string PalletId { get; set; }

        public string UrlCartonToPallet { get; set; }
        public string UrlBulkUpdateCarton { get; set; }
        public string UrlCartonLocating { get; set; }

        [Display(Name = "No Of Cartons")]
        [Required(ErrorMessage = "Total count is missing")]
        [DataType("Alert")]
        [DisplayFormat(DataFormatString="{0:N0}",NullDisplayText = "Null")]        
        public int TotalCartons { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]        
        public int TotalPieces { get; set; }
                   
        public IList<CartonHeadlineModel> AllCartons { get; set; }

        public string BuildingId
        {
            get
            {
                if (AllCartons == null || AllCartons.Count == 0)
                {
                    return string.Empty;
                }
                return AllCartons[0].Building;
            }
        }

        /// <summary>
        /// List of distict areas of cartons on pallet
        /// </summary>
        public string AreaList
        {
            get
            {
                return string.Join(", ", AllCartons.Select(p => p.AreaId).Distinct());
            }
        }

        public int AreaCount
        {
            get
            {
                return AllCartons.Select(p => p.AreaId).Distinct().Count();
            }
        }

    }


    
}



//$Id: CartonPalletViewModel.cs 26203 2014-09-02 12:16:59Z ssinghal $