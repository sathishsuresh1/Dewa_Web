using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.MoveOut
{
    public class MoveOutState
    {
        public MoveOutResult moveoutresult { get; set; }
        public MoveOutResponse moveoutdetails { get; set; }
        public List<string> page { get; set; }
    }
}