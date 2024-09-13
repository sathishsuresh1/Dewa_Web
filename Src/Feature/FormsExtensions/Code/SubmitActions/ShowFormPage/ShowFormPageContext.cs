using System;
using System.Web;

namespace DEWAXP.Feature.FormsExtensions.SubmitActions.ShowFormPage
{
    public class ShowFormPageContext
    {
        public static Guid? FormPage
        {
            get => (Guid?) HttpContext.Current.Items["NextFormPage"];
            set => HttpContext.Current.Items["NextFormPage"] = value;
        }
    }
}