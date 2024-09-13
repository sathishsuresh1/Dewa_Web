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
    [UserDefinedTableType("DRRG_PVModule_TY")]
    public class DRRG_PVModule_TY
    {
        /// <summary>
        /// Gets or sets the PVID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string PVID { get; set; }

        /// <summary>
        /// Gets or sets the Parent PV ID.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Parent_PV_ID { get; set; }

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
        /// Gets or sets the Nominal_Power.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public string Nominal_Power { get; set; }

        /// <summary>
        /// Gets or sets the Module_Length.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string Module_Length { get; set; }

        /// <summary>
        /// Gets or sets the Module_Width.
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string Module_Width { get; set; }

        /// <summary>
        /// Gets or sets the Cell_Technology.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string Cell_Technology { get; set; }

        /// <summary>
        /// Gets or sets the Other_Cell_Technology.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string Other_Cell_Technology { get; set; }


        /// <summary>
        /// Gets or sets the PV Cell Structure.
        /// </summary>
        [UserDefinedTableTypeColumn(10)]
        public string PV_Cell_Structure { get; set; }

        /// <summary>
        /// Gets or sets the Other PV Cell Structure.
        /// </summary>
        [UserDefinedTableTypeColumn(11)]
        public string Other_PV_Cell_Structure { get; set; }

        /// <summary>
        /// Gets or sets the Cell_Technology.
        /// </summary>
        [UserDefinedTableTypeColumn(12)]
        public bool Bifacial { get; set; }

        /// <summary>
        /// Gets or sets the Framed.
        /// </summary>
        [UserDefinedTableTypeColumn(13)]
        public string Framed { get; set; }

        /// <summary>
        /// Gets or sets the Front_Superstrate.
        /// </summary>
        [UserDefinedTableTypeColumn(14)]
        public string Front_Superstrate { get; set; }

        /// <summary>
        /// Gets or sets the Othe_Front_Superstrate.
        /// </summary>
        [UserDefinedTableTypeColumn(15)]
        public string Othe_Front_Superstrate { get; set; }


        /// <summary>
        /// Gets or sets the Back_Superstrate
        /// Gets or sets a value indicating whether Local_Representative....
        /// </summary>
        [UserDefinedTableTypeColumn(16)]
        public string Back_Superstrate { get; set; }

        /// <summary>
        /// Gets or sets the Other_Back_Superstrate.
        /// </summary>
        [UserDefinedTableTypeColumn(17)]
        public string Other_Back_Superstrate { get; set; }

        /// <summary>
        /// Gets or sets the Encapsulant.
        /// </summary>
        [UserDefinedTableTypeColumn(18)]
        public string Encapsulant { get; set; }

        /// <summary>
        /// Gets or sets the Othe_Encapsulant.
        /// </summary>
        [UserDefinedTableTypeColumn(19)]
        public string Othe_Encapsulant { get; set; }

        /// <summary>
        /// Gets or sets the DC_System_Grounding_Mandatory.
        /// </summary>
        [UserDefinedTableTypeColumn(20)]
        public string DC_System_Grounding_Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the DC_System_Grounding.
        /// </summary>
        [UserDefinedTableTypeColumn(21)]
        public string DC_System_Grounding { get; set; }

        /// <summary>
        /// Gets or sets the Position_JB.
        /// </summary>
        [UserDefinedTableTypeColumn(22)]
        public string Position_JB { get; set; }

        /// <summary>
        /// Gets or sets the Material_JB.
        /// </summary>
        [UserDefinedTableTypeColumn(23)]
        public string Material_JB { get; set; }

        /// <summary>
        /// Gets or sets the Other_Material_JB.
        /// </summary>
        [UserDefinedTableTypeColumn(24)]
        public string Other_Material_JB { get; set; }

        /// <summary>
        /// Gets or sets the Features_JB.
        /// </summary>
        [UserDefinedTableTypeColumn(25)]
        public string Features_JB { get; set; }

        /// <summary>
        /// Gets or sets the Other_Features_JB.
        /// </summary>
        [UserDefinedTableTypeColumn(26)]
        public string Other_Features_JB { get; set; }

        /// <summary>
        /// Gets or sets the Terminations.
        /// </summary>
        [UserDefinedTableTypeColumn(27)]
        public string Terminations { get; set; }

        /// <summary>
        /// Gets or sets the Other_Terminations.
        /// </summary>
        [UserDefinedTableTypeColumn(28)]
        public string Other_Terminations { get; set; }

        /// <summary>
        /// Gets or sets the Session.
        /// </summary>
        [UserDefinedTableTypeColumn(29)]
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [UserDefinedTableTypeColumn(30)]
        public string Userid { get; set; }

    }
}
