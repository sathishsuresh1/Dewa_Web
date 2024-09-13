using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    
    public class GetFileResponse
    {
        public string FileName { get; set; }

        public byte[] filebytes { get; set; }

        
    }

}
