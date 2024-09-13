using Sitecore.Data.Items;
using Sitecore.Events;
using System;

namespace DEWAXP.Foundation.Content.Events
{
    // Found via http://blog.falafel.com/sitecore-remove-spaces-from-urls/
    /// <summary>
    /// Event handler to ensure items use dashes instead of spaces
    /// </summary>
    public class ItemEventHandler
    {
        protected void HandleItemName(object sender, EventArgs args)
        {
            var item = (Item)Event.ExtractParameter(args, 0);
            string processedName;

            if (item.Database.Name != "master"
                || !item.Paths.Path.StartsWith("/sitecore/content/Home/") && !item.Paths.Path.StartsWith("/sitecore/content/DSCE/")
                && !item.Paths.Path.StartsWith("/sitecore/content/DEWA/Home/")
                && !item.Paths.Path.StartsWith("/sitecore/content/Suqia/home/")
                || item.Name == (processedName = item.Name.ToLower().Replace(' ', '-')))
            {
                return;
            }

            item.Editing.BeginEdit();
            try
            {
                item.Appearance.DisplayName = item.Name;
                item.Name = processedName;
            }
            finally
            {
                item.Editing.EndEdit();
            }
        }

        protected void HandleItemNameCopied(object sender, EventArgs args)
        {
            var item = (Item)Event.ExtractParameter(args, 1);
            string processedName;

            if (item.Database.Name != "master"
                || !item.Paths.Path.StartsWith("/sitecore/content/Home/")
                || item.Name == (processedName = item.Name.ToLower().Replace(' ', '-')))
            {
                return;
            }

            item.Editing.BeginEdit();
            try
            {
                item.Appearance.DisplayName = item.Name;
                item.Name = processedName;
            }
            finally
            {
                item.Editing.EndEdit();
            }
        }
    }
}