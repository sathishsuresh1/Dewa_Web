using DEWAXP.Feature.Bills.Models.TariffCalculator;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.TariffCalculator;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DEWAXP.Feature.Bills.Controllers
{
    /// <summary>
    /// Not the best approach to do this but front-end markup was too harcdoded
    /// Todo: Should refactor this to be more dynamic (e.g. table rows should be dynamic (number of rows/values, colours, values) Should not expect
    /// same number of values, types, etct.
    /// </summary>
    public class TariffCalculatorController : BaseController
    {
        public override ActionResult Index()
        {
            var ret = new CalculatorViewModel();

            //get datasource item and read its children to populate customer type dropdown
            var source = RenderingRepository.GetDataSourceItem<Calculator>();
            if (source != null && source.Id != Guid.Empty && source.CustomerTypes != null)
            {
                var customerTypes = source.CustomerTypes.ToList();
                if (customerTypes.Any())
                {
                    source.CustomerTypes = customerTypes;
                    ret.CalculatorDs = source;

                    ret.Residential = new CustomerTypeValue();
                    ret.Industrial = new CustomerTypeValue();
                    ret.Commercial = new CustomerTypeValue();
                    ret.D33 = new CustomerTypeValue();

                    AddCustomerTypeValues(source, ret.Residential, "residential");
                    AddCustomerTypeValues(source, ret.Industrial, "industrial");
                    AddCustomerTypeValues(source, ret.Commercial, "commercial");
                    AddCustomerTypeValues(source, ret.D33, "d33");

                    #region [Generic DataHandle]
                    ret.TariffJsonList = new System.Collections.Generic.List<TariffJsonData>();
                    foreach (var customerType in customerTypes)
                    {
                        var getFormatterData = GetFormatterData(customerType);
                        string _typeName = customerType.Name.ToLowerInvariant();

                        ret.TariffJsonList.Add(new TariffJsonData()
                        {
                            dtype = _typeName,
                            data = new TariffData(getFormatterData)
                        });
                    }

                    ret.tariffJson = new JavaScriptSerializer().Serialize(ret.TariffJsonList);
                    #endregion

                }
            }
            return View("~/Views/Feature/Bills/TariffCalculator/_TariffCalculator.cshtml", ret);
        }

        private static void AddCustomerTypeValues(Calculator calculator, CustomerTypeValue customerTypeObject, string customerTypeName)
        {
            var custType = calculator.CustomerTypes.Where(x => x.Name.ToLowerInvariant().Equals(customerTypeName)).FirstOrDefault();
            if (custType != null)
            {
                var custTypeElect = custType.TariffTypes.Where(o => o.Name.ToLowerInvariant().Equals("electricity")).FirstOrDefault();
                if (custTypeElect != null)
                {
                    if (custTypeElect.Consumption != null && custTypeElect.Consumption.Any())
                    {
                        customerTypeObject.Electricity = new TariffTypeValue();
                        customerTypeObject.Electricity.FuelSurchargeTariff = custTypeElect.FuelSurchargeTariff;
                        customerTypeObject.Electricity.Red = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("red")).FirstOrDefault();
                        customerTypeObject.Electricity.Green = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("green")).FirstOrDefault();
                        customerTypeObject.Electricity.Orange = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("orange")).FirstOrDefault();
                        customerTypeObject.Electricity.Yellow = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("yellow")).FirstOrDefault();
                    }
                }

                var custTypeWater = custType.TariffTypes.Where(o => o.Name.ToLowerInvariant().Equals("water")).FirstOrDefault();
                if (custTypeWater != null)
                {
                    if (custTypeWater.Consumption != null && custTypeWater.Consumption.Any())
                    {
                        customerTypeObject.Water = new TariffTypeValue();
                        customerTypeObject.Water.FuelSurchargeTariff = custTypeWater.FuelSurchargeTariff;
                        customerTypeObject.Water.Red = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("red")).FirstOrDefault();
                        customerTypeObject.Water.Green = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("green")).FirstOrDefault();
                        customerTypeObject.Water.Orange = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("orange")).FirstOrDefault();
                        customerTypeObject.Water.Yellow = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("yellow")).FirstOrDefault();
                    }
                }
            }
        }


        private CustomerTypeValue GetFormatterData(CustomerType custType)
        {
            CustomerTypeValue customerTypeObject = new CustomerTypeValue();
            if (custType != null)
            {
                var custTypeElect = custType.TariffTypes.Where(o => o.Name.ToLowerInvariant().Equals("electricity")).FirstOrDefault();
                if (custTypeElect != null)
                {
                    if (custTypeElect.Consumption != null && custTypeElect.Consumption.Any())
                    {
                        customerTypeObject.Electricity = new TariffTypeValue();
                        customerTypeObject.Electricity.FuelSurchargeTariff = custTypeElect.FuelSurchargeTariff;
                        customerTypeObject.Electricity.Red = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("red")).FirstOrDefault();
                        customerTypeObject.Electricity.Green = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("green")).FirstOrDefault();
                        customerTypeObject.Electricity.Orange = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("orange")).FirstOrDefault();
                        customerTypeObject.Electricity.Yellow = custTypeElect.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("yellow")).FirstOrDefault();
                    }
                }

                var custTypeWater = custType.TariffTypes.Where(o => o.Name.ToLowerInvariant().Equals("water")).FirstOrDefault();
                if (custTypeWater != null)
                {
                    if (custTypeWater.Consumption != null && custTypeWater.Consumption.Any())
                    {
                        customerTypeObject.Water = new TariffTypeValue();
                        customerTypeObject.Water.FuelSurchargeTariff = custTypeWater.FuelSurchargeTariff;
                        customerTypeObject.Water.Red = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("red")).FirstOrDefault();
                        customerTypeObject.Water.Green = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("green")).FirstOrDefault();
                        customerTypeObject.Water.Orange = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("orange")).FirstOrDefault();
                        customerTypeObject.Water.Yellow = custTypeWater.Consumption.Where(x => x.Name.ToLowerInvariant().Equals("yellow")).FirstOrDefault();
                    }
                }
            }

            return customerTypeObject;
        }
    }

    //public static class TariffCalculatorManager
    //{
    //    public static List<>

    //}
}