using System.Collections.Generic;
using System;

namespace DcmsMobile.Receiving.Models
{
    public class Pallet
    {
        public string PalletId { get; set; }


        public int PalletLimit { get; set; }

        public int ProcessId { get; set; }

        public IList<ReceivedCarton> Cartons { get; set; }
    }
}



//$Id: Pallet.cs 25204 2014-07-04 09:20:39Z asharma $