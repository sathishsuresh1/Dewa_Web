// <copyright file="DRRG_Factory_Details_TY.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG
{
    using EntityFrameworkExtras.EF6;

    /// <summary>
    /// Defines the <see cref="DRRG_Files_TY" />.
    /// </summary>
    [UserDefinedTableType("DRRG_Files_TY")]
    public class DRRG_Files_TY
    {
        /// <summary>
        /// Gets or sets the Reference_ID.
        /// </summary>
        [UserDefinedTableTypeColumn(1)]
        public string Reference_ID { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [UserDefinedTableTypeColumn(2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ContentType.
        /// </summary>
        [UserDefinedTableTypeColumn(3)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Extension.
        /// </summary>
        [UserDefinedTableTypeColumn(4)]
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        [UserDefinedTableTypeColumn(5)]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the Size.
        /// </summary>
        [UserDefinedTableTypeColumn(6)]
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the File_Type.
        /// </summary>
        [UserDefinedTableTypeColumn(7)]
        public string File_Type { get; set; }

        /// <summary>
        /// Gets or sets the Entity.
        /// </summary>
        [UserDefinedTableTypeColumn(8)]
        public string Entity { get; set; }

        /// <summary>
        /// Gets or sets the Manufacturercode.
        /// </summary>
        [UserDefinedTableTypeColumn(9)]
        public string Manufacturercode { get; set; }
    }
}
