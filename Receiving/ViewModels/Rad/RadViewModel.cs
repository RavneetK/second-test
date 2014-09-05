using System.Collections.Generic;

namespace DcmsMobile.Receiving.ViewModels.Rad
{
    public class RadViewModel : ViewModelBase
    {
        public bool EnableEditing { get; set; }

        public IList<SpotCheckViewModel> SpotCheckList { get; set; }

        public SpotCheckViewModel SpotCheckViewModel { get; set; }

        public IList<SpotCheckViewModel> SpotCheckAreaList { get; set; }
    }
}



//$Id: RadViewModel.cs 21151 2013-06-11 09:41:25Z rverma $