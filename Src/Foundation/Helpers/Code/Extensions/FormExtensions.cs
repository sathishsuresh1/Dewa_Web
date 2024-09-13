using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl.VendorSvc;
using Glass.Mapper.Sc;
using SitecoreX = Sitecore.Context;
namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class FormExtensions
    {
        //public static string GetParameterValue(this FieldModel model, string @param)
        //{
        //    if (model.ParametersDictionary != null)
        //    {
        //        if (model.ParametersDictionary.ContainsKey(@param))
        //        {
        //            return model.ParametersDictionary[@param];
        //        }
        //    }
        //    return string.Empty;
        //}

        //public static string GetFieldPlaceholderText(this FieldModel model)
        //{
        //    return model.GetParameterValue("placeholder");
        //}

        //public static string GetFormFieldId(this FieldModel model)
        //{

        //    var pair = model.Form.Fields.Select((Value, Index) => new { Value, Index })
        //        .FirstOrDefault(p => p.Value.FieldDisplayName == model.FieldDisplayName);
        //    return model.Form.ShortFormId + "_Sections_0__Fields_" + pair.Index + "__FieldId";
        //}
        public static List<SelectListItem> GetNationalities(Dictionary<string, string> dropDownTermValues, bool useShortValues = true, bool usedefault = false)
        {
            var source = "en".Equals(SitecoreX.Language.CultureInfo.TwoLetterISOLanguageName)
                ? ConfigurationManager.AppSettings["ENNationalitiesXML"] : ConfigurationManager.AppSettings["ARNationalitiesXML"];

            var list = new List<SelectListItem>();

            using (var ds = new DataSet())
            {
                ds.ReadXml(source);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = row[2].ToString().AddToTermDictionary(row[1].ToString(), dropDownTermValues),
                        Value = useShortValues ? row[1].ToString() : row[2].ToString(),
                        Selected = !usedefault ? (row[1].ToString() == "AE" ? true : false) : false
                    });
                }
            }
            return list;
        }
        public static List<SelectListItem> GetCountryCodes()
        {
            var source = "en".Equals(SitecoreX.Language.CultureInfo.TwoLetterISOLanguageName)
                ? ConfigurationManager.AppSettings["ENCountryCodeXML"] : ConfigurationManager.AppSettings["ARCountryCodeXML"];

            var list = new List<SelectListItem>();

            using (var ds = new DataSet())
            {
                ds.ReadXml(System.Web.HttpContext.Current.Server.MapPath(source));

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = row[2].ToString(),
                        Value = row[1].ToString(),
                        Selected = row[1].ToString() == "AE" ? true : false
                    });
                }
            }
            return list;
        }
        public static List<SelectListItem> GetSAPCountryList(string lang, string segment)
        {
            SupportedLanguage supportedLanguage = SupportedLanguage.Arabic;
            if (!string.IsNullOrWhiteSpace(lang) && lang.ToLower().Equals("en"))
            {
                supportedLanguage = SupportedLanguage.English;
            }
            var countrylist = new List<SelectListItem>();
            IVendorServiceClient vendorServiceClient = new VendorSoapClient();
            var response = vendorServiceClient.GetCountryList(new DEWAXP.Foundation.Integration.SmartVendorSvc.GetCountryList(), supportedLanguage, HttpRequestExtensions.DetermineSegment(segment));
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.countryList != null && response.Payload.countryList.Count() > 0)
            {
                countrylist = response.Payload.countryList.Select(c => new SelectListItem
                {
                    Text = c.countryName,
                    Value = c.countryKey
                }).ToList();
                return countrylist;
            }
            return countrylist;
        }
        public static List<SelectListItem> GetYears(int startYear, int endYear)
        {
            var list = new List<SelectListItem>();
            int sYear = DateTime.Now.Year - startYear;
            int eYear = DateTime.Now.Year + endYear;
            for (int i = sYear; i <= eYear; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

            }

            return list;
        }
        
    }

}