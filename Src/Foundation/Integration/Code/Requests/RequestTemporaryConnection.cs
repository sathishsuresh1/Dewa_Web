using System;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class RequestTemporaryConnection
    {
        public string MobileNumber { get; set; }

        public string City { get; set; }

        public string ContractAccountNumber { get; set; }

        public string Remarks { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public EventType EventType { get; set; }
    }
}
