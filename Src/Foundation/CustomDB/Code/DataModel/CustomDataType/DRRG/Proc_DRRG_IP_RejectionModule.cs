// <copyright file="Proc_DRRG_RejectionModule.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Proc_DRRG_IP_RejectionModule" />.
    /// </summary>
    [StoredProcedure("SP_DRRG_PVInterfaceRejection")]
    public class Proc_DRRG_IP_RejectionModule
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
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "pvcode")]
        public string pvcode { get; set; }

        /// <summary>
        /// Gets or sets the dRRG_Files_TY.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Files")]
        public List<DRRG_Files_TY> dRRG_Files_TY { get; set; }
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "pvusername", Direction = System.Data.ParameterDirection.Output)]
        public string pvusername { get; set; }

        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "pvuseremail", Direction = System.Data.ParameterDirection.Output)]
        public string pvuseremail { get; set; }
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "pvmodelname", Direction = System.Data.ParameterDirection.Output)]
        public string pvmodelname { get; set; }
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "error", Direction = System.Data.ParameterDirection.Output)]
        public string error { get; set; }
    }
}
