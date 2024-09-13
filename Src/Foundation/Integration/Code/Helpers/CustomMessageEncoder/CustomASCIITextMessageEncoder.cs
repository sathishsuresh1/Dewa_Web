// <copyright file="CustomASCIITextMessageEncoder.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

/// <summary>
/// Defines the <see cref="CustomASCIITextMessageEncoder" />.
/// </summary>
public class CustomASCIITextMessageEncoder : MessageEncoder
{
    /// <summary>
    /// Defines the factory.
    /// </summary>
    private CustomASCIITextMessageEncoderFactory factory;

    /// <summary>
    /// Defines the writerSettings.
    /// </summary>
    private XmlWriterSettings writerSettings;

    /// <summary>
    /// Defines the contentType.
    /// </summary>
    private readonly string contentType;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomASCIITextMessageEncoder"/> class.
    /// </summary>
    /// <param name="factory">The factory<see cref="CustomASCIITextMessageEncoderFactory"/>.</param>
    public CustomASCIITextMessageEncoder(CustomASCIITextMessageEncoderFactory factory)
    {
        this.factory = factory;

        writerSettings = new XmlWriterSettings
        {
            Encoding = Encoding.GetEncoding(factory.CharSet)
        };
        contentType = (writerSettings.Encoding.Equals(Encoding.ASCII) ? "text/xml;charset=ASCII" : $"{factory.MediaType};charset={writerSettings.Encoding.HeaderName}");
    }

    /// <summary>
    /// The IsContentTypeSupported.
    /// </summary>
    /// <param name="contentType">The contentType<see cref="string"/>.</param>
    /// <returns>The <see cref="bool"/>.</returns>
    public override bool IsContentTypeSupported(string contentType)
    {
        return base.IsContentTypeSupported(contentType) || contentType == MediaType;
    }

    /// <summary>
    /// Gets the ContentType.
    /// </summary>
    public override string ContentType => contentType;

    /// <summary>
    /// Gets the MediaType.
    /// </summary>
    public override string MediaType => factory.MediaType;

    /// <summary>
    /// Gets the MessageVersion.
    /// </summary>
    public override MessageVersion MessageVersion => factory.MessageVersion;

    /// <summary>
    /// The ReadMessage.
    /// </summary>
    /// <param name="buffer">The buffer<see cref="ArraySegment{byte}"/>.</param>
    /// <param name="bufferManager">The bufferManager<see cref="BufferManager"/>.</param>
    /// <param name="contentType">The contentType<see cref="string"/>.</param>
    /// <returns>The <see cref="Message"/>.</returns>
    public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
    {
        byte[] msgContents = new byte[buffer.Count];
        Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
        bufferManager.ReturnBuffer(buffer.Array);

        MemoryStream stream = new MemoryStream(msgContents);
        return ReadMessage(stream, int.MaxValue);
    }

    /// <summary>
    /// The ReadMessage.
    /// </summary>
    /// <param name="stream">The stream<see cref="Stream"/>.</param>
    /// <param name="maxSizeOfHeaders">The maxSizeOfHeaders<see cref="int"/>.</param>
    /// <param name="contentType">The contentType<see cref="string"/>.</param>
    /// <returns>The <see cref="Message"/>.</returns>
    public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
    {
        XmlReader reader = XmlReader.Create(stream);
        return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
    }

    /// <summary>
    /// The WriteMessage.
    /// </summary>
    /// <param name="message">The message<see cref="Message"/>.</param>
    /// <param name="maxMessageSize">The maxMessageSize<see cref="int"/>.</param>
    /// <param name="bufferManager">The bufferManager<see cref="BufferManager"/>.</param>
    /// <param name="messageOffset">The messageOffset<see cref="int"/>.</param>
    /// <returns>The <see cref="ArraySegment{byte}"/>.</returns>
    public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
    {
        MemoryStream stream = new MemoryStream();
        XmlWriter writer = XmlWriter.Create(stream, writerSettings);
        message.WriteMessage(writer);
        writer.Close();

        byte[] messageBytes = stream.GetBuffer();
        int messageLength = (int)stream.Position;
        stream.Close();

        int totalLength = messageLength + messageOffset;
        byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
        Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

        ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
        return byteArray;
    }

    /// <summary>
    /// The WriteMessage.
    /// </summary>
    /// <param name="message">The message<see cref="Message"/>.</param>
    /// <param name="stream">The stream<see cref="Stream"/>.</param>
    public override void WriteMessage(Message message, Stream stream)
    {
        XmlWriter writer = XmlWriter.Create(stream, writerSettings);
        message.WriteMessage(writer);
        writer.Close();
    }
}
