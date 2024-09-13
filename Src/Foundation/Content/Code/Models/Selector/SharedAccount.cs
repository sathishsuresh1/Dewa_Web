using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models
{
    [Serializable]
    public class SharedAccount
    {
        public string AccountNumber { get; set; }

        public string Name { get; set; }
        public string NickName { get; set; }

        public bool Active { get; set; }

        public string Type { get; set; }

        public string Premise { get; set; }

        public string InternalPremise { get; set; }

        public string BusinessPartner { get; set; }

        public string ImageUrl { get; set; }

        public bool PartialPaymentPermitted { get; set; }

        public string BillingClass { get; set; }

        public bool ShowAccountSummary { get; set; }

        public string IbanAccountNumber { get; set; }
        public string Street { get; set; }
        public string Location { get; set; }

        public BillingClassification BillingClassification { get; set; }

        public bool LinkedToIbanAccount
        {
            get { return !string.IsNullOrWhiteSpace(IbanAccountNumber); }
        }

        public static SharedAccount CreateFrom(AccountDetails source)
        {
            if (source != null)
            {
                return new SharedAccount
                {
                    Name = !string.IsNullOrWhiteSpace(source.NickName) ? source.NickName : source.AccountName,
                    AccountNumber = source.AccountNumber,
                    Active = source.IsActive,
                    BusinessPartner = source.BusinessPartnerNumber,
                    Type = source.Category,
                    ImageUrl = source.HasPhoto ? string.Format("/account_thumbs.ashx?id={0}", source.AccountNumber) : null,
                    Premise = source.CustomerPremiseNumber,
                    InternalPremise = source.PremiseNumber,
                    PartialPaymentPermitted = source.PartialPaymentPermitted,
                    BillingClass = (source.BillingClass == BillingClassification.Residential) ? Translate.Text("Residential") : (source.BillingClass == BillingClassification.ElectricVehicle) ? Translate.Text("Electric Vehicle") : Translate.Text("Non-residential"),
                    ShowAccountSummary = false,
                    BillingClassification = source.BillingClass
                };
            }
            return null;
        }

        public static SharedAccount[] CreateForMoveOut(List<AccountDetails> source, List<string> moveOutAccounts)
        {
            if (source != null && source.Count > 0)
            {
                List<SharedAccount> selectedAccounts = new List<SharedAccount>();

                foreach (var account in source)
                {
                    if (moveOutAccounts.Contains("00" + account.AccountNumber))
                    {
                        selectedAccounts.Add(new SharedAccount
                        {
                            Name = !string.IsNullOrWhiteSpace(account.NickName) ? account.NickName : account.AccountName,
                            AccountNumber = "00" + account.AccountNumber,
                            Active = account.IsActive,
                            BusinessPartner = account.BusinessPartnerNumber,
                            Type = account.Category,
                            ImageUrl = account.HasPhoto ? string.Format("/account_thumbs.ashx?id={0}", account.AccountNumber) : null,
                            Premise = account.CustomerPremiseNumber,
                            InternalPremise = account.PremiseNumber,
                            PartialPaymentPermitted = account.PartialPaymentPermitted,
                            BillingClass = (account.BillingClass == BillingClassification.Residential) ? Translate.Text("Residential") : (account.BillingClass == BillingClassification.ElectricVehicle) ? Translate.Text("Electric Vehicle") : (account.BillingClass == BillingClassification.NonResidential) ? Translate.Text("Non-residential") : "-",
                            ShowAccountSummary = true,
                            BillingClassification = account.BillingClass
                        });
                    }
                }

                return selectedAccounts.ToArray();
            }
            return null;
        }
    }
}