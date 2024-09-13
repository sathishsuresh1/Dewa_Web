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
    [StoredProcedure("SP_DRRG_InsertinverterModule")]
    public class Proc_DRRG_InsertInverterModule
    {
        /// <summary>
        /// Gets or sets the useremail.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.NVarChar, ParameterName = "useremail")]
        public string useremail { get; set; }

        /// <summary>
        /// Gets or sets the DRRG_Inverter.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Inverter")]
        public List<DRRG_Inverter_TY> DRRG_Inverter { get; set; }

        /// <summary>
        /// Gets or sets the DRRG_Inverter_RP.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Inverter_RP")]
        public List<DRRG_Inverter_RP_TY> DRRG_Inverter_RP { get; set; }

        /// <summary>
        /// Gets or sets the DRRG_Inverter_AP.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Inverter_AP")]
        public List<DRRG_Inverter_AP_TY> DRRG_Inverter_AP { get; set; }

        /// <summary>
        /// Gets or sets the DRRG_Inverter_AP.
        /// </summary>
        [StoredProcedureParameter(System.Data.SqlDbType.Udt, ParameterName = "DRRG_Inverter_MAP")]
        public List<DRRG_Inverter_MAP_TY> DRRG_Inverter_MAP { get; set; }

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
