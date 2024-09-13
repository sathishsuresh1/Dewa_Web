using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.DRRG.Models
{
    public class InverterModule
    {
        public InverterModule()
        {
            LstInternalInterface = GenericExtension.GetLstDataSource(DataSources.DRRG_YESNO);
            LstPowerFactorRange = GenericExtension.GetLstDataSource(DataSources.DRRG_PowerFactorRange);
            FileList = new Dictionary<string, fileResult>();
            LstDCAC = GenericExtension.GetLstDataSource(DataSources.DRRG_DCACGalvanicIsolation);
            LstPowerDerating = GenericExtension.GetLstDataSource(DataSources.DRRG_PowerDerating);
            LstPossibilityEarthing = GenericExtension.GetLstDataSource(DataSources.DRRG_PossibilityEarthing);
        }
        public string ModelName { get; set; }
        public string Manufacturer_Code { get; set; }
        public long Id { get; set; }
        public string Status { get; set; }
        public string InverterID { get; set; }
        public List<string> RatedPower { get; set; }
        public List<string> MaximumAcApparentPower { get; set; }
        public List<string> MaximumActivePower { get; set; }
        public string PowerFactorRange { get; set; }
        public string OtherPowerFactorRange { get; set; }
        public string PossibilityEarthing { get; set; }
        public string NumberOfPhases { get; set; }
        public string RemoteControl { get; set; }
        public string RemoteMonitoring { get; set; }
        public string LVRT { get; set; }
        public string DegreeofProtection { get; set; }
        public string InternalInterface { get; set; }
        public string Multimasterfeature { get; set; }
        public string Functionofstring { get; set; }
        public string NumberofString { get; set; }
        public string MultiMPPTSection { get; set; }
        public string Numberofsection { get; set; }
        public string DCACSection { get; set; }
        public string PowerDerating { get; set; }
        public List<SelectListItem> LstPowerDerating { get; set; }
        public List<SelectListItem> LstInternalInterface { get; set; }
        public List<SelectListItem> LstPowerFactorRange { get; set; }
        public List<SelectListItem> LstDCAC { get; set; }
        public List<SelectListItem> LstPossibilityEarthing { get; set; }

        public HttpPostedFileBase ModelDataSheet { get; set; }
        public HttpPostedFileBase Document621091 { get; set; }
        public HttpPostedFileBase Document1741 { get; set; }
        public HttpPostedFileBase Document6100032 { get; set; }
        public HttpPostedFileBase Document61000312 { get; set; }
        public HttpPostedFileBase Document6100061 { get; set; }
        public HttpPostedFileBase Document6100062 { get; set; }
        public HttpPostedFileBase Document6100063 { get; set; }
        public HttpPostedFileBase Document6100064 { get; set; }
        public HttpPostedFileBase DewaDRRGStandard { get; set; }
        public HttpPostedFileBase HarmonicSpectrum { get; set; }
        public Dictionary<string, fileResult> FileList { get; set; }
        public string signatureCopy { get; set; }
        public string UsageCategory { get; set; }

    }
    public class fileResult
    {
        public long fileId { get; set; }
        public string fileName { get; set; }
        public byte[] content { get; set; }
    }
}