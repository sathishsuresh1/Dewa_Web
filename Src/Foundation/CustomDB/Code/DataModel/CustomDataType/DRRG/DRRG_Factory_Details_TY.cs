// <copyright file="DRRG_Factory_Details_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_Factory_Details_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Factory_Details_TY")]
    public class DRRG_Factory_Details_TY
    {
        /// <summary>
        /// Gets or sets the Factory_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string Factory_Name { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        [UserDefinedTableTypeColumn(3)]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the EOL_PV_Module.
        /// </summary>
        [UserDefinedTableTypeColumn(4)]
        public string EOL_PV_Module { get; set; }

        /// <summary>
        /// Gets or sets the Factory_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string Factory_Code { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturer_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string Manufacturer_Code { get; set; }
    }
}
