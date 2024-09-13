using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.VAT
{
    public class VATModel
    {
        public string accounts { get; set; }
        public string BusinessPartnerNumber { get; set; }

        public string Emirate { get; set; }

        public string VatNumber { get; set; }

        [DEWAXP.Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase VatDocument { get; set; }
    }
}