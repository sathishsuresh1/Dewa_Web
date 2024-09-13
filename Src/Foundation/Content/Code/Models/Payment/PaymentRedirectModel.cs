using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace DEWAXP.Foundation.Content.Models.Payment
{
    public class PaymentRedirectModel : PaymentRequestModel
    {

        /// <summary>
        /// PayBill
        /// MoveOut
        /// ServiceActivation
        /// ReraServiceActivation
        /// Clearance
        /// EstimatePayment
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of contract accounts to be paid /premise number
        /// </summary>
        public string c { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of amounts to be paid
        /// </summary>
        public string a { get; set; }

        public string clearancetransactionnumber { get; set; }

        public bool ThirdPartyPayment { get; set; }

        public override bool IsValid()
        {
            if (!string.IsNullOrWhiteSpace(c) && !string.IsNullOrWhiteSpace(a))
            {
                var accounts = c.Split(',');
                var amounts = a.Split(',');

                return accounts.Length == amounts.Length;
            }
            return false;
        }

        public override PaymentContext PaymentContext
        {
            get
            {


                // PayBill
                if (type == "PayBill")
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
                }
                // MoveOut
                else if (type == "MoveOut")
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
                }
                // ServiceActivation
                else if (type == "ServiceActivation")
                {
                    return ThirdPartyPayment ? PaymentContext.AnonymousServiceActivation : PaymentContext.ServiceActivation;
                }
                // ReraServiceActivation
                else if (type == "ReraServiceActivation")
                {
                    return ThirdPartyPayment ? PaymentContext.AnonymousReraServiceActivation : PaymentContext.ReraServiceActivation;
                }
                // Clearance
                else if (type == "Clearance")
                {
                    return ThirdPartyPayment ? PaymentContext.AnonymousClearance : PaymentContext.Clearance;
                }
                // EstimatePayment
                else if (type == "EstimatePayment")
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsEstimate : PaymentContext.PayMyEstimate;
                }
                //TemporaryConnection
                else if (type == "TemporaryConnection")
                {
                    return PaymentContext.TemporaryConnection;
                }
                //Miscellaneous 
                else if (type == "Miscellaneous")
                {
                    return PaymentContext.Miscellaneous;
                }
                //EV card 
                else if (type == "EVCard")
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.EVCard;
                }
                else if (type =="RammasPayment")
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.RammasPayment;
                }
                else
                {
                    return ThirdPartyPayment ? PaymentContext.PayFriendsBill : PaymentContext.PayBill;
                }
            }
        }
        /// <summary>
        /// Gets or sets User id
        /// </summary>
        public string u { get; set; }
        /// <summary>
        /// EPayTransactionCode
        /// </summary>
        public string t { get; set; }
        /// <summary>
        /// Email Address
        /// </summary>
        public string em { get; set; }

        /// <summary>
        /// MobileNumber
        /// </summary>
        public string mb { get; set; }

        /// <summary>
        /// BusinessPartnerNumber
        /// </summary>
        public string b { get; set; }

        /// <summary>
        /// anonymous 
        /// </summary>
        public string pnuser { get; set; }

        /// <summary>
        /// TransactionNumber
        /// </summary>
        public string x { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string emid { get; set; }


        /// <summary>
        /// EstimateNumber
        /// </summary>
        public string est { get; set; }

        /// <summary>
        /// ConsultantBusinessPartnerNumber
        /// </summary>
        public string cons { get; set; }

        /// <summary>
        /// OwnerBusinessPartnerNumber
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// NotificationReference
        /// </summary>
        public string ntf { get; set; }

        public string d { get; set; }
        /// <summary>
        ///  Note [AvdM]:		Nonce (nn) doesn't seem to be validated by E-Pay. 
        ///					While we've written the algorithm for producing the nonce as per their specification,
        ///					they have simply failed to tell us which encoding format we should use to produce the nonce string.
        /// </summary>

        public string nn { get; set; }
        /// <summary>
        /// Language
        /// </summary>
        public string l { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string swal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mto { get; set; }

        /// <summary>
        /// E Pay Url
        /// </summary>
        public string EPayUrl { get; set; }
        /// <summary>
        /// Session id to be passed for loggedin users
        /// </summary>
        public string sxid { get; set; }

        public string branch { get; set; }

        public string epnum { get; set; }
        public string epf { get; set; }
       
        public string vendorid { get; set; }
        public string paymode { get; set; }
    }
}