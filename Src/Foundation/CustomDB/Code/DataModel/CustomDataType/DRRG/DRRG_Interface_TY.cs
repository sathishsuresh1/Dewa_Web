// <copyright file="DRRG_Interface_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_Interface_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Interface_TY")]
    public class DRRG_Interface_TY
    {
        /// <summary>
        /// Gets or sets the InterfaceID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string InterfaceID { get; set; }

        /// <summary>
        /// Gets or sets the Parent PV ID.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Parent_IP_ID { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturer_Code.
        /// </summary>
        [UserDefinedTableTypeColumn(3)]
        public string Manufacturer_Code { get; set; }

        /// <summary>
        /// Gets or sets the Model_Name.
        /// </summary>
        [UserDefinedTableTypeColumn(4)]
        public string Model_Name { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationName.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the CommunicationProtocol.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string CommunicationProtocol { get; set; }

        /// <summary>
        /// Gets or sets the Compliance.
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string Compliance { get; set; }

        /// <summary>
        /// Gets or sets the Session.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string Userid { get; set; }
    }
}
