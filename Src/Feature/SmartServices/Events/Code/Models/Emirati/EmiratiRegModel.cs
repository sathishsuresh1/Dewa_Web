using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.Emirati
{
    public class EmiratiRegModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Designation { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string PrNo { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public bool Tc { get; set; }

        public string Entrytype { get; set; }
        [Required(ValidationMessageKey = "generic validation message")]
        public bool IsVaccinated {
            get
            {
                if (ProofType == "vacc")
                    return true;
                else
                    return false;
            }
        }
        [Required(ValidationMessageKey = "generic validation message")]
        public bool Will_provide_rtpcr {
            get
            {
                if (ProofType == "rtpcr")
                    return true;
                else
                    return false;
            }
        }
        
        public string ProofType { get; set; }

    }

    public class VerficationModel {
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }
    }

}