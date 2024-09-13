using Sitecore.ExperienceForms.ValueProviders;

namespace DEWAXP.Feature.FormsExtensions.ValueProviders
{
    public interface IFieldValueBinder : IFieldValueProvider
    {
        void StoreValue(object newValue);
    }
}