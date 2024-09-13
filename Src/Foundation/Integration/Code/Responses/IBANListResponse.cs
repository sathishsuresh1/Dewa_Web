using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;

namespace DEWAXP.Foundation.Integration.Responses
{
	public class IBANListResponse
	{
		public string BPNumber;

		public string Description;

		public ibanListchild[] IBAN;

		public string lang;

		public string ResponseCode;

		public string UserId;

		public IBANListResponse()
        {
          
        }
		
    }

  
}
