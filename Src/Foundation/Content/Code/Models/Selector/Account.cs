using System;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models
{
    [Serializable]
    public class Account
    {
        public string AccountNumber { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string PremiseNumber { get; set; }

        public string CustomerPremiseNumber { get; set; }

        public string CustomerType { get; set; }

        public string NotificationNumber { get; set; }

        public string Category { get; set; }

        public string AccountCategory { get; set; }

        public string Name { get; set; }

        public string NickName { get; set; }

        public decimal ElectricityBill { get; set; }

        public decimal WaterBill { get; set; }

        public decimal HoldingFees { get; set; }

        public decimal SewerageFees { get; set; }

        public decimal CoolingCharges { get; set; }
        public decimal DMCharges { get; set; }

        public decimal OtherCharges { get; set; }

        public decimal Balance { get; set; }

        public decimal HousingFees { get; set; }

        public decimal ElectedPaymentAmount { get; set; }

        public bool Active { get; set; }

        public bool Primary { get; set; }

        public bool Selected { get; set; }

        public string BillingClass { get; set; }

        public string BillingClassTemp { get; set; }

        public bool HasPhoto { get; set; }

        public bool PartialPaymentPermitted { get; set; }

        public AccountClassification AccountClass { get; set; }
        public string Street { get; set; }

        public string Location { get; set; }
        public string XCordinate { get; set; }
        public string YCordinate { get; set; }

        public bool HasAccounts { get; set; }

        public bool POD { get; set; }
        public bool Medical { get; set; }
        public bool Senior { get; set; }

        public BillingClassification BillingClassification { get; set; }

        public static Account Null
        {
            get
            {
                return new Account
                {
                    Name = Translate.Text("No accounts available"),
                    NickName = Translate.Text("No accounts available"),
                    AccountNumber = "-",
                    PremiseNumber = "-",
                    BillingClass = "-",
                    BusinessPartnerNumber = "-",
                    Active = false,
                    PartialPaymentPermitted = false,
                    HasPhoto = false,
                    HasAccounts = false,
                    BillingClassification = BillingClassification.Unknown,
                    POD = false,
                    Medical = false,
                    Senior = false,
                    XCordinate = string.Empty,
                    YCordinate = string.Empty,
                };
            }
        }

        public static Account From(AccountDetails response)
        {
            return new Account
            {
                AccountNumber = response.AccountNumber,
                BusinessPartnerNumber = response.BusinessPartnerNumber,
                Name = response.AccountName,
                PremiseNumber = response.PremiseNumber,
                Category = response.Category,
                CustomerPremiseNumber = response.CustomerPremiseNumber,
                CustomerType = !string.IsNullOrWhiteSpace(response.CustomerType) ? response.CustomerType.ToLower().Equals("o") ? Translate.Text("moveout.owner") : Translate.Text("moveout.tenant") : string.Empty,
                NotificationNumber = response.NotificationNumber,
                NickName = response.NickName,
                ElectricityBill = response.Electricity,
                WaterBill = response.Water,
                SewerageFees = response.Sewerage,
                CoolingCharges = response.Cooling,
                DMCharges = response.DM,
                OtherCharges = response.Others,
                HoldingFees = response.DM,
                HousingFees = response.Housing,
                Balance = response.Balance,
                Active = response.IsActive,
                AccountClass = (response.AccountStatusCode == null) ? AccountClassification.Unknown : GetAccountClass(response.AccountStatusCode),
                BillingClass = (response.BillingClass == BillingClassification.Residential) ? Translate.Text("Residential") : (response.BillingClass == BillingClassification.ElectricVehicle) ? Translate.Text("Electric Vehicle") : (response.BillingClass == BillingClassification.NonResidential) ? Translate.Text("Non-residential") : "-",
                BillingClassTemp = (response.BillingClass == BillingClassification.Residential || response.BillingClass == BillingClassification.ElectricVehicle || response.BillingClass == BillingClassification.NonResidential) ? response.BillingClass.ToString() : "-",
                HasPhoto = response.HasPhoto,
                PartialPaymentPermitted = response.PartialPaymentPermitted,
                ElectedPaymentAmount = response.Balance > 0 ? response.Balance : 0.00m,
                AccountCategory = response.AccountCategory,
                Street = response.Street,
                Location = response.Location,
                HasAccounts = true,
                BillingClassification = response.BillingClass,
                POD = response.POD,
                Medical = response.Medical,
                Senior = response.Senior,
                XCordinate = response.XCordinate,
                YCordinate = response.YCordinate
            };
        }

        public static Account From(BillEnquiryResponse response)
        {
            return new Account
            {
                AccountNumber = response.AccountNumber,
                WaterBill = response.Water,
                ElectricityBill = response.Electricity,
                DMCharges = response.DM,
                CoolingCharges = response.Cooling,
                Balance = response.Balance,
                ElectedPaymentAmount = response.Balance > 0 ? response.Balance : 0.00m,
                Name = response.Name,
                OtherCharges = response.Other,
                NickName = response.Nickname,
                SewerageFees = response.Sewerage,
                HousingFees = response.Housing,
                PartialPaymentPermitted = response.PartialPaymentPermitted,
                HasAccounts = true
            };
        }

        public static Account From(Bill response)
        {
            return new Account
            {
                AccountNumber = response.AccountNumber,
                WaterBill = response.Water,
                DMCharges = response.DM,
                ElectricityBill = response.Electricity,
                CoolingCharges = response.Cooling,
                Balance = response.Total,
                ElectedPaymentAmount = response.Total > 0 ? response.Total : 0.00m,
                Name = response.Name,
                OtherCharges = response.Other,
                NickName = response.Nickname,
                SewerageFees = response.Sewerage,
                HousingFees = response.Housing,
                PartialPaymentPermitted = response.PartialPaymentPermitted,
                HasAccounts = true
            };
        }

        public static AccountClassification GetAccountClass(string accountStatusCode)
        {
            AccountClassification accountClass = AccountClassification.Unknown;
            accountStatusCode = (accountStatusCode == string.Empty) ? "-1" : accountStatusCode;
            if (accountStatusCode != null)
                DataSources.AccountStatus.TryGetValue(int.Parse(accountStatusCode), out accountClass);

            return accountClass;
        }
    }
}