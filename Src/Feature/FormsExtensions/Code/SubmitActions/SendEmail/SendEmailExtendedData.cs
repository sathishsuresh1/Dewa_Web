﻿using System;
using System.Collections.Generic;
using Sitecore.EmailCampaign.Cd.Actions;

namespace DEWAXP.Feature.FormsExtensions.SubmitActions.SendEmail
{
    public class SendEmailExtendedData : SendEmailData
    {
        public string Type { get; set; }
        public Guid? FieldEmailAddressId { get; set; }
        public bool UpdateCurrentContact { get; set; }
        public string FixedEmailAddress { get; set; }
        public IList<Guid> FileUploadFieldsToAttach { get; set; }
        public bool GenerateAllFieldsToken { get; set; }
        public string EmailFieldInDynamicDatasource { get; set; }
    }
}