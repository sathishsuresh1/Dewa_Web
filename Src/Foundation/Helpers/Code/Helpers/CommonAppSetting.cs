// <copyright file="CommonAppSetting.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Defines the <see cref="CommonAppSetting" />
    /// </summary>
    public class CommonAppSetting
    {
        /// <summary>
        /// Gets the SourceImgPath
        /// Source Folder base path
        /// </summary>
        public static string SourceImgPath => ConfigurationManager.AppSettings["SourceImgPath"];

        /// <summary>
        /// Gets the DestinationImgPath
        /// Migration Folder base path
        /// </summary>
        public static string DestinationImgPath => ConfigurationManager.AppSettings["DestinationImgPath"];

        /// <summary>
        /// Gets a value indicating whether EnableArchivedFileDeletion
        /// Enabled the Archived File Deletion after Migration completed
        /// </summary>
        public static bool EnableArchivedFileDeletion => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableArchiveFileDeletion"] == "true");

        /// <summary>
        /// Gets a value indicating whether EnabledReportTest
        /// Enable RepostTest Delete Previous process files.
        /// </summary>
        public static bool EnabledReportTest => Convert.ToBoolean(ConfigurationManager.AppSettings["EnabledReportTest"] == "true");

        /// <summary>
        /// Gets the ImageExtension
        /// </summary>
        public static string ImageExtension => Convert.ToString(ConfigurationManager.AppSettings["ImageExtension"] ?? ".");

        /// <summary>
        /// Gets the BaseOptimizeUnitPercent
        /// </summary>
        public static int BaseOptimizeUnitPercent => Convert.ToInt32(ConfigurationManager.AppSettings["BaseOptimizeUnitPercent"] ?? "10");
    }
}
