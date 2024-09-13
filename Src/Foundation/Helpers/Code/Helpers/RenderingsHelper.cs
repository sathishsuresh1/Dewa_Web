using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace DEWAXP.Foundation.Helpers
{
    public static class RenderingsHelper
    {
        public static bool ContainsRendering(this Item currentItem, ID renderingId)
        {
            if (currentItem != null)
            {
                ID validId;
                if (ID.TryParse(renderingId, out validId) && !validId.IsNull)
                {
                    var renderings = currentItem.Visualization.GetRenderings(global::Sitecore.Context.Device, false);
                    if (renderings != null && renderings.Any())
                    {
                        return renderings.Any(x => x.RenderingItem != null && x.RenderingItem.ID == validId);
                    }
                }
            }

            return false;
        }

        public static void RemoveFor<TModel>(this ModelStateDictionary modelState,
                                         Expression<Func<TModel, object>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.StartsWith(expressionText + "."))
                {
                    modelState.Remove(ms);
                }
            }
        }
    }
}