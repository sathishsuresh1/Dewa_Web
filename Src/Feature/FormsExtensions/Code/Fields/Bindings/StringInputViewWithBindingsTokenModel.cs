﻿using System;
using Sitecore.Data.Items;
using Sitecore.ExperienceForms.Mvc.Models.Fields;

namespace DEWAXP.Feature.FormsExtensions.Fields.Bindings
{
    [Serializable]
    public class StringInputViewWithBindingsTokenModel : StringInputViewModel, IBindingSettings
    {
        public bool StoreBindingValue { get; set; }

        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            this.InitBindingSettingsProperties(item);
        }

        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            this.UpdateBindingSettingsFields(item);
        }

    }
}