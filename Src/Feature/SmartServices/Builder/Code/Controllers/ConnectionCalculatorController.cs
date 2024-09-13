using DEWAXP.Feature.Builder.Models.ConnectionCalculator;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Logger;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.Builder.Controllers
{
    public class ConnectionCalculatorController : BaseController
    {
        [HttpGet]
        public ActionResult NewRequest()
        {
            try
            {
                ConnectionCalculatorModel model = null;
                model = ContextRepository.GetCurrentItem<ConnectionCalculatorModel>();
                return PartialView("~/Views/Feature/Builder/ConnectionCalculator/_NewRequest.cshtml", model);
            }
            catch (Exception ex)
            {
                LogService.Debug(ex);
            }
            return new EmptyResult();
        }
    }
}