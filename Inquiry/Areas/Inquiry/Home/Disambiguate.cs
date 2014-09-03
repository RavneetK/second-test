
using System.Collections.Generic;

namespace DcmsMobile.Inquiry.Areas.Inquiry.Home
{
    public class ChoiceItem
    {
        public string Description { get; set; }

        public string Url { get; set; }

    }

    public class DisambiguateViewModel
    {
        public IList<ChoiceItem> Choices { get; set; }

        public string Scan { get; set; }
    }
}





//$Id: Disambiguate.cs 25710 2014-07-24 06:15:29Z apanwar $