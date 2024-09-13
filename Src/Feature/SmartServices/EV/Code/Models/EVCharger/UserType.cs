using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Flags]
    public enum UserType : byte
    {
        DewaCustomer = 0x00,
        NoDewaAccount = 0x01,
        NoDewaCustomer = 0x02
    }

    public enum AccountType : int
    {
        Personal = 1,
        Business = 2,
        Government = 3
    }

    public class EVDocType
    {
        public const string PassportDocType = "Z00001";
        public const string EidDocType = "Z00002";
        public const string TradLicenseDocType = "Z00005";
    }

    public class Authority
    {
        public const string Other = "TL0999";
    }

}