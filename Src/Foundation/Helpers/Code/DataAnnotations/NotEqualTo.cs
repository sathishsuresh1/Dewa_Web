using System.Globalization;
using Sitecore.Globalization;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;
using System.Collections.Generic;

namespace DEWAXP.Foundation.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NonEqualAttribute : ValidationAttribute, IClientValidatable, ISitecoreDictionaryDataAnnotation
	{
        private const string DefaultErrorMessage = "{0}";

        public string OtherProperty { get; private set; }

		public string ValidationMessageKey { get; set; }

		public NonEqualAttribute(string otherProperty) 
			: base(string.Format(DefaultErrorMessage, otherProperty))
        {
            OtherProperty = otherProperty;
        }
		
	    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

                if (value.Equals(otherPropertyValue))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;

        }

	    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "unlike",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("otherproperty", OtherProperty);

            yield return rule;
        }

	    public override string FormatErrorMessage(string name)
        {
            return Translate.Text(ValidationMessageKey, name);
        }
	}
}