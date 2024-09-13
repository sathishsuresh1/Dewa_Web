﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message)
            : base(message)
        {
        }
    }
}
