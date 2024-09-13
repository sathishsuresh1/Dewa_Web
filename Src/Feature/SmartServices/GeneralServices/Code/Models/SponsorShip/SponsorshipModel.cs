// <copyright file="SponsorshipModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Sponsorship
{
    [Serializable]
    /// <summary>
    /// Defines the <see cref="SponsorshipModel" />.
    /// </summary>
    public class SponsorshipModel
    {
        public SponsorshipModel()
        {
            eventInformation = new EventInformation();
            eventParticipants = new EventParticipants();
            otherDetails = new OtherDetails();
            sponsorshipDetails = new SponsorshipDetails();
        }
        public string RequestID { get; set; }
        public string PersonName { get; set; }
        public string PersonEmail { get; set; }
        public string PersonContact { get; set; }
        public bool IsSuccess { get; set; }
        public EventInformation eventInformation { get; set; }
        public EventParticipants eventParticipants { get; set; }
        public OtherDetails otherDetails { get; set; }
        public SponsorshipDetails sponsorshipDetails { get; set; }
    }
    // Step 1. Event General Information
    public class EventInformation
    {
        public string Eventname { get; set; }
        public string Companyname { get; set; }
        public string Eventofdate { get; set; }
        public string Eventvenue { get; set; }
        public string Eventshortdescr { get; set; }
        public string Eventgoals { get; set; }
        public string Eventgoalsjs { get; set; }
        public string Eventlettertoceo { get; set; }

        // Event Trade License
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Event_tradelicenseattach { get; set; }
        public byte[] Event_tradelicenseattach_FileBinary { get; set; }
        public string Event_tradelicenseattach_FileType { get; set; }
    }

    // Step 2. Event Invitees/Participants
    public class EventParticipants
    {
        public string Eventtargetaudience { get; set; }
        public string Eventbeneficiaries { get; set; }
        public string Eventbeneficiariesjs { get; set; }
        public string Eventnoofbeneficiaries { get; set; }
        public string EventinvitedVIP { get; set; }
        public string EventinvitedVIPjs { get; set; }
        public string Eventnoofattend { get; set; }
        public string Eventnoofvolunteers { get; set; }
        public string Eventnoofcommittee { get; set; }
        public string Eventnoofdewaemployee { get; set; }
        public List<SelectListItem> EventTargetAudienceList { get; set; }

    }
    // Step 3. Other Details
    public class OtherDetails
    {
        public string Eventmediacoverage { get; set; }
        public string Eventnoofsocialmedia { get; set; }
        public string Eventcurrentsponsor { get; set; }
        public string Eventprevioussponsor { get; set; }
        public string Eventmanagesponsor { get; set; }
        public string Eventbenefits { get; set; }

        // Benefits Document
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Event_benefitsattach { get; set; }
        public byte[] Event_benefitsattach_FileBinary { get; set; }
        public string Event_benefitsattach_FileType { get; set; }
    }
    // Step 4. Sponsorship Details
    public class SponsorshipDetails
    {
        public string Eventsponsorshipduration { get; set; }
        public string Eventsponsorshipbefore { get; set; }
        public string Eventtotalyears { get; set; }
        public string Eventinvestmentdewa { get; set; }
        public string Eventtypeofmedia { get; set; }
        public string Eventcostofmedia { get; set; }
        public bool Eventcommunication { get; set; }
        public bool Eventlocalmedia { get; set; }
        public bool Eventuseofdewalogo { get; set; }
        public bool Eventopportunityfordewa { get; set; }
        public bool Eventopportunitytoreview { get; set; }
        public List<SelectListItem> EventSponsorshipDurationList { get; set; }
        public List<SelectListItem> EventsponsorshipbeforeList { get; set; }

        // Letter of Sponsorship Request
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Event_letterofsponsorattach { get; set; }
        public byte[] Event_letterofsponsorattach_FileBinary { get; set; }
        public string Event_letterofsponsorattach_FileType { get; set; }

        // Supporting Event Document
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Event_supporteventdocument { get; set; }
        public byte[] Event_supporteventdocument_FileBinary { get; set; }
        public string Event_supporteventdocument_FileType { get; set; }
    }
}
