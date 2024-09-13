// <copyright file="InboxViewModel.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Partner.Models.CorporatePartnership
{
    using DEWAXP.Foundation.Integration.CorporatePortal;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="InboxViewModel" />.
    /// </summary>
    public class InboxViewModel
    {
        /// <summary>
        /// Gets or sets the taskDetailsExternalResponse.
        /// </summary>
        public taskDetailsExternalResponse taskDetailsExternalResponse { get; set; }

        /// <summary>
        /// Gets or sets the sentAndPipeLineTaskResponse.
        /// </summary>
        public List<CP_MSG_IDS_ISS> sentAndPipeLineTaskResponse { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="InboxListViewModel" />.
    /// </summary>
    public class InboxListViewModel
    {
        /// <summary>
        /// Gets or sets the inboxmessage.
        /// </summary>
        public string inboxmessage { get; set; }

        /// <summary>
        /// Gets or sets the totalpage.
        /// </summary>
        public int totalpage { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pagination.
        /// </summary>
        public bool pagination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether receivedmessage.
        /// </summary>
        public bool receivedmessage { get; set; }

        /// <summary>
        /// Gets or sets the receivedorsent.
        /// </summary>
        public string receivedorsent { get; set; }

        /// <summary>
        /// Gets or sets the pagenumbers.
        /// </summary>
        public IEnumerable<int> pagenumbers { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public string keywords { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="InboxMessage" />.
    /// </summary>
    public class InboxMessage
    {
        /// <summary>
        /// Gets or sets the messageurl.
        /// </summary>
        public string messageurl { get; set; }

        /// <summary>
        /// Gets or sets the messagereadornot.
        /// </summary>
        public string messagereadornot { get; set; }

        /// <summary>
        /// Gets or sets the messagesubject.
        /// </summary>
        public string messagesubject { get; set; }

        /// <summary>
        /// Gets or sets the messagetitle.
        /// </summary>
        public string messagetitle { get; set; }

        /// <summary>
        /// Gets or sets the messageobjectname.
        /// </summary>
        public string messageobjectname { get; set; }

        /// <summary>
        /// Gets or sets the messagerequestername.
        /// </summary>
        public string messagerequestername { get; set; }

        /// <summary>
        /// Gets or sets the messagedetail.
        /// </summary>
        public string messagedetail { get; set; }

        /// <summary>
        /// Gets or sets the messagedate.
        /// </summary>
        public string messagedate { get; set; }

        /// <summary>
        /// Gets or sets the messagestatus.
        /// </summary>
        public string messagestatus { get; set; }
    }
}