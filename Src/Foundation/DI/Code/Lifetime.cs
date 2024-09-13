using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.DI
{
    public enum Lifetime
    {
        Transient,
        Singleton,
        Scoped
    }
}