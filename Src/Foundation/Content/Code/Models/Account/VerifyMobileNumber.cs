// <copyright file="ManageAccountInfo.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    using System;
    using System.Collections.Generic;
    using System.Web;


    [Serializable]
    public class VerifyMobileNumber
    {
        public string Requestnumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessPartner { get; set; }
        public CustomerProfileSuccessURL SuccessURL { get; set; }
        public string URL { get; set; }
        public string RequestType { get; set; }    
    }
    public enum CustomerProfileSuccessURL
    {
       Customer_Profile,
       Manage_Account_Information
    }
}
