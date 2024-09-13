// <copyright file="CSVfileformat.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.WorkPermit
{
    using DEWAXP.Feature.GatePass.Models.ePass;
    using FileHelpers;
    using System;

    /// <summary>
    /// Defines the <see cref="CSVfileformat" />.
    /// </summary>
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class WorkPermitCSVfileformat
    {
        /// <summary>
        /// Defines the ID.
        /// </summary>
        [FieldHidden]
        public int ID;

        /// <summary>
        /// Defines the CustomerName.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(1)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        //[FieldNullValue(typeof(string), "nobody")]
        [FieldConverter(typeof(CustomFieldNotemptyConverter), "Customer Name")]
        public string CustomerName;

        /// <summary>
        /// Defines the Profession.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(2)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(CustomFieldNotemptyConverter), "Profession")]
        [FieldTrim(TrimMode.Both)]
        public string Profession;

        /// <summary>
        /// Defines the Phonenumber.
        /// </summary>
        [FieldOrder(3)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(UAEPhonenumberConverter))]
        [FieldTrim(TrimMode.Both)]
        public string Phonenumber;

        /// <summary>
        /// Defines the Emailid.
        /// </summary>
        [FieldOrder(4)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(EmailConverter))]
        [FieldTrim(TrimMode.Both)]
        public string Emailid;

        /// <summary>
        /// Defines the EmiratesID.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(5)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(EmiratesIDConverter))]
        [FieldTrim(TrimMode.Both)]
        public string EmiratesID;

        /// <summary>
        /// Defines the EidDate.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(6)]
        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(ExpiryDateConverter), "Emirates ID Expiry date should be 15 days greater than current date", "")]
        public DateTime? EidDate;

        /// <summary>
        /// Defines the Visanumber.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(7)]
        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Visanumber;

        /// <summary>
        /// Defines the VisaexpDate.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(8)]
        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(ExpiryDateConverter), "Visa Expiry date should be 15 days greater than current date", "")]
        public DateTime? VisaexpDate;

        /// <summary>
        /// Defines the Passportnumber.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(9)]
        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Passportnumber;

        /// <summary>
        /// Defines the PassportexpDate.
        /// </summary>
        [FieldNotEmpty]
        [FieldOrder(10)]
        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldConverter(typeof(ExpiryDateConverter), "Passport Expiry date should be 15 days greater than current date", "")]
        public DateTime? PassportexpDate;

        ///// <summary>
        ///// Defines the Purpose.
        ///// </summary>
        //[FieldNotEmpty]
        //[FieldConverter(typeof(CustomFieldNotemptyConverter), "Purpose")]
        //[FieldTrim(TrimMode.Both)]
        //public string Purpose;

        /// <summary>
        /// Defines the registeredefolderid.
        /// </summary>
        [FieldHidden]
        public string registeredefolderid;

        /// <summary>
        /// Defines the grouppassid.
        /// </summary>
        [FieldHidden]
        public string grouppassid;
        [FieldHidden]
        public string Nationality;
    }
}
