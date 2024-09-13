// <copyright file="DRRGInverterModule.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="DRRGInverterModule" />
    /// </summary>
    public class DRRGInverterModule
    {
        /// <summary>
        /// Gets or sets the Efolderid
        /// </summary>
        public string Efolderid { get; set; }

        /// <summary>
        /// Gets or sets the Manufactureruserid
        /// </summary>
        public string Manufactureruserid { get; set; }

        /// <summary>
        /// Gets or sets the ManufacturerName
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Gets or sets the Reject_Remarks
        /// </summary>
        public string Reject_Remarks { get; set; }

        /// <summary>
        /// Gets or sets the ModelofInverterModule
        /// </summary>
        public string ModelofInverterModule { get; set; }

        /// <summary>
        /// Gets or sets the Application
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the PowerRange
        /// </summary>
        public string PowerRange { get; set; }

        /// <summary>
        /// Gets or sets the DegreeofProtectionIPoftheEnvelope
        /// </summary>
        public string DegreeofProtectionIPoftheEnvelope { get; set; }

        /// <summary>
        /// Gets or sets the InternalInterfaceProtection
        /// </summary>
        public string InternalInterfaceProtection { get; set; }

        /// <summary>
        /// Gets or sets the MasterslaveFeature
        /// </summary>
        public string MasterslaveFeature { get; set; }

        /// <summary>
        /// Gets or sets the FunctionofStringParallel
        /// </summary>
        public string FunctionofStringParallel { get; set; }

        /// <summary>
        /// Gets or sets the OtherFunctionofStringParallel
        /// </summary>
        public string OtherFunctionofStringParallel { get; set; }

        /// <summary>
        /// Gets or sets the MultipleMPPTSections
        /// </summary>
        public string MultipleMPPTSections { get; set; }

        /// <summary>
        /// Gets or sets the OtherMultipleMPPTSections
        /// </summary>
        public string OtherMultipleMPPTSections { get; set; }

        /// <summary>
        /// Gets or sets the DCandACSectionsSeparatedbyanInternalTransformer
        /// </summary>
        public string DCandACSectionsSeparatedbyanInternalTransformer { get; set; }

        /// <summary>
        /// Gets or sets the PowerDerating
        /// </summary>
        public string PowerDerating { get; set; }

        /// <summary>
        /// Gets or sets the CategoryType
        /// </summary>
        public string CategoryType { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC621091
        /// </summary>
        public string objidIEC621091 { get; set; }

        /// <summary>
        /// Gets or sets the IEC621091
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC621091 { get; set; }

        /// <summary>
        /// Gets or sets the IEC621091Date
        /// </summary>
        public string IEC621091Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC621092
        /// </summary>
        public string objidIEC621092 { get; set; }

        /// <summary>
        /// Gets or sets the IEC621092
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC621092 { get; set; }

        /// <summary>
        /// Gets or sets the IEC621092Date
        /// </summary>
        public string IEC621092Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC1741
        /// </summary>
        public string objidIEC1741 { get; set; }

        /// <summary>
        /// Gets or sets the IEC1741
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC1741 { get; set; }

        /// <summary>
        /// Gets or sets the IEC1741Date
        /// </summary>
        public string IEC1741Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC6100032
        /// </summary>
        public string objidIEC6100032 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100032
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC6100032 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100032Date
        /// </summary>
        public string IEC6100032Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61000312
        /// </summary>
        public string objidIEC61000312 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61000312
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61000312 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61000312Date
        /// </summary>
        public string IEC61000312Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC6100061
        /// </summary>
        public string objidIEC6100061 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100061
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC6100061 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100061Date
        /// </summary>
        public string IEC6100061Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC6100062
        /// </summary>
        public string objidIEC6100062 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100062
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC6100062 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100062Date
        /// </summary>
        public string IEC6100062Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC6100063
        /// </summary>
        public string objidIEC6100063 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100063
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC6100063 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100063Date
        /// </summary>
        public string IEC6100063Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC6100064
        /// </summary>
        public string objidIEC6100064 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100064
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC6100064 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6100064Date
        /// </summary>
        public string IEC6100064Date { get; set; }

        /// <summary>
        /// Gets or sets the objiddewadrrg
        /// </summary>
        public string objiddewadrrg { get; set; }

        /// <summary>
        /// Gets or sets the dewadrrg
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase dewadrrg { get; set; }

        /// <summary>
        /// Gets or sets the dewadrrgDate
        /// </summary>
        public string dewadrrgDate { get; set; }

        /// <summary>
        /// Gets or sets the objidundertaking
        /// </summary>
        public string objidundertaking { get; set; }

        /// <summary>
        /// Gets or sets the inverterundertaking
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase inverterundertaking { get; set; }
    }
}
