using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.ClearanceCertificate
{
    public class AnonymousClearanceCertificateModel : ClearanceCertificateModel
    {
        [Required]
        public string IdentityType { get; set; }

        [Required]
        public string IdentityNumber { get; set; }

        [Required]
        public string Emirate { get; set; }

        [Required]
        public string CustomerType { get; set; }

        public IEnumerable<SelectListItem> Emirates { get; set; }

        public IEnumerable<SelectListItem> IdentityTypes { get; set; }

        public IEnumerable<SelectListItem> CustomerTypes { get; set; }

        public string IsAccountNO { get; set; }
    }

    public class AnonymousClearanceCertificateSuccessModel
    {
        public string NotificationNumber { get; set; }
    }
}