using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;

namespace DEWAXP.Project.DEWA.sitecore.admin
{
    public partial class MigrateLayouts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Sitecore.Context.User.IsAdministrator)
            {
                Response.Write("You have no permission to run this script.");
                return;
            }

            var databaseName = Context.Request.QueryString["database"];
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = "master";
            }

            var database = Factory.GetDatabase(databaseName);

            ID startItemId = null;
            Item startItem = null;
            var startItemIdString = Context.Request.QueryString["itemId"];

            if (string.IsNullOrEmpty(startItemIdString))
            {
                Response.Write("No Item ID provided. Please use GET parameter 'itemId'.");
                return;
            }

            if (!string.IsNullOrEmpty(startItemIdString) &&
                !Sitecore.Data.ID.TryParse(startItemIdString, out startItemId))
            {
                Response.Write("Invalid Item ID.");
                return;
            }

            if (!string.IsNullOrEmpty(startItemIdString))
            {
                startItem = database.GetItem(Sitecore.Data.ID.Parse(startItemIdString));
            }

            if (!string.IsNullOrEmpty(startItemIdString) && startItem == null)
            {
                Response.Write(string.Format("No item found for ID {0}.", startItemIdString));
                return;
            }

            bool isRecursionEnabled;
            bool.TryParse(Context.Request.QueryString["enableRecursion"], out isRecursionEnabled);

            var fixRenderings = new UpgradeLayoutHelper(database);

            Sitecore.Diagnostics.Log.Info("Layout Migration: Started", this);

            fixRenderings.Iterate(startItem, isRecursionEnabled);

            Sitecore.Diagnostics.Log.Info("Layout Migration: Finished", this);
        }

        public class UpgradeLayoutHelper
        {
            private const string ItemsWithPresentationDetailsQuery =
                "{0}//*[@__Renderings != '' or @__Final Renderings != '']";

            private readonly Database _database;

            public UpgradeLayoutHelper(Database database)
            {
                _database = database;
            }

            public Dictionary<Item, List<KeyValuePair<string, string>>> Iterate(Item startItem, bool isRecursionEnabled)
            {
                var result = new Dictionary<Item, List<KeyValuePair<string, string>>>();

                var items = new List<Item>();

                if (startItem != null)
                {
                    items.Add(startItem);

                    if (isRecursionEnabled)
                    {
                        items.AddRange(_database.SelectItems(string.Format(ItemsWithPresentationDetailsQuery,
                            startItem.Paths.FullPath)));
                    }
                }
                else
                {
                    items.AddRange(
                        _database.SelectItems(string.Format(ItemsWithPresentationDetailsQuery, "/sitecore/content")));
                }

                foreach (var itemInDefaultLanguage in items)
                {
                    UpdateLayoutField(itemInDefaultLanguage);
                }

                return result;
            }

            public void UpdateLayoutField(Item item)
            {
                bool isSharedLayoutFieldUpdated = false;

                foreach (var language in item.Languages)
                {
                    Item itemInLanguage = _database.GetItem(item.ID, language);
                    if (itemInLanguage.Versions.Count > 0)
                    {
                        foreach (Item itemVersion in itemInLanguage.Versions.GetVersions())
                        {
                            foreach (Field f in itemVersion.Fields)
                            {
                                if (f.ID == FieldIDs.FinalLayoutField)
                                {
                                    itemVersion.Editing.BeginEdit();
                                    string fieldValue = Sitecore.Data.Fields.LayoutField.GetFieldValue(itemVersion.Fields[FieldIDs.FinalLayoutField]);
                                    LayoutField.SetFieldValue(f, fieldValue);
                                    itemVersion.Editing.EndEdit();
                                }
                            }

                            if (!isSharedLayoutFieldUpdated)
                            {
                                foreach (Field f in itemVersion.Fields)
                                {
                                    if (f.ID == FieldIDs.LayoutField)
                                    {
                                        itemVersion.Editing.BeginEdit();
                                        string fieldValue = LayoutField.GetFieldValue(itemVersion.Fields[FieldIDs.LayoutField]);
                                        LayoutField.SetFieldValue(f, fieldValue);
                                        itemVersion.Editing.EndEdit();

                                        isSharedLayoutFieldUpdated = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}