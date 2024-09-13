// <copyright file="Proc_DRRG_RejectionModule.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Proc_DRRG_RejectionModule" />.
    /// </summary>
    [StoredProcedure("SP_DRRG_ManufacturerRejection")]
    public class Proc_DRRG_RejectionModule
    {
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "useremail")]
        public string useremail { get; set; }

        /// <summary>
        /// Gets or sets the rejectedreason.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "rejectedreason")]
        public string rejectedreason { get; set; }

        /// <summary>
        /// Gets or sets the rejectedfileid.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "rejectedfileid")]
        public string rejectedfileid { get; set; }

        /// <summary>
        /// Gets or sets the manufacturecode.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "manufacturecode")]
        public string manufacturecode { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Files_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Files")]
        public List<DRRG_Files_TY> dRRG_Files_TY { get; set; }

        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "manufacturername", Direction = System.Data.ParameterDirection.Output)]
        public string manufacturername { get; set; }

        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "manufactureremail", Direction = System.Data.ParameterDirection.Output)]
        public string manufactureremail { get; set; }
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "error", Direction = System.Data.ParameterDirection.Output)]
        public string error { get; set; }
    }
}
