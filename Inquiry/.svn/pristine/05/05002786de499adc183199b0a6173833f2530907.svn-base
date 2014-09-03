using DcmsMobile.Inquiry.Areas.Inquiry.SharedViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DcmsMobile.Inquiry.Areas.Inquiry.PickslipEntity
{
   
    public class WaveViewModel
    {
        [Display(Name = "Wave ID")]
        [Required(ErrorMessage = "Wave ID is required")]
        [DataType("Alert")]
        public int BucketId { get; set; }

        [Display(Name = "Customer Name")]
        [Required(ErrorMessage = "Customer Name is required")]
        [DataType("Alert")]
        public string CustomerName { get; set; }

        [Display(Name = "Export Order")]
        public bool ExportFlag { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Wave Name is required")]
        [DataType("Alert")]
        public string Name { get; set; }

        [Display(Name = "Pull To Dock")]
        [DataType("Alert")]
        public string PullToDock { get; set; }

        [Display(Name = "Pick Mode")]
        [DataType("Alert")]
        public string PickMode { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Pick Area")]
        public string PitchArea { get; set; }

        [Display(Name = "Pitch Limit")]
        [Required(ErrorMessage = "Pitch Limit is required")]
        [DataType("Alert")]
        public int PitchLimit { get; set; }

        [Display(Name = "Pitch Type")]
        [Required(ErrorMessage = "Pitch Type is required")]
        [DataType("Alert")]
        public string PitchType { get; set; }

        [Display(Name = "Creation Time")]
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "Date Created is required")]
        [DataType("Alert")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Created By")]
        [Required(ErrorMessage = "User is required")]
        [DataType("Alert")]
        public string CreatedBy { get; set; }

        [Display(Name = "Freeze")]
        public bool Freeze { get; set; }

        [Display(Name = "Available for Pitching")]
        public bool AvailableForPitching { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Building")]
        public string Building { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Total Boxes")]
        public int? TotalBoxes { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Red Boxes")]
        public int? RedBoxCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Non Physical Boxes")]
        public int? NonPhysicalBoxCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Pullable Boxes")]
        public int? PullableBoxCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# About To Pull Boxes")]
        public int? AboutToPullBoxCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Unprocessed Pieces")]
        public int? UnprocessedPieces { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Pickslips")]
        public int? PickslipCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Pitchable Boxes")]
        public int? PitchableBoxes { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Pitched Boxes")]
        public int? PitchedBoxes { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "# Checked Boxes")]
        public int? CheckedBoxes { get; set; }


        [Display(Name = "Pull To Dock")]
        public string DockPullStatus
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PullToDock))
                {
                    return "Yes";
                }
                return "No";
            }
        }

        [Display(Name = "Pick Mode")]
        public string Pick
        {
            get
            {
                if (this.PickMode == "ADRE")
                {
                    return "Exclusive Pull To Dock Post Printing";
                }
                else if (this.PickMode == "ADREPPWSS")
                {
                    return "Exclusive Pull To Dock Pre Printing";
                }
                return this.PickMode;
            }
        }


        public int? PercentPicked
        {
            get
            {
                return this.TotalBoxes != null
                           ? ((this.PitchedBoxes + this.CheckedBoxes) * 100) / this.TotalBoxes
                           : null;
            }
        }

        /// <summary>
        /// Url of Report 140.02: This report display details of buckets which are opened for processing.
        /// </summary>
        public string OpenBucketSummaryUrl
        {
            get
            {

                return System.Configuration.ConfigurationManager.AppSettings["DcmsLiveBaseUrl"] + "Reports/Category_140/R140_02.aspx";
            }
        }


        /// <summary>
        /// Url of Report 140.102: For the bucket this report lists all the SKUs of the unprocessed cartons.
        /// </summary>
        public string UnprocessedSkuReportUrl
        {
            get
            {

                return System.Configuration.ConfigurationManager.AppSettings["DcmsLiveBaseUrl"] + "Reports/Category_140/R140_102.aspx";
            }
        }

        /// <summary>
        /// Url of Report 140.05: It displays all SKUs of the bucket for which pieces are short in forward pick area.
        /// </summary>
        public string ForwardPickShortageReportUrl
        {
            get
            {

                return System.Configuration.ConfigurationManager.AppSettings["DcmsLiveBaseUrl"] + "Reports/Category_140/R140_105.aspx";
            }
        }

        /// <summary>
        /// Url of Report 140.05: In ProcessReport
        /// </summary>
        public string InProcessReport
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["DcmsLiveBaseUrl"] + "Reports/Category_110/R110_07.aspx";
            }
        }

        public string UrlPullBoxes { get; set; }

        public string UrlManagePickwave { get; set; }

        
    }
}