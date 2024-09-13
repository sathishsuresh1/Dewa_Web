// Binding elements allow the configuration of the WCF run-time stack.
// To use the custom message encoder in a WCF application, a binding element
// is required that creates the message encoder factory with the appropriate 
// settings at the appropriate level in the run-time stack.

namespace DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines the <see cref="CustomASCIITextMessageEncodingBindingElement" />.
    /// </summary>
    public class CustomASCIITextMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        /// <summary>
        /// Gets or sets the MessageVersion.
        /// </summary>
        public override MessageVersion MessageVersion { get; set; }

        /// <summary>
        /// Gets or sets the MediaType.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the Encoding.
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomASCIITextMessageEncodingBindingElement"/> class.
        /// </summary>
        /// <param name="binding">The binding<see cref="CustomASCIITextMessageEncodingBindingElement"/>.</param>
        private CustomASCIITextMessageEncodingBindingElement(CustomASCIITextMessageEncodingBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomASCIITextMessageEncodingBindingElement"/> class.
        /// </summary>
        /// <param name="encoding">The encoding<see cref="string"/>.</param>
        /// <param name="mediaType">The mediaType<see cref="string"/>.</param>
        /// <param name="messageVersion">The messageVersion<see cref="MessageVersion"/>.</param>
        public CustomASCIITextMessageEncodingBindingElement(string encoding, string mediaType,
            MessageVersion messageVersion)
        {
            MessageVersion = messageVersion;
            MediaType = mediaType;
            Encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomASCIITextMessageEncodingBindingElement"/> class.
        /// </summary>
        /// <param name="encoding">The encoding<see cref="string"/>.</param>
        /// <param name="messageVersion">The messageVersion<see cref="MessageVersion"/>.</param>
        public CustomASCIITextMessageEncodingBindingElement(string encoding, MessageVersion messageVersion)
        {
            Encoding = encoding;
            MessageVersion = messageVersion;
            if (messageVersion.Envelope == EnvelopeVersion.Soap11)
            {
                MediaType = "text/xml";
            }
            else if (messageVersion.Envelope == EnvelopeVersion.Soap12)
            {
                MediaType = "application/soap+xml";
            }
            else
            {
                MediaType = "application/xml";
            }
        }

        /// <summary>
        /// The Clone.
        /// </summary>
        /// <returns>The <see cref="BindingElement"/>.</returns>
        public override BindingElement Clone()
        {
            return new CustomASCIITextMessageEncodingBindingElement(this);
        }

        /// <summary>
        /// The CreateMessageEncoderFactory.
        /// </summary>
        /// <returns>The <see cref="MessageEncoderFactory"/>.</returns>
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomASCIITextMessageEncoderFactory(MediaType, Encoding, MessageVersion);
        }

        /// <summary>
        /// The BuildChannelFactory.
        /// </summary>
        /// <typeparam name="TChannel">.</typeparam>
        /// <param name="context">The context<see cref="BindingContext"/>.</param>
        /// <returns>The <see cref="IChannelFactory{TChannel}"/>.</returns>
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }
    }
}
