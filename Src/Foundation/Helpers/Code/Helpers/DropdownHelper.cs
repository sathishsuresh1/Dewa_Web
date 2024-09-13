using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Helpers
{
    public interface IDropdownHelper
    {
        IEnumerable<SelectListItem> CityDropdown(CityList cityList);

        IEnumerable<SelectListItem> ComplaintsDropdown(ComplaintCodeList codeList);
    }
    [Service(typeof(IDropdownHelper),Lifetime =Lifetime.Transient)]
    public class DropdownHelper : IDropdownHelper
    {
        public IEnumerable<SelectListItem> CityDropdown(CityList cityList)
        {
            var cities = cityList.Cities;
            var list = new List<SelectListItem>();

            foreach (var city in cities.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                try
                {
                    var split = city?.Split('-');

                    list.Add(new SelectListItem { Text = Convert.ToInt32(split?.Length) > 1 ? split[1].Trim() : split[0], Value = city.Trim() });
                }
                catch (Exception ex)
                {
                    Error.LogError(string.Format("Could not split city text and value with default text of '{0}'. Message: {1})", city, ex.Message));
                    list.Add(new SelectListItem { Text = city.Trim(), Value = city.Trim() });
                }
            }

            list = list.OrderBy(x => x.Text).ToList();
            return list;
        }

        public IEnumerable<SelectListItem> ComplaintsDropdown(ComplaintCodeList codeList)
        {
            var codes = codeList.Codes;
            var list = codes
                .Select(code => new SelectListItem { Text = code.CodeDescription, Value = code.GroupCode + "|" + code.Code })
                .OrderBy(c => c.Text)
                .ToList();
            return list;
        }
    }
}