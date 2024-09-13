using DEWAXP.Foundation.DataAnnotations;
using System;

namespace DEWAXP.Feature.GeneralServices.Models.GreenRibonPledge
{
    [Serializable]
    public class PledgeForm
    {
        public string ChkReplaceBulbs { get; set; }

        public string NumberOfBulbs { get; set; }

        public string ChkTempTo24 { get; set; }

        public string ACTemperature { get; set; }

        public string ChkMaintenanceTwice { get; set; }

        public string Periodicmaintenance { get; set; }

        public string ChkDryingClothes { get; set; }

        public string ChkSolarWaterHeaters { get; set; }

        public string NumberOfElectricHeaters { get; set; }

        public string ChkBucketInsteadOfWaterHose { get; set; }

        public string ChkReduceMinutesInShower { get; set; }

        public string EnterNumberOfMinutes { get; set; }

        public string ChkFixWaterFlowReducer { get; set; }

        public string NumberOfKits { get; set; }
        public string NumberOfPersons { get; set; }

        public string ChkReduceAmountOfWater { get; set; }

        public string ChkWaterTapWhileBrushing { get; set; }

        public string ChkDewaGreenBox { get; set; }

        public string LblKGOfCarbonDioxide { get; set; }

        public string LblKilloWattOfElectricity { get; set; }

        public string LblGallonsOfWater { get; set; }

        public string LblWaterBottlesSaved { get; set; }

        public string LblCflLamps { get; set; }

        public string LblTreesOffset { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "greenpledge.nameerror")]
        public string name { get; set; }

        public string account_number { get; set; }

        [Required(ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string contact_number { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string email { get; set; }

        public int PledgeNumber { get; set; }

        public string formdata { get; set; }

        public string PledgeType { get; set; }
        public string Recaptcha { get; set; }
    }
}