using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class ServiceResponse
    {
        public string Message { get; private set; }

        public bool Succeeded { get; private set; }

        public ServiceResponse(bool succeeded = true, string message = "Success")
        {
            Message = message;
            Succeeded = succeeded;
        }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T Payload { get; private set; }

        public ServiceResponse(T payload, bool succeeded = true, string message = "Success")
            : base(succeeded, message)
        {
            Payload = payload;
        }
    }
}
