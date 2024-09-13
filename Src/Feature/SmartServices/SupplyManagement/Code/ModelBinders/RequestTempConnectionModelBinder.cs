﻿using DEWAXP.Feature.SupplyManagement.Models.RequestTempConnection;
using Sitecore;
using System;
using System.Globalization;
using System.Web.Mvc;
using DateTime = System.DateTime;

namespace DEWAXP.Feature.SupplyManagement.ModelBinders
{
    public class RequestTempConnectionModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as NewRequest; //Bind most of the model using the built-in binder, this will bind all primitives for us
            if (model != null)
            {
                const string sEventStartDateName = "EventStartDate";
                const string sEventEndDateName = "EventEndDate";

                var EventStartDate = bindingContext.ValueProvider.GetValue(sEventStartDateName).AttemptedValue; //Get the posted value
                var EventEndDate = bindingContext.ValueProvider.GetValue(sEventEndDateName).AttemptedValue; //Get the posted value

                model.EventStartDate = this.FillModel(EventStartDate, sEventStartDateName, bindingContext);
                model.EventEndDate = this.FillModel(EventEndDate, sEventEndDateName, bindingContext);
            }
            return model;
        }

        private DateTime? FillModel(string sValue, string sName, ModelBindingContext bindingContext)
        {
            if (!string.IsNullOrEmpty(sValue))
            { //Check we have a value for
                bindingContext.ModelState.Remove(sName);

                try
                {
                    CultureInfo culture;
                    DateTimeStyles styles;
                    DateTime dateResult;

                    culture = Context.Culture;
                    styles = DateTimeStyles.None;
                    if (DateTime.TryParse(sValue, culture, styles, out dateResult))
                    {
                        return dateResult;
                    }
                }
                catch (FormatException ex)
                {
                    bindingContext.ModelState.AddModelError(sName, ex);
                    //Add an error to the model state, used for ModelState.IsValid and Html error helpers
                    return null;
                }
                catch (Exception ex)
                {
                    //Unexpected exception
                    throw ex;
                }
            }
            return null;
        }
    }
}