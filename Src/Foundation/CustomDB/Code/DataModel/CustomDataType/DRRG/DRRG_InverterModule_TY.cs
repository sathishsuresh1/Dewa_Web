// <copyright file="DRRG_PVModule_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_PVModule_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Inverter_TY")]
    public class DRRG_Inverter_TY
    {
        /// <summary>
        /// Gets or sets the InverterID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string InverterID { get; set; }

        /// <summary>
        /// Gets or sets the Parent PV ID.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Parent_Inv_ID { get; set; }

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
        /// Gets or sets the Power_Factor_Range.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string Power_Factor_Range { get; set; }

        /// <summary>
        /// Gets or sets the Power_Factor_Range.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string Possibility_DC_Conductors { get; set; }

        /// <summary>
        /// Gets or sets the Number_Of_Phases.[Possibility DC conductors]
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string Number_Of_Phases { get; set; }

        /// <summary>
        /// Gets or sets the Remote_Control.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string Remote_Control { get; set; }

        /// <summary>
        /// Gets or sets the Remote_Monitoring.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string Remote_Monitoring { get; set; }

        /// <summary>
        /// Gets or sets the LVRT.
        /// </summary>
        [UserDefinedTableTypeColumn(10)]
        public string LVRT { get; set; }

        /// <summary>
        /// Gets or sets the Protection_Degree.
        /// </summary>
        [UserDefinedTableTypeColumn(11)]
        public string Protection_Degree { get; set; }

        /// <summary>
        /// Gets or sets the Internal_Interface.
        /// </summary>
        [UserDefinedTableTypeColumn(12)]
        public string Internal_Interface { get; set; }

        /// <summary>
        /// Gets or sets the Multi_Master_Feature.
        /// </summary>
        [UserDefinedTableTypeColumn(13)]
        public string Multi_Master_Feature { get; set; }

        /// <summary>
        /// Gets or sets the Function_String.
        /// </summary>
        [UserDefinedTableTypeColumn(14)]
        public string Function_String { get; set; }

        /// <summary>
        /// Gets or sets the Number_String.
        /// </summary>
        [UserDefinedTableTypeColumn(15)]
        public string Number_String { get; set; }

        /// <summary>
        /// Gets or sets the MPPT_Section.
        /// </summary>
        [UserDefinedTableTypeColumn(16)]
        public string MPPT_Section { get; set; }

        /// <summary>
        /// Gets or sets the Back_Superstrate
        /// Gets or sets a value indicating whether Local_Representative....
        /// </summary>
        [UserDefinedTableTypeColumn(17)]
        public string Number_Section { get; set; }

        /// <summary>
        /// Gets or sets the AC_DC_Section.
        /// </summary>
        [UserDefinedTableTypeColumn(18)]
        public string AC_DC_Section { get; set; }

        /// <summary>
        /// Gets or sets the Power_Derating.
        /// </summary>
        [UserDefinedTableTypeColumn(19)]
        public string Power_Derating { get; set; }

        /// <summary>
        /// Gets or sets the Session.
        /// </summary>
        [UserDefinedTableTypeColumn(20)]
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [UserDefinedTableTypeColumn(21)]
        public string Userid { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="DRRG_Inverter_RP_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Inverter_RP_TY")]
    public class DRRG_Inverter_RP_TY
    {
        /// <summary>
        /// Gets or sets the InverterID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string InverterID { get; set; }

        /// <summary>
        /// Gets or sets the Rated_Power.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Rated_Power { get; set; }
    }
    /// <summary>
    /// Defines the <see cref="DRRG_Inverter_AP_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Inverter_AP_TY")]
    public class DRRG_Inverter_AP_TY
    {
        /// <summary>
        /// Gets or sets the InverterID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string InverterID { get; set; }

        /// <summary>
        /// Gets or sets the AC_Apparent_Power.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string AC_Apparent_Power { get; set; }
    }
    /// <summary>
    /// Defines the <see cref="DRRG_Inverter_MAP_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Inverter_MAP_TY")]
    public class DRRG_Inverter_MAP_TY
    {
        /// <summary>
        /// Gets or sets the InverterID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string InverterID { get; set; }

        /// <summary>
        /// Gets or sets the Max_Active_Power.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Max_Active_Power { get; set; }
    }
}
