// <copyright file="Proc_DRRG_InsertPVModule.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Proc_DRRG_InsertPVModule" />.
    /// </summary>
    [StoredProcedure("SP_DRRG_InsertinterfaceModule")]
    public class Proc_DRRG_InsertInterfaceModule
    {
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Int, ParameterName = "updateId")]
        public long updateId { get; set; }
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "useremail")]
        public string useremail { get; set; }

        /// <summary>
        /// Gets or sets the DRRG_Interface.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Interface")]
        public List<DRRG_Interface_TY> DRRG_Interface { get; set; }

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
