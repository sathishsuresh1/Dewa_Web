// <copyright file="Proc_DRRG_InsertManufacturer.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Proc_DRRG_InsertManufacturer" />.
    /// </summary>
    [StoredProcedure("SP_DRRG_InsertManufacturer")]
    public class Proc_DRRG_InsertManufacturer
    {
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "useremail")]
        public string useremail { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Manufacturer_Details_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Manufacturer_Details")]
        public List<DRRG_Manufacturer_Details_TY> dRRG_Manufacturer_Details_TY { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Factory_Details_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Factory_Details")]
        public List<DRRG_Factory_Details_TY> dRRG_Factory_Details_TY { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Files_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Files")]
        public List<DRRG_Files_TY> dRRG_Files_TY { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "error", Direction = System.Data.ParameterDirection.Output)]
        public string error { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Proc_DRRG_UpdateManufacturer" />.
    /// </summary>
    [StoredProcedure("SP_DRRG_UpdateManufacturer")]
    public class Proc_DRRG_UpdateManufacturer
    {
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "useremail")]
        public string useremail { get; set; }

        /// <summary>
        /// Gets or sets the manufacturecode.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "manufacturecode")]
        public string manufacturecode { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Manufacturer_Details_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Manufacturer_Details")]
        public List<DRRG_Manufacturer_Details_TY> dRRG_Manufacturer_Details_TY { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Factory_Details_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Factory_Details")]
        public List<DRRG_Factory_Details_TY> dRRG_Factory_Details_TY { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Files_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Files")]
        public List<DRRG_Files_TY> dRRG_Files_TY { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "error", Direction = System.Data.ParameterDirection.Output)]
        public string error { get; set; }
    }
}
