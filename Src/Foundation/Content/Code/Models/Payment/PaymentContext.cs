using System;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Content.Models.Payment
{
    public enum PaymentContext
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
        MoveOutActivation = 11,
        Miscellaneous = 12,
        EVCard = 13,
        EVAnonymous = 14, RammasPayment = 15
    }


    public enum ServiceType
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
        MoveOutActivation = 11,
        Miscellaneous = 12,
        EVCard = 13,
        MoveOut = 14,
        EstimatePayment = 15,
        EVAnonymous = 16,RammasPayment=17
    }
    public enum PaymentMethod
    {
        PaythroughEPay = 1,
        PaythroughApplePay = 2,
        PaythroughNoqodiNetBank = 3,
        PaythroughNoqodiEwallet = 4,
        PaythroughEmiratesNBD = 5,
        PaythroughNetbanking = 6,
        PaythroughSamsungPay = 7,
        PayOffline = 0

    }

    public static class PaymentContextExtensions
    {
        public static string EPayTransactionCode(this PaymentContext context, RequestSegment segment)
        {
            switch (context)
            {
                case PaymentContext.PayFriendsBill:
                    return segment == RequestSegment.Desktop ? "m1b2d" : "m1b2";
                case PaymentContext.ServiceActivation:
                case PaymentContext.ReraServiceActivation:
                    return segment == RequestSegment.Desktop ? "w1a1mwd" : "w1a1mw";
                case PaymentContext.AnonymousServiceActivation:
                case PaymentContext.AnonymousReraServiceActivation:
                    return segment == RequestSegment.Desktop ? "w1a2mwd" : "w1a2mw";
                case PaymentContext.Clearance:
                    return segment == RequestSegment.Desktop ? "w1cc2mwd" : "w1cc2mw";
                case PaymentContext.AnonymousClearance:
                    return segment == RequestSegment.Desktop ? "w1cc1mwd" : "w1cc1mw";
                case PaymentContext.PayMyEstimate:
                    return "w2cc";
                case PaymentContext.PayFriendsEstimate:
                    return "w2ccp";
                case PaymentContext.TemporaryConnection:
                    return "w1tc1";
                case PaymentContext.Miscellaneous:
                    return "mis1mw";
                case PaymentContext.EVCard:
                    return segment == RequestSegment.Desktop ? "ev1mwd" : "ev1mw";
                case PaymentContext.EVAnonymous:
                    return segment == RequestSegment.Desktop ? "eva2mwd" : "eva2mw";
                case PaymentContext.RammasPayment:
                    return "r1b1";
                default:
                    return segment == RequestSegment.Desktop ? "m1b1d" : "m1b1";
            }
        }

        public static PaymentContext Parse(string type)
        {
            switch (type)
            {
                case "m1b2":
                case "m1b2d":
                    return PaymentContext.PayFriendsBill;
                case "w1a1mw":
                case "w1a1mwd":
                    return PaymentContext.ServiceActivation;
                case "w1a2mw":
                case "w1a2mwd":
                    return PaymentContext.AnonymousServiceActivation;
                case "w1cc2mw":
                case "w1cc2mwd":
                    return PaymentContext.Clearance;
                case "m1cc1mw":
                case "m1cc1mwd":
                    return PaymentContext.AnonymousClearance;
                case "w2cc":
                    return PaymentContext.PayMyEstimate;
                case "w2ccp":
                    return PaymentContext.PayFriendsEstimate;
                case "w1tc1":
                    return PaymentContext.TemporaryConnection;
                case "mis1mw":
                    return PaymentContext.Miscellaneous;
                case "ev1mwd":
                case "ev1mw":
                    return PaymentContext.EVCard;
                case "eva2mw":
                case "eva2mwd":
                    return PaymentContext.EVAnonymous;
                case "r1b1":
                    return PaymentContext.RammasPayment;
                default:
                    return PaymentContext.PayBill;
            }
        }
        public static string GetPaymentMode(PaymentMethod paymentMethod)
        {
            string _paymentMeth = string.Empty;
            switch (paymentMethod)
            {
                case PaymentMethod.PaythroughEPay:
                    _paymentMeth = string.Empty;
                    break;
                case PaymentMethod.PaythroughApplePay:
                    _paymentMeth = string.Empty;
                    break;
                case PaymentMethod.PaythroughNoqodiNetBank:
                    _paymentMeth = "DDB";
                    break;
                case PaymentMethod.PaythroughNoqodiEwallet:
                case PaymentMethod.PaythroughEmiratesNBD:
                    _paymentMeth = "ECA";
                    break;
                case PaymentMethod.PaythroughSamsungPay:
                    _paymentMeth = string.Empty;
                    break; 
                default:
                    break;
            }
            return _paymentMeth;
        }

        public static PaymentMethod GetPaymentMethod(string method)
        {
            switch (method)
            {
                case "enbd":
                    return PaymentMethod.PaythroughEmiratesNBD;
                //case "sdg":
                //    return PaymentMethod.PaythroughEPay;
                default:
                    return PaymentMethod.PaythroughEmiratesNBD;
            }
        }

        public static string GetPaymentChannelType(PaymentMethod paymentMethod)
        {
            string _paymentMeth = string.Empty;
            switch (paymentMethod)
            {
                case PaymentMethod.PaythroughApplePay:
                    _paymentMeth = "apple";
                    break;
                case PaymentMethod.PaythroughSamsungPay:
                    _paymentMeth = "samsung";
                    break;
                default:
                    break;
            }
            return _paymentMeth;
        }
    }
}