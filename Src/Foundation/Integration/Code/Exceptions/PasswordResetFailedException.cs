using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Exceptions
{
    public class PasswordResetFailedException : Exception
    {
        public PasswordResetFailedException(string message)
            : base(message)
        { }
    }
}
