using DEWAXP.Feature.SupplyManagement.ModelBinders;
using DEWAXP.Foundation.Content.Models;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveTo
{
    [ModelBinder(typeof(MoveToModelBinder))]
    public class MoveToAccount
    {
        public SharedAccount SelectedAccount { get; set; }

        public string AccountNumber { get; set; }

        [DEWAXP.Foundation.DataAnnotations.Required]
        public string DisconnectDate { get; set; }

        public DateTime? DisconnectDateAsDateTime
        {
            get
            {
                DateTime dateTime;
                if (DateTime.TryParse(DisconnectDate, out dateTime))
                {
                    return dateTime;
                }
                return null;
            }
            set { }
        }

        [DEWAXP.Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [DEWAXP.Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
    }
}