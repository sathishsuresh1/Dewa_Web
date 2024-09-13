using Sitecore.ExperienceForms.Models;

namespace DEWAXP.Feature.FormsExtensions.Fields.ValueResolvers
{
    public interface IFormsFieldValueResolver
    {
        string GetStringFieldValue(IViewModel fieldViewModel);
    }
}