using Sitecore.ExperienceForms.Mvc.Models;

namespace DEWAXP.Feature.FormsExtensions.ValueProviders
{
    public interface IFieldValueBinderMapFactory
    {
        IFieldValueBinder GetBindingHandler(ValueProviderSettings bindingSettingsValueProviderSettings);
    }
}
