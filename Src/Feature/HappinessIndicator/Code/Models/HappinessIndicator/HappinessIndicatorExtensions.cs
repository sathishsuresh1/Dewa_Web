using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HappinessIndicator.Models.HappinessIndicator
{
    public enum HappinessIndicatorServices
    {
        PayBill = 0,
        TemporaryConnection = 1,
        PayFriendsBill = 2,
        PayFriendsEstimate = 3,
        PayMyEstimate = 4,
        Clearance = 5,
        AnonymousClearance = 6,
        ServiceActivation = 7,
        AnonymousServiceActivation = 8,
        ReraServiceActivation = 9,
        AnonymousReraServiceActivation = 10,
        MoveOut = 11,
        UpdateContactDetails = 12
    }
    public static class HappinessIndicatorExtensions
    {
        public static HappinessIndicatorServices Parse(string type)
        {
            switch (type)
            {
                case "m1b2":
                case "m1b2d":
                    return HappinessIndicatorServices.PayFriendsBill;
                case "w1a1mw":
                case "w1a1mwd":
                    return HappinessIndicatorServices.ServiceActivation;
                case "w1a2mw":
                case "w1a2mwd":
                    return HappinessIndicatorServices.AnonymousServiceActivation;
                case "w1cc2mw":
                case "w1cc2mwd":
                    return HappinessIndicatorServices.Clearance;
                case "m1cc1mw":
                case "m1cc1mwd":
                    return HappinessIndicatorServices.AnonymousClearance;
                case "w2cc":
                    return HappinessIndicatorServices.PayMyEstimate;
                case "w2ccp":
                    return HappinessIndicatorServices.PayFriendsEstimate;
                case "w1tc1":
                    return HappinessIndicatorServices.TemporaryConnection;
                case "mo":
                    return HappinessIndicatorServices.MoveOut;
                case "uc":
                    return HappinessIndicatorServices.UpdateContactDetails;
                default:
                    return HappinessIndicatorServices.PayBill;
            }
        }
    }
}