// <copyright file="DRRG_PVModulenominal_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_PVModulenominal_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_PVModulenominal_TY")]
    public class DRRG_PVModulenominal_TY
    {
        /// <summary>
        /// Gets or sets the PVID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string PVID { get; set; }

        /// <summary>
        /// Gets or sets the wp1.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string wp1 { get; set; }

        /// <summary>
        /// Gets or sets the wp2.
        /// </summary>
        [UserDefinedTableTypeColumn(3)]
        public string wp2 { get; set; }
        /// <summary>
        /// Gets or sets the wp3.
        /// </summary>
        [UserDefinedTableTypeColumn(4)]
        public string wp3 { get; set; }
        /// <summary>
        /// Gets or sets the mpv1.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string mpv1 { get; set; }
        /// <summary>
        /// Gets or sets the wp3.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string mpc1 { get; set; }
        /// <summary>
        /// Gets or sets the ocv1.
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string ocv1 { get; set; }
        /// <summary>
        /// Gets or sets the tci1.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string scc1 { get; set; }
        /// <summary>
        /// Gets or sets the tci1.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string tci1 { get; set; }
        /// <summary>
        /// Gets or sets the tcv1.
        /// </summary>
        [UserDefinedTableTypeColumn(10)]
        public string tcv1 { get; set; }
        /// <summary>
        /// Gets or sets the noct1.
        /// </summary>
        [UserDefinedTableTypeColumn(11)]
        public string noct1 { get; set; }
        /// <summary>
        /// Gets or sets the npnoct1.
        /// </summary>
        [UserDefinedTableTypeColumn(12)]
        public string npnoct1 { get; set; }
    }
}
