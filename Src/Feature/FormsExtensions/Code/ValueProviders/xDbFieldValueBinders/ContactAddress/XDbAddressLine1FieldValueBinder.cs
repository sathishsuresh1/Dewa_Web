﻿using System.Collections.Generic;
using System.Linq;
using Sitecore.XConnect.Collection.Model;

namespace DEWAXP.Feature.FormsExtensions.ValueProviders.xDbFieldValueBinders.ContactAddress
{
    public class XDbAddressLine1FieldValueBinder: PreferredAddressFieldValueBinder
    {

        protected override IFieldValueBinderResult GetFieldBindingValueFromFacet(Address addres)
        {
            if (string.IsNullOrEmpty(addres.AddressLine1))
                return new NoFieldValueBindingFoundResult();
            return new FieldValueBindingFoundResult(addres.AddressLine1);
        }

        public override void StoreValue(object newValue)
        {
            if (newValue is string addressLine1)
            {
                UpdateFacet(x => x.PreferredAddress.AddressLine1=addressLine1);
            }
            if (newValue is List<string> valueList)
            {
                UpdateFacet(x => x.PreferredAddress.AddressLine1 = valueList.FirstOrDefault());
            }
        }

    }
}