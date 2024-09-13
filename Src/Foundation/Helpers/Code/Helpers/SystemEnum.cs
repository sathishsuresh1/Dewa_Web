// <copyright file="SystemEnum.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    /// <summary>
    /// Defines the <see cref="SystemEnum" />.
    /// </summary>
    public class SystemEnum
    {
        /// <summary>
        /// Defines the AttachmentType
        /// </summary>
        public enum AttachmentType
        {
            /// <summary>
            /// Defines the Unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Defines the Profile
            /// </summary>
            Profile = 1,
            /// <summary>
            /// Defines the Passport
            /// </summary>
            Passport = 2,
            /// <summary>
            /// Defines the VISA
            /// </summary>
            VISA = 3,
            /// <summary>
            /// Defines the EID
            /// </summary>
            EID = 4,
            /// <summary>
            /// Defines the Mulkiya
            /// </summary>
            Mulkiya = 5,
            /// <summary>
            /// Defines the TradingLicense
            /// </summary>
            TradingLicense = 6,
            /// <summary>
            /// Defines the DrivingLicense
            /// </summary>
            DrivingLicense = 7,

            /// <summary>
            /// Defines the DEWAID.
            /// </summary>
            DEWAID = 8
        }

        /// <summary>
        /// Defines the ProcessType
        /// </summary>
        public enum ProcessType
        {
            /// <summary>
            /// Defines the OnlyImageExtraction
            /// </summary>
            OnlyImageExtraction = 1,
            /// <summary>
            /// Defines the ExtractionImageAndValidateOnlyExtension
            /// </summary>
            ExtractionImageAndValidateOnlyExtension = 2,
            /// <summary>
            /// Defines the ExtractionImageAndValidateCompletely
            /// </summary>
            ExtractionImageAndValidateCompletely = 3,
            /// <summary>
            /// Defines the ExtractionImageAndValidateOnlyExtensionAndOptimizeImage
            /// </summary>
            ExtractionImageAndValidateOnlyExtensionAndOptimizeImage = 4,
            /// <summary>
            /// Defines the ExtractionImageAndValidateCompletelyAndOptimizeImage
            /// </summary>
            ExtractionImageAndValidateCompletelyAndOptimizeImage = 5,
            /// <summary>
            /// Defines the ExtractionImageAndValidateOnlyExtensionAndSortbySizeAndOptimizeImage
            /// </summary>
            ExtractionImageAndValidateOnlyExtensionAndSortbySizeAndOptimizeImage = 6,
            /// <summary>
            /// Defines the ExtractionImageAndValidateCompletelyAndSortbySizeAndOptimizeImage
            /// </summary>
            ExtractionImageAndValidateCompletelyAndSortbySizeAndOptimizeImage = 7,
        }
    }
}
