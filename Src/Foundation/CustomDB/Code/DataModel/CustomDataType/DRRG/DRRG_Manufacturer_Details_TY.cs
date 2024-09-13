// <copyright file="DRRG_Manufacturer_Details_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_Manufacturer_Details_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Manufacturer_Details_TY")]
    public class DRRG_Manufacturer_Details_TY
    {
        /// <summary>
        /// Gets or sets the Manufacturer_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string Manufacturer_Name { get; set; }

        /// <summary>
        /// Gets or sets the Brand_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Brand_Name { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturer_Country.
        /// </summary>
        [UserDefinedTableTypeColumn(3)]
        public string Manufacturer_Country { get; set; }

        /// <summary>
        /// Gets or sets the Corporate_Email.
        /// </summary>
        [UserDefinedTableTypeColumn(4)]
        public string Corporate_Email { get; set; }

        /// <summary>
        /// Gets or sets the Corporate_Phone_Number.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string Corporate_Phone_Number { get; set; }

        /// <summary>
        /// Gets or sets the Corporate_Phone_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string Corporate_Phone_Code { get; set; }

        /// <summary>
        /// Gets or sets the Corporate_Fax_Number.
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string Corporate_Fax_Number { get; set; }

        /// <summary>
        /// Gets or sets the Corporate_Fax_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string Corporate_Fax_Code { get; set; }

        /// <summary>
        /// Gets or sets the Website.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Local_Representative.
        /// </summary>
        [UserDefinedTableTypeColumn(10)]
        public bool Local_Representative { get; set; }

        /// <summary>
        /// Gets or sets the Company_Full_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(11)]
        public string Company_Full_Name { get; set; }

        /// <summary>
        /// Gets or sets the User_First_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(12)]
        public string User_First_Name { get; set; }

        /// <summary>
        /// Gets or sets the User_Last_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(13)]
        public string User_Last_Name { get; set; }

        /// <summary>
        /// Gets or sets the User_Gender.
        /// </summary>
        [UserDefinedTableTypeColumn(14)]
        public string User_Gender { get; set; }

        /// <summary>
        /// Gets or sets the User_Nationality.
        /// </summary>
        [UserDefinedTableTypeColumn(15)]
        public string User_Nationality { get; set; }

        /// <summary>
        /// Gets or sets the User_Designation.
        /// </summary>
        [UserDefinedTableTypeColumn(16)]
        public string User_Designation { get; set; }

        /// <summary>
        /// Gets or sets the User_Mobile_Number.
        /// </summary>
        [UserDefinedTableTypeColumn(17)]
        public string User_Mobile_Number { get; set; }

        /// <summary>
        /// Gets or sets the User_Mobile_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(18)]
        public string User_Mobile_Code { get; set; }

        /// <summary>
        /// Gets or sets the User_Email_Address.
        /// </summary>
        [UserDefinedTableTypeColumn(19)]
        public string User_Email_Address { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturer_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(20)]
        public string Manufacturer_Code { get; set; }
    }
}
