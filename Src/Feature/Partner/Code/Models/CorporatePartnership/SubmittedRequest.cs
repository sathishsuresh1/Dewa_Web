// <copyright file="SubmittedRequest.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Partner.Models.CorporatePartnership
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Content.Repositories;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Web;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using SitecoreX = global::Sitecore.Context;

    /// <summary>
    /// Defines the <see cref="SubmittedRequest" />.
    /// </summary>
    public class SubmittedRequest
    {
        /// <summary>
        /// Gets or sets the Eventname.
        /// </summary>
        public string Eventname { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Month.
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the Date.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the FromTime.
        /// </summary>
        public string FromTime { get; set; }

        /// <summary>
        /// Gets or sets the ToTime.
        /// </summary>
        public string ToTime { get; set; }

        /// <summary>
        /// Gets or sets the RequestType.
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// Gets or sets the ToList.
        /// </summary>
        public string ToList { get; set; }

        /// <summary>
        /// Gets or sets the FromDate.
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Gets or sets the ToDate.
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CancelledRequest.
        /// </summary>
        public bool CancelledRequest { get; set; }

        /// <summary>
        /// Gets or sets the Efolderid.
        /// </summary>
        public string Efolderid { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="CP_MSG_IDS_ISS" />.
    /// </summary>
    public class CP_MSG_IDS_ISS
    {
        /// <summary>
        /// Gets or sets the UpdatedEfid.
        /// </summary>
        public string UpdatedEfid { get; set; }

        /// <summary>
        /// Gets or sets the Partnername.
        /// </summary>
        public string Partnername { get; set; }

        /// <summary>
        /// Gets or sets the RequesterName.
        /// </summary>
        public string RequesterName { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the fromemail.
        /// </summary>
        public string fromemail { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CancelledRequest.
        /// </summary>
        public bool CancelledRequest { get; set; }

        /// <summary>
        /// Gets or sets the Partnerid.
        /// </summary>
        public string Partnerid { get; set; }

        /// <summary>
        /// Gets or sets the CreatedDate.
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the corporateportalRequests.
        /// </summary>
        public CorporateportalRequests corporateportalRequests { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the Ideas.
        /// </summary>
        public string Ideas { get; set; }

        /// <summary>
        /// Gets or sets the Issues.
        /// </summary>
        public string Issues { get; set; }

        /// <summary>
        /// Gets or sets the Efolderid.
        /// </summary>
        public string Efolderid { get; set; }

        /// <summary>
        /// Gets or sets the strupdatedMessages.
        /// </summary>
        public string strupdatedMessages { get; set; }

        /// <summary>
        /// Gets or sets the updatedMessages.
        /// </summary>
        public List<string> updatedMessages { get; set; }

        /// <summary>
        /// Gets or sets the strupdateddates.
        /// </summary>
        public string strupdateddates { get; set; }

        /// <summary>
        /// Gets or sets the updateddates.
        /// </summary>
        public List<string> updateddates { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="UpdatedMsg_ids_iss" />.
    /// </summary>
    public class UpdatedMsg_ids_iss
    {
        /// <summary>
        /// Gets or sets the UpdatedDate.
        /// </summary>
        public string UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedMsg.
        /// </summary>
        public string UpdatedMsg { get; set; }
    }

    /// <summary>
    /// Defines the CorporateportalRequests.
    /// </summary>
    public enum CorporateportalRequests
    {
        /// <summary>
        /// Defines the ComposeMessage.
        /// </summary>
        ComposeMessage,

        /// <summary>
        /// Defines the SubmitIdeas.
        /// </summary>
        SubmitIdeas,

        /// <summary>
        /// Defines the ReportIssues.
        /// </summary>
        ReportIssues
    }

    /// <summary>
    /// Defines the <see cref="CPHelpers" />.
    /// </summary>
    public static class CPHelpers
    {
        /// <summary>
        /// Defines the _service.
        /// </summary>
        //private static ISitecoreService _service;

        /// <summary>
        /// Gets the SCService.
        /// </summary>
        //private static ISitecoreService SCService => _service ?? (_service = new SitecoreService("web"));
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        /// <summary>
        /// The GetCPMeetingRequestTypes.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{SelectListItem}"/>.</returns>
        public static IEnumerable<SelectListItem> GetCPMeetingRequestTypes()
        {
            try
            {
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.CP_MEETINGTYPES));
                IEnumerable<SelectListItem> convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Owner Types");
            }
        }

        public static List<SelectListItem> GetFromToTime()
        {
            // Set the start time (00:00 means 12:00 AM)
            DateTime StartTime = DateTime.ParseExact("07:30", "HH:mm", null);
            // Set the end time (23:55 means 11:55 PM)
            DateTime EndTime = DateTime.ParseExact("14:30", "HH:mm", CultureInfo.InvariantCulture);
            //Set 5 minutes interval
            TimeSpan Interval = new TimeSpan(0, 30, 0);
            //To set 1 hour interval
            //TimeSpan Interval = new TimeSpan(1, 0, 0);
            List<SelectListItem> lists = new List<SelectListItem>();
            while (StartTime <= EndTime)
            {
                lists.Add(new SelectListItem { Text = StartTime.ToString("HH:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = StartTime.ToString("HH:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً") });
                StartTime = StartTime.Add(Interval);
            }
            return lists;
        }
    }
}