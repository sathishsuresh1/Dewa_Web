using DEWAXP.Feature.SupplyManagement.Models.MoveTo;
using Sitecore;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.ModelBinders
{
    public class MoveToModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as MoveToAccount; //Bind most of the model using the built-in binder, this will bind all primitives for us

            const string magicString = "DisconnectDate";

            var sDisconnectDate = bindingContext.ValueProvider.GetValue(magicString).AttemptedValue; //Get the posted value

            if (!string.IsNullOrEmpty(sDisconnectDate))
            { //Check we have a value
                bindingContext.ModelState.Remove(magicString);

                try
                {
                    CultureInfo culture;
                    DateTimeStyles styles;
                    DateTime dateResult;

                    culture = Context.Culture;
                    if ((System.Convert.ToString(culture) ?? "").Equals("ar-AE"))
                    {
                        sDisconnectDate = (sDisconnectDate ?? "").Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    }
                    styles = DateTimeStyles.None;
                    if (DateTime.TryParse(sDisconnectDate, culture, styles, out dateResult))
                    {
                        if (dateResult != null)
                            model.DisconnectDate = dateResult.ToString("dd MMMM yyyy");
                    }
                }
                catch (FormatException ex)
                {
                    bindingContext.ModelState.AddModelError(magicString, ex); //Add an error to the model state, used for ModelState.IsValid and Html error helpers
                }
                catch (Exception ex)
                { //Unexpected exception
                    throw ex;
                }
            }

            return model;
        }
    }
}