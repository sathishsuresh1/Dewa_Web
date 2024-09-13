using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Publishing;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using System;

namespace DEWAXP.Foundation.Content.Events
{
    public class PublishingStatistics
    {
        /// <summary>
        /// Updates the published date and published by fields in item : /sitecore/content/Global Config/Publishing Information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void UpdatePublishStatistics(object sender, EventArgs args)
        {
            var publisher = (Publisher)Event.ExtractParameter(args, 0);
            var itemId = "{E658366B-59C9-463A-9654-7F791CA1EE33}"; //sitecore/content/Global Config/Publishing Information

            if (publisher != null)
            {
                Assert.ArgumentNotNull(publisher, "publisher");
                Log.Info(GetType().FullName + ": Updating Publish Date - START", this);

                var publishDate = publisher.Options.PublishDate.ToLocalTime();

                using (new SecurityDisabler())
                {
                    Database masterDB = Database.GetDatabase("master");
                    var item = masterDB.GetItem(itemId);
                    if (item != null)
                    {
                        item.Editing.BeginEdit();
                        try
                        {
                            if (publisher.Options.RootItem != null && publisher.Options.RootItem.TemplateID.Guid.Equals(Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")))
                            {
                                item.Fields["Latest Published News"].Value = DateUtil.ToIsoDate(publishDate);
                            }
                            item.Fields["Published On"].Value = DateUtil.ToIsoDate(publishDate);
                            item.Fields["Published By"].Value = User.Current.Name; ;

                            item.Editing.EndEdit();
                            Log.Info(GetType().FullName + ": Updating Publish Date - END", this);
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error(GetType().FullName + ": Could not update Publish Date - END" + "-----" + ex.Message, this);
                            item.Editing.CancelEdit();
                        }
                    }
                }
            }
        }
    }
}