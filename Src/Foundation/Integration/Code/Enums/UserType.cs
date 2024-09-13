using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Enums
{
    public enum UserType : byte
    {
        Customer = 0x43,
        Contractor = 0x52,
        Governmental = 0x47,
        Candidate = 0x44,
        Supplier = 0x53
    }

    public static class UserTypeExtensions
    {
        public static string Code(this UserType type)
        {
            return new string(new [] { (char)type });
        }
    }
}
