using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    /*
    <?xml version="1.0" encoding="UTF-8" standalone="no"?><SetUserActivation><ResponseCode>000</ResponseCode><Description>Your Account is Activated Successfully</Description></SetUserActivation>
    */

    [Serializable]
    [XmlRoot(ElementName = "SetUserActivation")]
    public class MoveinActivationResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        
    }


}
