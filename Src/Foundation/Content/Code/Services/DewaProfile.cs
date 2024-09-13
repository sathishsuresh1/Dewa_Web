using DEWAXP.Foundation.DI;
using System;

namespace DEWAXP.Foundation.Content.Services
{
    [Service(Lifetime =Lifetime.Transient)]
    [Serializable]
    public class DewaProfile
    {
        public DewaProfile()
        {
            Name = string.Empty;
            PrimaryAccount = string.Empty;
            EmailAddress = string.Empty;
            MobileNumber = string.Empty;
            Role = Roles.User;
        }

        public DewaProfile(string username, string sessionToken)
            : this(username, sessionToken, Roles.User)
        {

        }

        public DewaProfile(string username, string sessionToken, string role)
            : this()
        {
            Username = username;
            SessionToken = sessionToken;
            Role = role;
        }

        public virtual string UserId
        {
            get
            {
                if (IsMyIdUser && !string.IsNullOrWhiteSpace(EmiratesIdentifier))
                {
                    return string.Format("MYID{0}", EmiratesIdentifier);
                }
                return Username;
            }
        }
        public virtual string encryptedSessionparam { get; set; }

        public virtual string Username { get; private set; }

        public virtual string SessionToken { get; private set; }

        public virtual string EmiratesIdentifier { get; set; }

        public bool IsMyIdUser { get; set; }

        public virtual string Name { get; set; }
        public virtual string FullName { get; set; }

        public virtual string PrimaryAccount { get; set; }

        public virtual string BusinessPartner { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string MobileNumber { get; set; }

        public virtual string Role { get; set; }

        public virtual string VatNumber { get; set; }

        public bool HasActiveAccounts { get; set; }
        public bool IsEVUser { get; set; }
        public bool HasPrimaryAccount
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PrimaryAccount);
            }
        }

        public static DewaProfile Null
        {
            get
            {
                return new DewaProfile(string.Empty, string.Empty, Roles.User);
            }
        }

        public virtual string TermsAndConditions { get; set; }
        public bool AcceptedTerms
        {
            get
            {
                return !String.IsNullOrWhiteSpace(TermsAndConditions);
            }
        }
        public bool IsContactUpdated { get; set; }

        public bool IsFirstRegistration { get; set; }
        public bool PopupFlag { get; set; }
        public bool IsUSC { get; set; } = false;
        public bool IsRegister { get; set; }
        public virtual string CustomerNo { get; set; }
        public virtual string CustomerType { get; set; }
        public virtual string LastLogin {get; set;}
    }
}