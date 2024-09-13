// <copyright file="ManageAccountInfo.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    using DEWAXP.Foundation.Integration.Responses;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="ManageAccountInfo" />.
    /// </summary>
    [Serializable]
    public class ManageAccountInfo
    {
        /// <summary>
        /// Gets or sets the AccountNumberSelected.
        /// </summary>
        public string AccountNumberSelected { get; set; }

        /// <summary>
        /// Gets or sets the MultipleAccountNumberSelected.
        /// </summary>
        public string MultipleAccountNumberSelected { get; set; }

        /// <summary>
        /// Gets or sets the SelectedBusinessPartnerNumber.
        /// </summary>
        public string SelectedBusinessPartnerNumber { get; set; }

        /// <summary>
        /// Gets or sets the NickName.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the MobileNumber.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the Street.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the XCordinate.
        /// </summary>
        public string XCordinate { get; set; }

        /// <summary>
        /// Gets or sets the YCordinate.
        /// </summary>
        public string YCordinate { get; set; }

        /// <summary>
        /// Gets or sets the PremiseNumber.
        /// </summary>
        public string PremiseNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPrimaryAccount.
        /// </summary>
        public bool IsPrimaryAccount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsVerifyEmail.
        /// </summary>
        public bool IsVerifyEmail { get; set; }

        /// <summary>
        /// Gets or sets the CommunicationList.
        /// </summary>
        public List<ManageAccountCommunication> CommunicationList { get; set; }

        /// <summary>
        /// Gets or sets the Accounts.
        /// </summary>
        public AccountDetails[] Accounts { get; set; }

        /// <summary>
        /// Gets or sets the ProfilePictureUploader.
        /// </summary>
        public HttpPostedFileBase ProfilePictureUploader { get; set; }

        /// <summary>
        /// Gets or sets the ProfilePictureBytes.
        /// </summary>
        public byte[] ProfilePictureBytes { get; set; }

        /// <summary>
        /// Gets or sets the successModel.
        /// </summary>
        public UpdateContactInfoSuccessModel successModel { get; set; }

        public string OtpRequestId { get; set; }
        public string RequestType { get; set; }      
    }

    /// <summary>
    /// Defines the <see cref="ManageAccountCommunication" />.
    /// </summary>
    [Serializable]
    public class ManageAccountCommunication
    {
        /// <summary>
        /// Gets or sets the CommunicationType.
        /// </summary>
        public string CommunicationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsEmail.
        /// </summary>
        public bool IsEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSMS.
        /// </summary>
        public bool IsSMS { get; set; }
    }

    public class ModalaccountList
    {
        public AccountDetails[] Accounts { get; set; }

        public string BusinessPartnerNumber { get; set; }
    }
}
