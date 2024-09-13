using System;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Serializable]
    public class ApplyEVCard_NonDewa : EVPreRegistrationModel
    {
        //public string AccountType { get; set; }

        //Personal or Business
        public string Name { get; set; }

        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }

        public string NoofCards { get; set; }

        public string mulkiyanum { get; set; }

        //Mulkiya or Car Plate
        public string CardIdType { get; set; }

        public string CardIdNumber { get; set; }

        //public string mulkiya { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument { get; set; }
    }
}