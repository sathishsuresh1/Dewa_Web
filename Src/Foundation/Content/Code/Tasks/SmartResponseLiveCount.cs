// <copyright file="YoutubeAPIEn.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Tasks
{
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Integration.Impl.DewaSvc;
    using Glass.Mapper.Sc;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Publishing;
    using System;
    using SitecoreX = Sitecore;

    /// <summary>
    /// Defines the <see cref="SmartResponseLiveCount" />
    /// </summary>
    public class SmartResponseLiveCount
    {
        public SmartResponseLiveCount()
        {
        }

        private static Database masterdb = Database.GetDatabase("master");
        private static Item SmartreswaterItemid = masterdb.GetItem(new ID(SitecoreItemIdentifiers.Smart_Response_Config));
        private static ISitecoreService _service = new SitecoreService("master");
        public static SmartResponseWaterModel SmartreswaterItem = _service.GetItem<SmartResponseWaterModel>(new Guid(SitecoreItemIdentifiers.Smart_Response_Config));

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="items">The items<see cref="Item[]"/></param>
        /// <param name="command">The command<see cref="SitecoreX.Tasks.CommandItem"/></param>
        /// <param name="schedule">The schedule<see cref="SitecoreX.Tasks.ScheduleItem"/></param>
        public void Execute(Item[] items, SitecoreX.Tasks.CommandItem command, SitecoreX.Tasks.ScheduleItem schedule)
        {
            SitecoreX.Diagnostics.Log.Info("Smart Response Water Count Scheduled task is being run!", this);
            if (SmartreswaterItem != null && !string.IsNullOrWhiteSpace(SmartreswaterItem.WaterUsageCount))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(SmartreswaterItem.WaterUsageCount))
                    {
                        var DewaApiClient = new DewaSoapClient();
                        var response = DewaApiClient.GetWaterSaveNotification();
                        if (response != null && response.Succeeded && response.Payload != null && response.Payload.@return != null && !string.IsNullOrWhiteSpace(response.Payload.@return.watersavings))
                        {
                            SmartreswaterItemid.Editing.BeginEdit();
                            SmartreswaterItemid.Fields["Water Usage Count"].Value = string.Format("{0:#,###0.00}", Convert.ToDecimal(response.Payload.@return.watersavings.Trim())); // {0:#,###0.00}
                            SmartreswaterItemid.Editing.EndEdit();
                            DEWAYoutubeHelper.PublishItem(SmartreswaterItemid, false, false, PublishMode.Smart);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SitecoreX.Diagnostics.Log.Error("Smart Response Water Live Count Scheduled task is getting Error -" + ex.ToString() + " Message - " + ex.Message.ToString(), this);
                }
            }
        }
    }
}