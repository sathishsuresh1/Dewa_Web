// <copyright file="TrackApplicationModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>

using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.ShamsDubai.Models.ShamsDubaiTraining
{
    [Serializable]
    /// <summary>
    /// Defines the <see cref="ApplicationTrackingModel" />.
    /// </summary>
    public class ApplicationTrackingModel
    {
        public string RequestNumber { get; set; }
        public string TradeLicenceNumber { get; set; }
        public string EmiratesID { get; set; }
        
        public List<ApplicationTrackResult> AppTrackResultList { get; set; }
    }

    public class ApplicationTrackResult
    {
        public string Applicationname { get; set; }
        public string Certificate { get; set; }
        public string Certificatenumber { get; set; }
        public string CompanycontactPerson { get; set; }
        public string Companydescription { get; set; }
        public string Companyemail { get; set; }
        public string Companymobile { get; set; }
        public string Companyname { get; set; }
        public string Countryname { get; set; }
        public string Departmenttext { get; set; }
        public string Description { get; set; }
        public string Designation { get; set; }
        public string Designesexp { get; set; }
        public string Designpvexp { get; set; }
        public string Emailaddress { get; set; }
        public string EmiratesId { get; set; }
        public string Enddate { get; set; }
        public string Licenseexpirydate { get; set; }
        public string Licenseissuedate { get; set; }
        public string Mobilenumber { get; set; }
        public string Passportexpirydate { get; set; }
        public string Passportissuedate { get; set; }
        public string Passportnumber { get; set; }
        public string Reasonforenroll { get; set; }
        public string Responsecode { get; set; }
        public string Shamsexp { get; set; }
        public string Startdate { get; set; }
        public string Tradelicense { get; set; }
        public string Trainingduration { get; set; }
        public string TrainingId { get; set; }
        public string Trainingname { get; set; }
        public string Vatnumber { get; set; }
        public string Visaissuedate { get; set; }
        public string Visanumber { get; set; }
        public string Visavaliditydate { get; set; }
        public string Applicationstatus { get; set; }
    }
}
