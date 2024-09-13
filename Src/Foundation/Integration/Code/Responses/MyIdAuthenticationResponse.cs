using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "GetMyID", Namespace = "")]
    public class MyIdAuthenticationResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Credential")]
        public string Credential { get; set; }

        public bool CredentialIsKnown
        {
            get { return !string.IsNullOrEmpty(Credential); }
        }
    }

    [XmlRoot(ElementName = "Response", Namespace = "urn:MyIDWS")]
    public class Response
    {
        [XmlElement(ElementName = "GetMyID", Namespace = "")]
        public MyIdAuthenticationResponse GetMyID { get; set; }
    }

    [XmlRoot(ElementName = "getMyIDResponse", Namespace = "urn:MyIDWS")]
    public class GetMyIDResponse
    {
        [XmlElement(ElementName = "Response", Namespace = "urn:MyIDWS")]
        public Response Response { get; set; }
        [XmlAttribute(AttributeName = "rpl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Rpl { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "getMyIDResponse", Namespace = "urn:MyIDWS")]
        public GetMyIDResponse GetMyIDResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "SOAP-ENV", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string SOAPENV { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

}
