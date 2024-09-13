using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.DRRG.Models
{
    public class InterfaceModule
    {
        public InterfaceModule()
        {
            LstInterfaceApplications = GenericExtension.GetLstDataSource(DataSources.DRRG_INTERFACEAPPLICATION);
            FileList = new Dictionary<string, fileResult>();
        }
        public long Id { get; set; }

        public string Interface_ID { get; set; }
        public string Status { get; set; }
        public string Manufacturer_Code { get; set; }
        public string ModelName { get; set; }
        public string Application { get; set; }
        public string CommunicationProtocol { get; set; }
        public string Compliance { get; set; }
        public List<SelectListItem> LstInterfaceApplications { get; set; }
        public HttpPostedFileBase ModelDataSheet { get; set; }
        public HttpPostedFileBase Document61850 { get; set; }
        public HttpPostedFileBase DocumentDEWAStandard { get; set; }
        public HttpPostedFileBase Document61010 { get; set; }
        public Dictionary<string, fileResult> FileList { get; set; }
        public string signatureCopy { get; set; }
    }
}