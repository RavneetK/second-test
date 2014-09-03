using DcmsMobile.Inquiry.Helpers;

namespace DcmsMobile.Inquiry.Areas.Inquiry.CartonAreaEntity
{
    internal class CartonLocation
    {
        public string LocationId { get; set; }

        public string ShortName { get; set; }

        public string Area { get; set; }

        public string WhId { get; set; }

        public int? Capacity { get; set; }

        public SkuBase AssignedSku { get; set; }

        /// <summary>
        /// This property is added store number of non pallet cartons on location
        /// </summary>
        public int NonPalletCartonCount { get; set; }
    }

    internal class CartonAtLocation
    {
        public string CartonId { get; set; }

        public int? SKUQuantity { get; set; }

        public string PalletId { get; set; }
    }
}




//$Id: CartonLocation.cs 26020 2014-08-20 12:03:21Z ssinghal $