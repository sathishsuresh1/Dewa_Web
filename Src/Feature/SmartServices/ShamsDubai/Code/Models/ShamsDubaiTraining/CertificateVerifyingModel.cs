// <copyright file="CertificateVerifyingModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>

using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.ShamsDubai.Models.ShamsDubaiTraining
{
    [Serializable]
    /// <summary>
    /// Defines the <see cref="CertificateVerifyingModel" />.
    /// </summary>
    public class CertificateVerifyingModel
    {
        public string CertificateNumber { get; set; }
        public string EmiratesID { get; set; }
        
        public List<CertificateVerifyResult> CertiVerifyResultList { get; set; }
    }

    public class CertificateVerifyResult
    {
        public string Applicantname { get; set; }
        public string Companyname { get; set; }
        public string Certificatecategory { get; set; }
        public string Certificatecontent { get; set; }
        public string Certificateexpirydate { get; set; }
        public string Certificateissued { get; set; }
        public string Certificateissuedate { get; set; }
        public string Certificatenumber { get; set; }
        public string Certificatestatus { get; set; }
        public string Description { get; set; }
        public string Emiratesid { get; set; }
        public string Enddate { get; set; }
        public string Requestnumber { get; set; }
        public string Responsecode { get; set; }
        public string Startdate { get; set; }
        public string Trainingid { get; set; }
    }
}
