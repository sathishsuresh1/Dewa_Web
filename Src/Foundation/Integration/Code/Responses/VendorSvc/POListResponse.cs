// <copyright file="GetPOListResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.VendorSvc
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="ITEM" />.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "ITEM")]
    public class POITEM
    {
        /// <summary>
        /// Gets or sets the LineNumber.
        /// </summary>
        [XmlElement(ElementName = "Po_No")]
        public string Po_No { get; set; }

        /// <summary>
        /// Gets or sets the TenderNumber.
        /// </summary>
        [XmlElement(ElementName = "Vendor_Id")]
        public string Vendor_Id { get; set; }

        /// <summary>
        /// Gets or sets the TenderDescription.
        /// </summary>
        [XmlElement(ElementName = "PO_desc")]
        public string PO_desc { get; set; }

        /// <summary>
        /// Gets or sets the FloatingDate.
        /// </summary>
        [XmlElement(ElementName = "Totamt")]
        public string Totamt { get; set; }

        /// <summary>
        /// Gets or sets the ClosingDate.
        /// </summary>
        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PO_DISPLAY" />.
    /// </summary>
    [XmlRoot(ElementName = "PO_DISPLAY")]
    public class PO_DISPLAY
    {
        /// <summary>
        /// Gets or sets the Tender.
        /// </summary>
        [XmlElement(ElementName = "ITEM")]
        public List<POITEM> POITEMS { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="GetPOListResponse" />.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "PoDisplayResponse")]
    public class POListResponse
    {
        /// <summary>
        /// Gets or sets the PO_DISPLAY.
        /// </summary>
        [XmlElement(ElementName = "PO_DISPLAY")]
        public PO_DISPLAY PO_DISPLAY { get; set; }

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
    }
}
