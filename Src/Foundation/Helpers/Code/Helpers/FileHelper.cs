// <copyright file="FileHelper.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using System.IO;

    /// <summary>
    /// Defines the <see cref="FileHelper" />
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Defines the MemoryConversion
        /// </summary>
        public enum MemoryConversion
        {
            /// <summary>
            /// Defines the Decrease
            /// </summary>
            Decrease = 0,

            /// <summary>
            /// Defines the Increase
            /// </summary>
            Increase = 1,
        }

        /// <summary>
        /// Validate folder  IsExit if not Create folder & return filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isVirtualPath">Optional</param>
        /// <returns>The <see cref="string"/></returns>
        public static string IsExitOrCreate(string filePath, bool isVirtualPath = false)
        {
            filePath = PathResolver(filePath);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            return filePath;
        }

        /// <summary>
        /// The IsExitOrDelete
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/></param>
        public static void IsExitOrDelete(string filePath)
        {
            filePath = PathResolver(filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// The EmtpyFolderByPath
        /// </summary>
        /// <param name="folderPath">The folderPath<see cref="string"/></param>
        public static void EmtpyFolderByPath(string folderPath)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(folderPath);
            //delete folders
            foreach (DirectoryInfo fi in directory.GetDirectories())
            {
                fi.Delete(true);
            }
        }

        /// <summary>
        /// The PathResolver
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string PathResolver(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
            }
            return filePath;
        }

        /// <summary>
        /// The ConvertMemoryByUnit
        /// </summary>
        /// <param name="length">The length<see cref="long"/></param>
        /// <param name="conversiontype">The conversiontype<see cref="MemoryConversion"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertMemoryByUnit(long length, MemoryConversion conversiontype = MemoryConversion.Increase)
        {
            if (conversiontype == MemoryConversion.Increase)
            {
                return length / 1024;
            }
            else
            {
                return length * 1024;
            }
        }
    }
}
