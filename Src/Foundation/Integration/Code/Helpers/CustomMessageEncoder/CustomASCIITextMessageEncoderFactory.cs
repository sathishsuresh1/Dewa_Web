// <copyright file="CustomASCIITextMessageEncoderFactory.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder
{
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines the <see cref="CustomASCIITextMessageEncoderFactory" />.
    /// </summary>
    public class CustomASCIITextMessageEncoderFactory : MessageEncoderFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomASCIITextMessageEncoderFactory"/> class.
        /// </summary>
        /// <param name="mediaType">The mediaType<see cref="string"/>.</param>
        /// <param name="charSet">The charSet<see cref="string"/>.</param>
        /// <param name="version">The version<see cref="MessageVersion"/>.</param>
        internal CustomASCIITextMessageEncoderFactory(string mediaType, string charSet,
            MessageVersion version)
        {
            MessageVersion = version;
            MediaType = mediaType;
            CharSet = charSet;
            Encoder = new CustomASCIITextMessageEncoder(this);
        }

        /// <summary>
        /// Gets the Encoder.
        /// </summary>
        public override MessageEncoder Encoder { get; }

        /// <summary>
        /// Gets the MessageVersion.
        /// </summary>
        public override MessageVersion MessageVersion { get; }

        /// <summary>
        /// Gets the MediaType.
        /// </summary>
        internal string MediaType { get; }

        /// <summary>
        /// Gets the CharSet.
        /// </summary>
        internal string CharSet { get; }
    }
}
