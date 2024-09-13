// <copyright file="InfrastructureNocModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj.Rathva</author>

using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Integration.Responses.SmartCustomer;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Infrastructure_Noc
{
    public class InfrastructureNocReqModel
    {
        public SharedAccount selectedAccounts { get; set; }
        public string TransactionId { get; set; }
        public string BusinessPartner { get; set; }
        public string ContractAccount { get; set; }
        public string SelectedWorkType { get; set; }
        public string DescProposedWorkYype { get; set; }
        public string CustomerNotes { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string SubmittedDate { get; set; }
        public string Revision { get; set; }
        public HttpPostedFileBase Copy_AffectionPlan { get; set; }
        public HttpPostedFileBase Cover_Letter { get; set; }
        public HttpPostedFileBase ProposedWorked_Sketch { get; set; }
        public List<SelectListItem> WorkTypeList { get; set; }
        public string PlotNumber { get; set; }
        public List<NocRequestAttachments> nocReqAttachments { get; set; }
        public List<NocRequestAttachments> nocReqDewaAttachments { get; set; }
        public string InteractionHistory { get; set; }
        public List<Status> statusList { get; set; }
    }
  
    public class NocRequestAttachments
    {
        public string DocDate { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileId { get; set; }
        public string FileContent { get; set; }
        public string DocType { get; set; }
        public string Folder { get; set; }
        public string FileSize { get; set; }
    }
}