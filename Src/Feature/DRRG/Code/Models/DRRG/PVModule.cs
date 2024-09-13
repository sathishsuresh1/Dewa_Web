using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Extensions;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.DRRG.Models
{
    public class PVModule
    {
        public PVModule()
        {
            NominalPowerEntries = GenericExtension.GetLstDataSource(DataSources.DRRG_NOMINALPOWERENTRIES);
            CellTechnologies = GenericExtension.GetLstDataSource(DataSources.DRRG_CELLTECHNOLOGY);
            CellStructureOptions = GenericExtension.GetLstDataSource(DataSources.DRRG_CELLSTRUCTURE);
            LstFramed = GenericExtension.GetLstDataSource(DataSources.DRRG_YESNO);
            LstFrontSuperstrate = GenericExtension.GetLstDataSource(DataSources.DRRG_FRONTSUPERSTRATE);
            LstEncapsulant = GenericExtension.GetLstDataSource(DataSources.DRRG_ENCAPSULANT);
            LstDCSystemgroup = GenericExtension.GetLstDataSource(DataSources.DRRG_DC_SYSTEM_GROUPING);
            LstPositionofJunctionBox = GenericExtension.GetLstDataSource(DataSources.DRRG_POSITIONOFJUNCTIONBOX);
            LstMaterialofJunctionBox = GenericExtension.GetLstDataSource(DataSources.DRRG_MATERIALOFJUNCTIONBOX);
            LstTerminations = GenericExtension.GetLstDataSource(DataSources.DRRG_TERMINATIONS);
            LstFeaturesofJunctionBox = GenericExtension.GetLstDataSource(DataSources.DRRG_FEATURESOFJUNCTIONBOX);
            LstSaltMistTestMethods = GenericExtension.GetLstDataSource(DataSources.DRRG_SALT_MIST_TEST_METHODS);
            LstNewApprovedEquipments = new List<SelectListItem>();
        }

        public string Manufacturer_Code { get; set; }
        public string SelectedNominalPowerEntry { get; set; }
        public long Id { get; set; }
        public string Status {get;set;}
        public string PVId { get; set; }
        public string ModelName { get; set; }
        public string NominalPower { get; set; }
        public string Singlenominalpower { get; set; }
        public List<string> MultinominalPower { get; set; }
        public List<PVMultiNominal> pVMultiNominals { get; set; }
        public string NumberofMultinominals { get; set; }
        public string MaximumPowerVoltage { get; set; }
        public string MaximumPowerCurrent { get; set; }
        public string OpenCircuitVoltage { get; set; }
        public string ShortCircuitCurrent { get; set; }
        public string TemperatureCoefficientofIsc { get; set; }
        public string TemperatureCoefficientofVoc { get; set; }
        public string NominalOperatingCellTemp { get; set; }
        public string NominalPoweratNOCTCondition { get; set; }
        public string ModuleLength { get; set; }
        public string ModuleWidth { get; set; }
        public string CellTechnology { get; set; }
        public string OtherCellTechnology { get; set; }
        public IEnumerable<string> CellStructure { get; set; }
        public string OtherCellStructure { get; set; }
        public string SelectedCellStructure { get; set; }
        public bool Bifacial { get; set; }
        public string Framed { get; set; }
        public string FrontSuperstrate { get; set; }
        public string OtherFrontSuperstrate { get; set; }
        public string BackSuperstrate { get; set; }
        public string OtherBackSuperstrate { get; set; }
        public string Encapsulant { get; set; }
        public string OtherEncapsulant { get; set; }
        public string DCSystemGroupMandatory { get; set; }
        public string DCSystemGroup { get; set; }
        public string PositionofJunctionBox { get; set; }
        public string MaterialofJunctionBox { get; set; }
        public string OtherMaterialofJunctionBox { get; set; }
        public string Terminations { get; set; }
        public string OtherTerminations { get; set; }
        public string FeaturesofJunctionBox { get; set; }
        public string OtherFeaturesofJunctionBox { get; set; }
        public List<SelectListItem> NominalPowerEntries { get; set; }
        public List<SelectListItem> CellTechnologies { get; set; }
        public List<SelectListItem> CellStructureOptions { get; set; }
        public List<SelectListItem> LstFramed { get; set; }
        public List<SelectListItem> LstFrontSuperstrate { get; set; }
        public List<SelectListItem> LstEncapsulant { get; set; }
        public List<SelectListItem> LstDCSystemgroup { get; set; }
        public List<SelectListItem> LstPositionofJunctionBox { get; set; }
        public List<SelectListItem> LstMaterialofJunctionBox { get; set; }
        public List<SelectListItem> LstTerminations { get; set; }
        public List<SelectListItem> LstFeaturesofJunctionBox { get; set; }
        public List<SelectListItem> LstSaltMistTestMethods { get; set; }
        public List<SelectListItem> LstNewApprovedEquipments { get; set; }
        public HttpPostedFileBase ModelDataSheet { get; set; }
        public HttpPostedFileBase Document1 { get; set; }
        public HttpPostedFileBase Document2 { get; set; }
        public HttpPostedFileBase Document3 { get; set; }
        public HttpPostedFileBase Document4 { get; set; }
        public HttpPostedFileBase Document5 { get; set; }
        public string ModelDataSheetFilename { get; set; }
        public string Document1Filename { get; set; }
        public string Document2Filename { get; set; }
        public string Document3Filename { get; set; }
        public string Document4Filename { get; set; }
        public string Document5Filename { get; set; }
        public byte[] ModelDataSheetBinary { get; set; }
        public byte[] Document1Binary { get; set; }
        public byte[] Document2Binary { get; set; }
        public byte[] Document3Binary { get; set; }
        public byte[] Document4Binary { get; set; }
        public byte[] Document5Binary { get; set; }
        public string signatureCopy { get; set; }

    }

    public class PVMultiNominal
    {
        public string wp1 { get; set; }
        public string wp2 { get; set; }
        public string wp3 { get; set; }
        public string mpv1 { get; set; }
        public string mpc1 { get; set; }
        public string ocv1 { get; set; }
        public string scc1 { get; set; }
        public string tci1 { get; set; }
        public string tcv1 { get; set; }
        public string noct1 { get; set; }
        public string npnoct1 { get; set; }
    }
}