
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder
{
    public class CustomTextMessageEncoder : MessageEncoder
    {
        private readonly CustomTextMessageEncoderFactory _factory;
        private readonly XmlWriterSettings _writerSettings;

        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            this._factory = factory;
            this._writerSettings = new XmlWriterSettings();            
        }

        public override string ContentType
        {
            get
            {
                return "text/xml";
            }
        }

        public override string MediaType
        {
            get 
            {
                return "text/xml";
            }
        }

        public override MessageVersion MessageVersion
        {
            get 
            {
                return this._factory.MessageVersion;
            }
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {   
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            MemoryStream stream = new MemoryStream(msgContents);
            return ReadMessage(stream, int.MaxValue);
        }

        private Stream RemoveSignatures(Stream stream)
        {
            var sr = new StreamReader(stream);
            var wireResponse = sr.ReadToEnd();
            // Fix for Xml external entity injection violation in fortify report
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Prohibit;
            settings.XmlResolver = null;

            XmlDocument doc = new XmlDocument();
            XmlReader reader = XmlReader.Create(new StringReader(wireResponse), settings);
            doc.Load(reader);

            //XmlDocument doc = new XmlDocument();
            //doc.Load(stream);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsMgr.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
            nsMgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            XmlNode signatureNode = doc.SelectSingleNode("/soap:Envelope/soap:Header/wsse:Security/dsig:Signature", nsMgr);
            XmlNode binarySecurityTokenNode = doc.SelectSingleNode("/soap:Envelope/soap:Header/wsse:Security/wsse:BinarySecurityToken", nsMgr);
            XmlNode headerNode = doc.SelectSingleNode("/soap:Envelope/soap:Header/wsse:Security", nsMgr);

            if (headerNode != null)
            {
	            if (signatureNode != null)
	            {
					headerNode.RemoveChild(signatureNode);
				}

	            if (binarySecurityTokenNode != null)
	            {
					headerNode.RemoveChild(binarySecurityTokenNode);
				}
            }
            return new MemoryStream(new UTF8Encoding().GetBytes(doc.OuterXml));
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            stream = RemoveSignatures(stream);
            var sr = new StreamReader(stream);
            var wireResponse = sr.ReadToEnd();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Prohibit;
            settings.XmlResolver = null;

            XmlReader reader = XmlReader.Create(new StringReader(wireResponse), settings);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion.Soap11);            
        }

        internal static byte[] ExtractIvAndDecrypt(SymmetricAlgorithm algorithm, byte[] cipherText, int offset, int count)
        {
            if (cipherText == null)
            {
                throw new Exception();
            }
            if ((count < 0) || (count > cipherText.Length))
            {
                throw new Exception();
            }
            if ((offset < 0) || (offset > (cipherText.Length - count)))
            {
                throw new Exception();
            }
            int num = algorithm.BlockSize / 8;
            byte[] dst = new byte[num];
            Buffer.BlockCopy(cipherText, offset, dst, 0, dst.Length);
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Mode = CipherMode.CBC;

            using (ICryptoTransform transform = algorithm.CreateDecryptor(algorithm.Key, dst))
            {
                return transform.TransformFinalBlock(cipherText, offset + dst.Length, count - dst.Length);
            }
        }
        
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsMgr.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
            nsMgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            nsMgr.AddNamespace("ws", "http://ws.dmodel.customer.dewa.gov.ae/");

            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this._writerSettings);
            message.WriteMessage(writer);
            writer.Close();
            stream.Reset();
            
            var xmlMessage = XDocument.Load(stream);
            var secNode = xmlMessage.XPathSelectElement("/soap:Envelope/soap:Header/wsse:Security", nsMgr);
            if (secNode != null)
            {
                // First attribute is s:mustUnderstand
                secNode.FirstAttribute.SetValue("0");

                stream.Reset();
                xmlMessage.Save(stream);
            }

            byte[] messageBytes = stream.GetBuffer();
            int messageLength = (int)stream.Position;
            stream.Close();

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            XmlWriter writer = XmlWriter.Create(stream, this._writerSettings);
            message.WriteMessage(writer);
            writer.Close();
        }
    }
}
