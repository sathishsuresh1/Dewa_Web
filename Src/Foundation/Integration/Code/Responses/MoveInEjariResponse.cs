using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class MoveInEjariResponse : BaseResponse
    {
        public DewaSvc.moveInEjari moveinEjari { get; set; } 
        public static MoveInEjariResponse From(DewaSvc.moveInEjari payload)
        {
            MoveInEjariResponse typedResponse = new MoveInEjariResponse();
            int responseCode = 0;
            int.TryParse(payload.responseCode, out responseCode);
            typedResponse.ResponseCode = responseCode;
            typedResponse.Description = payload.description;
            typedResponse.moveinEjari = payload;

            return typedResponse;
        }
    }
}
