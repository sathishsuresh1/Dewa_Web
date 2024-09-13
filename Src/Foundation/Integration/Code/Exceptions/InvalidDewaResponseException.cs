using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Exceptions
{
    public class InvalidDewaResponseException : Exception
    {
        public InvalidDewaResponseException(string xml)
            : base(string.Format("Invalid response received. Response body: {0}", xml))
        {
        }
    }
}
