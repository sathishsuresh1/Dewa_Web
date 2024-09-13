// <copyright file="ContractAccountImage.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="ContractAccountImage" />.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "ContractAccountImage")]
    public class ContractAccountImage
    {
        /// <summary>
        /// Gets or sets the UserID.
        /// </summary>
        [XmlElement(ElementName = "UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the ResponseCode.
        /// </summary>
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ContractAccount.
        /// </summary>
        [XmlElement(ElementName = "ContractAccount")]
        public string ContractAccount { get; set; }
    }
}
