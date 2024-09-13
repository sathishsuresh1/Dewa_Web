// <copyright file="drrgpvmodule.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using DEWAXP.Foundation.DataAnnotations;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="DRRGPVModule" />
    /// </summary>
    public class DRRGPVModule
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
        /// Gets or sets the ModelofPVModule
        /// </summary>
        public string ModelofPVModule { get; set; }

        /// <summary>
        /// Gets or sets the PeakPower
        /// </summary>
        public string PeakPower { get; set; }

        /// <summary>
        /// Gets or sets the GrossSurface
        /// </summary>
        public string GrossSurface { get; set; }

        /// <summary>
        /// Gets or sets the CellTechnology
        /// </summary>
        public string CellTechnology { get; set; }

        /// <summary>
        /// Gets or sets the OtherCellTechnology
        /// </summary>
        public string OtherCellTechnology { get; set; }

        /// <summary>
        /// Gets or sets the Framed
        /// </summary>
        public string Framed { get; set; }

        /// <summary>
        /// Gets or sets the FrontSuperstrate
        /// </summary>
        public string FrontSuperstrate { get; set; }

        /// <summary>
        /// Gets or sets the OtherFrontSuperstrate
        /// </summary>
        public string OtherFrontSuperstrate { get; set; }

        /// <summary>
        /// Gets or sets the BackSuperstrate
        /// </summary>
        public string BackSuperstrate { get; set; }

        /// <summary>
        /// Gets or sets the OtherBackSuperstrate
        /// </summary>
        public string OtherBackSuperstrate { get; set; }

        /// <summary>
        /// Gets or sets the Encapsulant
        /// </summary>
        public string Encapsulant { get; set; }

        /// <summary>
        /// Gets or sets the OtherEncapsulant
        /// </summary>
        public string OtherEncapsulant { get; set; }

        /// <summary>
        /// Gets or sets the DCSystemGrounding
        /// </summary>
        public string DCSystemGrounding { get; set; }

        /// <summary>
        /// Gets or sets the PositionoftheJunctionBox
        /// </summary>
        public string PositionoftheJunctionBox { get; set; }

        /// <summary>
        /// Gets or sets the MaterialoftheJunctionBoxEnclosure
        /// </summary>
        public string MaterialoftheJunctionBoxEnclosure { get; set; }

        /// <summary>
        /// Gets or sets the OtherMaterialoftheJunctionBoxEnclosure
        /// </summary>
        public string OtherMaterialoftheJunctionBoxEnclosure { get; set; }

        /// <summary>
        /// Gets or sets the Terminations
        /// </summary>
        public string Terminations { get; set; }

        /// <summary>
        /// Gets or sets the OtherTerminations
        /// </summary>
        public string OtherTerminations { get; set; }

        /// <summary>
        /// Gets or sets the FurtherFeaturesoftheJunctionBox
        /// </summary>
        public string FurtherFeaturesoftheJunctionBox { get; set; }

        /// <summary>
        /// Gets or sets the OtherFurtherFeaturesoftheJunctionBox
        /// </summary>
        public string OtherFurtherFeaturesoftheJunctionBox { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61215
        /// </summary>
        public string objidIEC61215 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61215
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61215 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61215orIEC61646Date
        /// </summary>
        public string IEC61215orIEC61646Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61646
        /// </summary>
        public string objidIEC61646 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61646
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61646 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61646Date
        /// </summary>
        public string IEC61646Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61730
        /// </summary>
        public string objidIEC61730 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61730
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61730 { get; set; }

        /// <summary>
        /// Gets or sets the IEC6173012Date
        /// </summary>
        public string IEC6173012Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61701
        /// </summary>
        public string objidIEC61701 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61701
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61701 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61701Date
        /// </summary>
        public string IEC61701Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC62716
        /// </summary>
        public string objidIEC62716 { get; set; }

        /// <summary>
        /// Gets or sets the IEC62716
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC62716 { get; set; }

        /// <summary>
        /// Gets or sets the IEC62716Date
        /// </summary>
        public string IEC62716Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC61345
        /// </summary>
        public string objidIEC61345 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61345
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC61345 { get; set; }

        /// <summary>
        /// Gets or sets the IEC61345Date
        /// </summary>
        public string IEC61345Date { get; set; }

        /// <summary>
        /// Gets or sets the objidIEC60068
        /// </summary>
        public string objidIEC60068 { get; set; }

        /// <summary>
        /// Gets or sets the IEC60068
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IEC60068 { get; set; }

        /// <summary>
        /// Gets or sets the IEC60068268Date
        /// </summary>
        public string IEC60068268Date { get; set; }

        /// <summary>
        /// Gets or sets the objidmodeldatasheet
        /// </summary>
        public string objidmodeldatasheet { get; set; }

        /// <summary>
        /// Gets or sets the ModelDataSheet
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase ModelDataSheet { get; set; }

        /// <summary>
        /// Gets or sets the objidundertaking
        /// </summary>
        public string objidundertaking { get; set; }

        /// <summary>
        /// Gets or sets the undertaking
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase undertaking { get; set; }
    }
}
