using System.Collections.Generic;
using System.Linq;
using Sitecore.XConnect.Collection.Model;

namespace DEWAXP.Feature.FormsExtensions.ValueProviders.xDbFieldValueBinders.ContactPhoneNumbers
{
    public class XDbNumberFieldValueBinder : PreferredPhoneNumbersFieldValueBinder
    {
        protected override IFieldValueBinderResult GetFieldBindingValueFromFacet(PhoneNumber phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber.Number))
                return new NoFieldValueBindingFoundResult();
            return new FieldValueBindingFoundResult(phoneNumber.Number);
        }

        public override void StoreValue(object newValue)
        {
            if (newValue is string value)
            {
                UpdateFacet(x => x.PreferredPhoneNumber.Number = value);
            }
            if (newValue is List<string> valueList)
            {
                UpdateFacet(x => x.PreferredPhoneNumber.Number = valueList.FirstOrDefault());
            }
        }
        
    }
}