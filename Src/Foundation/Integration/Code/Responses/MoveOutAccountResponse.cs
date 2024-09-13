using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class MoveOutAccountResponse : BaseResponse
    {
        public decimal ClearanceCharge { get; set; }
        public decimal ClearanceTax { get; set; }
        public decimal ClearanceTotalAmount { get; set; }
        public string TaxRate { get; set; }
        public string IBANNumber { get; set; }

        public string ClearanceRequestNumber { get; set; }

        public string MoveOutRequestNumber { get; set; }
        public List<MoveOutAccountDetailsResponse> AccountDetails { get; set; }

        public static MoveOutAccountResponse From(DewaSvc.validateRefund payload)
        {
            MoveOutAccountResponse typedResponse = null;
            List<MoveOutAccountDetailsResponse> lstAccountDetails = null;
            int responseCode = 0;

            if (payload != null)
            {
                decimal cashAmount = decimal.Zero;
                decimal cashNonSDAmount = decimal.Zero;
                decimal cashSDAmount = decimal.Zero;
                decimal chequeAmount = decimal.Zero;
                decimal chequeNonSDAmount = decimal.Zero;
                decimal chequeSDAmount = decimal.Zero;
                decimal downPaymentAmount = decimal.Zero;
                decimal iBANAmount = decimal.Zero;
                decimal iBANNonSDAmount = decimal.Zero;
                decimal iBANSDAmount = decimal.Zero;
                decimal netAmount = decimal.Zero;
                decimal nonSDAmount = decimal.Zero;
                decimal outStandingAmount = decimal.Zero;
                decimal sdAmount = decimal.Zero;
                decimal clearanceCharge = decimal.Zero;
                decimal clearanceTax = decimal.Zero;
                decimal clearanceTotalAmount = decimal.Zero;

                int.TryParse(payload.responseCode, out responseCode);
                typedResponse = new MoveOutAccountResponse();
                typedResponse.ResponseCode = responseCode;
                typedResponse.Description = payload.description;
                //typedResponse.IBANNumber = payload.ibannumber;
                typedResponse.ClearanceRequestNumber = payload.clearancerequestnumber;
                typedResponse.MoveOutRequestNumber = payload.moveoutrequestnumber;
                Decimal.TryParse(payload.clearanceTotalAmount, out clearanceTotalAmount);
                typedResponse.ClearanceTotalAmount = clearanceTotalAmount;
                Decimal.TryParse(payload.clearanceCharge, out clearanceCharge);
                typedResponse.ClearanceCharge = clearanceCharge;
                Decimal.TryParse(payload.clearanceTax, out clearanceTax);
                typedResponse.ClearanceTax = clearanceTax;
                typedResponse.TaxRate = payload.taxRate;
                lstAccountDetails = new List<MoveOutAccountDetailsResponse>();
                if (payload.ibanvalidlist != null)
                {
                    foreach (var item in payload.ibanvalidlist)
                    {
                        Decimal.TryParse(item.limitednetamountcash, out cashAmount);
                        Decimal.TryParse(item.limitednonsecuredepositamountcash, out cashNonSDAmount);
                        Decimal.TryParse(item.limitedsecuredepositamountcash, out cashSDAmount);
                        Decimal.TryParse(item.limitednetamountcheque, out chequeAmount);
                        Decimal.TryParse(item.limitednonsecuredepositamountcheque, out chequeNonSDAmount);
                        Decimal.TryParse(item.limitedsecuredepositamountcheque, out chequeSDAmount);
                        Decimal.TryParse(item.netamountincludingdownpayment, out downPaymentAmount);
                        Decimal.TryParse(item.limitednetamountiban, out iBANAmount);
                        Decimal.TryParse(item.limitednonsecuredepositamountiban, out iBANNonSDAmount);
                        Decimal.TryParse(item.limitedsecuredepositamountiban, out iBANSDAmount);
                        Decimal.TryParse(item.netamount, out netAmount);
                        Decimal.TryParse(item.nonsecuredepositamount, out nonSDAmount);
                        Decimal.TryParse(item.outstandingamountocollect, out outStandingAmount);
                        Decimal.TryParse(item.securedepositamount, out sdAmount);

                        lstAccountDetails.Add(new MoveOutAccountDetailsResponse
                        {
                            BusinessPartnerNo = item.businesspartner,
                            BusinessPartnerType = item.businesspartnertype,
                            CashAllowedCode = item.okcash,
                            CashAmount = cashAmount,
                            CashNonSDAmount = cashNonSDAmount,
                            CashSDAmount = cashSDAmount,
                            ChequeAllowedCode = item.okcheque,
                            ChequeAmount = chequeAmount,
                            ChequeNonSDAmount = chequeNonSDAmount,
                            ChequeSDAmount = chequeSDAmount,
                            CollectPaymentCode = item.okPaymenttoCollect,
                            ContractAccountNo = item.contractaccount,
                            DownPaymentAmount = downPaymentAmount,
                            IBANAllowedCode = item.okiban,
                            IBANAmount = iBANAmount,
                            IBANNonSDAmount = iBANNonSDAmount,
                            IBANSDAmount = iBANSDAmount,
                            NetAmount = netAmount,
                            NonSDAmount = nonSDAmount,
                            OutStandingAmount = outStandingAmount,
                            PermiseType = item.premisetype,
                            PremiseNo = item.premisenumber,
                            SDAmount = sdAmount,
                            WorkflowInProcess = item.workflowinprocess,
                        });
                    }
                }
                typedResponse.AccountDetails = lstAccountDetails;
            }
            return typedResponse;
        }
    }

    [Serializable]
    public class MoveOutAccountDetailsResponse
    {
        public string BusinessPartnerNo { get; set; }
        public string BusinessPartnerType { get; set; }
        public string ContractAccountNo { get; set; }
        public string Exception { get; set; }
        public decimal CashAmount { get; set; }
        public decimal ChequeAmount { get; set; }
        public decimal IBANAmount { get; set; }
        public decimal CashNonSDAmount { get; set; } //Cash Non Security Deposit Amount
        public decimal ChequeNonSDAmount { get; set; } //Cheque Non Security Deposit Amount
        public decimal IBANNonSDAmount { get; set; } //IBAN Non Security Deposit Amount
        public decimal CashSDAmount { get; set; } //Cash Security Deposit Amount
        public decimal ChequeSDAmount { get; set; } //Cheque Security Deposit Amount
        public decimal IBANSDAmount { get; set; } //IBAN Security Deposit Amount
        public decimal NetAmount { get; set; }
        public decimal DownPaymentAmount { get; set; }
        public decimal NonSDAmount { get; set; }
        public string CashAllowedCode { get; set; }
        public bool IsCashAllowed
        {
            get { return "Y".Equals(CashAllowedCode); }
        }
        public string ChequeAllowedCode { get; set; }
        public bool IsChequeAllowed
        {
            get { return "Y".Equals(ChequeAllowedCode); }
        }
        public string IBANAllowedCode { get; set; }
        public bool IsIBANAllowed
        {
            get { return "Y".Equals(IBANAllowedCode); }
        }
        public decimal OutStandingAmount { get; set; }
        public string PremiseNo { get; set; }
        public string PermiseType { get; set; }
        public decimal SDAmount { get; set; } //Security Deposit Amount
        public string WorkflowInProcess { get; set; }
        public string CollectPaymentCode { get; set; }
        public bool IsCollectPayment
        {
            get { return "Y".Equals(CollectPaymentCode); }
        }

    }
}
