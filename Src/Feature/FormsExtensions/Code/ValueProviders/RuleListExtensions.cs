using System;
using DEWAXP.Feature.FormsExtensions.ValueProviderConditions;
using Sitecore.Rules;

namespace DEWAXP.Feature.FormsExtensions.ValueProviders
{
    public static class RuleListExtensions
    {
        public static bool EvaluateRules<T>(this RuleList<T> ruleList, T ruleContext)
            where T : ValueProviderConditionsRulesContext
        {
            if (ruleList == null)
            {
                throw new ArgumentNullException(nameof(ruleList));
            }

            if (ruleContext == null)
            {
                throw new ArgumentNullException(nameof(ruleContext));
            }

            if (ruleContext.IsAborted)
            {
                return false;
            }

            foreach (var rule in ruleList.Rules)
            {
                if (rule.Condition == null)
                {
                    continue;
                }

                var result = rule.Evaluate(ruleContext);
                if (ruleContext.IsAborted)
                {
                    return false;
                }

                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
    }
}