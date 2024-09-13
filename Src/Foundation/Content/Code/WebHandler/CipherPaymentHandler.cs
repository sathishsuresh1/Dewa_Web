using DEWAXP.Foundation.Integration.APIHandler.Impl;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SecuredPayment;
namespace DEWAXP.Foundation.Content.WebHandler
{

    public class CipherPaymentHandler
    {
        internal static PaymentContext GetPaymentContext(ServiceType type, bool thirdPartyPayment = false)
        {

            switch (type)
            {
                //PayBill
                case ServiceType.PayBill:
                    return thirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
                //MoveOut
                case ServiceType.MoveOut:
                    return thirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
                case ServiceType.ServiceActivation:
                    return thirdPartyPayment ? PaymentContext.AnonymousServiceActivation : PaymentContext.ServiceActivation;
                // ReraServiceActivation
                case ServiceType.ReraServiceActivation:
                    return thirdPartyPayment ? PaymentContext.AnonymousReraServiceActivation : PaymentContext.ReraServiceActivation;
                // Clearance
                case ServiceType.Clearance:
                    return thirdPartyPayment ? PaymentContext.AnonymousClearance : PaymentContext.Clearance;
                // EstimatePayment
                case ServiceType.EstimatePayment:
                    return thirdPartyPayment ? PaymentContext.PayFriendsEstimate : PaymentContext.PayMyEstimate;
                //TemporaryConnection
                case ServiceType.TemporaryConnection:
                    return PaymentContext.TemporaryConnection;
                //Miscellaneous 
                case ServiceType.Miscellaneous:
                    return PaymentContext.Miscellaneous;
                //EV Smart charging 
                case ServiceType.EVAnonymous:
                    return PaymentContext.EVAnonymous;
                //EV card 
                case ServiceType.EVCard:
                    return thirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.EVCard;
                case ServiceType.RammasPayment:
                    return PaymentContext.RammasPayment;
                default:
                    return thirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
            }

        }

        internal static string GetSecuredPayUrl(PaymentMethod paymentMethod)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.PaythroughNoqodiNetBank:
                case PaymentMethod.PaythroughNoqodiEwallet:
                    return Config.SecuredNoqodiPayUrl;
                case PaymentMethod.PaythroughEmiratesNBD:
                    return Config.SecuredEmiratesNBDPayUrl;
                case PaymentMethod.PaythroughNetbanking:
                    return Config.Secured_UAEPGS_PayURL;
                case PaymentMethod.PaythroughEPay:
                case PaymentMethod.PaythroughApplePay:
                default:
                    return Config.SecuredEmiratesNBDPayUrl;

            }

        }
    }
}