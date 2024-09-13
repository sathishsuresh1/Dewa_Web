using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DEWA.Website.ModelBinders;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    [ModelBinder(typeof(MoveInIdentificationModelBinder))]
    public class IdentificationDetailsViewModel
    {
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false)]
        public string IDType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false)]
        public string IDNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd MMMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IdentityDocument { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IdentityDocumentCopy { get; set; }

        public string IdentityDocumentLabel1 { get; set; }

        public string IdentityDocumentLabel2 { get; set; }

	    public bool MultipleIdentityDocumentsRequired { get; set; }

        public string customertype { get; set; }
        public string customercategory { get; set; }

        public string IdTypeLabel1 { get; set; }
        public string IdTypeLabel2 { get; set; }

    }
}
