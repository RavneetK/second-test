using System;

namespace DcmsMobile.Inquiry.Areas.Inquiry.PickslipEntity
{
    internal class Wave
    {
        public int BucketId { get; set; }

        public string CustomerName { get; set; }

        public bool ExportFlag { get; set; }

        public string Name { get; set; }

        public string PullToDock { get; set; }

        public string PickMode { get; set; }

        public string Comment { get; set; }

        public string PitchArea { get; set; }

        public int PitchLimit { get; set; }

        public string PitchType { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public bool Freeze { get; set; }

        public bool AvailableForPitching { get; set; }

        public string Status { get; set; }

        public string Building { get; set; }

        public int? TotalBoxes { get; set; }

        public int? RedBoxCount { get; set; }

        public int? NonPhysicalBoxCount { get; set; }

        public int? PullableBoxCount { get; set; }

        public int? AboutToPullBoxCount { get; set; }

        public int? UnprocessedPieces { get; set; }

        public int? PickslipCount { get; set; }

        public int? PitchableBoxes { get; set; }

        public int? PitchedBoxes { get; set; }

        public int? CheckedBoxes { get; set; }

    }
}





//$Id: Wave.cs 25729 2014-07-24 09:42:15Z asharma $