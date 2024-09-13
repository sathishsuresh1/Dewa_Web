using System;

namespace DEWAXP.Feature.Account.Models.CustomerRegistration
{
    [Flags]
    public enum VerificationMethods : byte
    {
        None = 0x00,
        Email = 0x01,
        Mobile = 0x02,
        Both = 0x03
    }
}