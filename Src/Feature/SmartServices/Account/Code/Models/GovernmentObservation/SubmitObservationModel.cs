using DEWAXP.Foundation.DataAnnotations;
using System;

namespace DEWAXP.Feature.Account.Models.GovernmentObservation
{
    public class SubmitObservationModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string MobileNumber { get; set; }

        public DateTime Date { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Area { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Road { get; set; }

        public string Structure { get; set; }

        public string ContactAccountNumber { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Defect { get; set; }

        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string ElectricityOrWater { get; set; }

        public string Community { get; set; }

        public bool SendCoordinates { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string ApiKey { get; set; }
    }
}