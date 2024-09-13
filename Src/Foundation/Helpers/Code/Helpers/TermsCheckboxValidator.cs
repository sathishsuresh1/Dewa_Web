using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace DEWAXP.Foundation.Helpers
{
	public class TermsCheckboxValidator : BaseValidator
	{
		private CheckBox _checkBoxToValidate;
		protected CheckBox CheckBoxToValidate
		{
			get
			{
				if (_checkBoxToValidate == null)
				{
					_checkBoxToValidate = base.FindControl(ControlToValidate) as CheckBox;
				}
				return _checkBoxToValidate;
			}
		}

		protected override bool ControlPropertiesValid()
		{
			if (string.IsNullOrEmpty(base.ControlToValidate))
			{
				throw new HttpException(string.Format("The ControlToValidate property of '{0}' cannot be blank.", this.ID));
			}

			if (this.CheckBoxToValidate == null)
			{
				throw new HttpException(string.Format("The CheckBoxValidator can only validate controls of type CheckBox."));
			}
			return true;
		}

		protected override bool EvaluateIsValid()
		{
			this.ErrorMessage = string.Format(this.ErrorMessage, "{0}", CheckBoxToValidate.Text);
			//Validate whether checkbox is checked
			return this.CheckBoxToValidate.Checked;
		}
	}

    public static class HtmlAttributesExtensions
    {
        public static IDictionary<string, object> AddHtmlAttrItem(this object obj, string name, object value, bool condition)
        {
            var items = !condition ? new RouteValueDictionary(obj) : new RouteValueDictionary(obj) { { name, value } };
            return UnderlineToDashInDictionaryKeys(items);
        }
        public static IDictionary<string, object> AddHtmlAttrItem(this IDictionary<string, object> dictSource, string name, object value, bool condition)
        {
            if (!condition)
                return dictSource;

            dictSource.Add(name, value);
            return UnderlineToDashInDictionaryKeys(dictSource);
        }
        private static IDictionary<string, object> UnderlineToDashInDictionaryKeys(IDictionary<string, object> items)
        {
            var newItems = new RouteValueDictionary();
            foreach (var item in items)
            {
                newItems.Add(item.Key.Replace("_", "-"), item.Value);
            }
            return newItems;
        }
    }
}