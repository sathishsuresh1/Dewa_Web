using DEWAXP.Foundation.Content.Models.Payment;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Serializable]
    public class ApplyEVCard : EVIndividualModel
    {
        public string BPCategoryType { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string BusinessPartnerDetails { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string POBox { get; set; }
        public string POBoxregion { get; set; }
        public string NoofCards { get; set; }
        public string mulkiyanum { get; set; }
        public string CardIdType { get; set; }
        public string CardIdNumber { get; set; }
        public string mulkiya { get; set; }
        public string IdNumber { get; set; }
        public string IdType { get; set; }
        public string processtFlag { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument2 { get; set; }

        public byte[] AttachmentFileBinary { get; set; }
        public string AttachmentFileType { get; set; }
        public byte[] AttachmentFileBinary2 { get; set; }
        public string AttachmentFileType2 { get; set; }
        public string CarPlateNumberList { get; set; }
        public string CarPlateCodeList { get; set; }
        public string CarCategoryList { get; set; }
        public decimal PayingAmount { get; set; }
        public string SubmittedCarPlateNumbers { get; internal set; }
        public string SubmittedCarPlateCodes { get; internal set; }
        public string SubmittedCarCategories { get; internal set; }

        public string CarDetailJs { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
        public string bankkey { get; set; }
    }

    public class EVIndividualModel
    {
        public string CategoryCode { get; set; }
        public string PlateCode { get; set; }
        public string TCNumber { get; set; }
        public string PlateNumber { get; set; }
        public string CarRegisteredIn { get; set; }
        public string EmirateOrCountry { get; set; }
        public string TrafficCodeNumber { get; set; }
        public string CarPlateNumber { get; set; }
        public string AccountType { get; set; }
        public Step RegistrationStage { get; set; }

        //public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Emirates { get; set; }

        public string EmirateinDetail { get; set; }

        public string GetCarRegisteredRegion()
        {
            return this.CarRegisteredIn == "1" ? this.EmirateOrCountry : "";
        }

        public string GetCarRegisteredCountry()
        {
            return this.CarRegisteredIn == "1" ? "AE" : this.EmirateOrCountry;
        }

        public List<EvCardPaymentDetail> EvCardPaymentDetail { get; set; }

        public string UserId { get; set; }
    }

    //public class EvCardPaymentDetail
    //{
    //    public string accountNumber { get; set; }
    //    public string amount1 { get; set; }
    //    public string amount2 { get; set; }
    //    public string courierCharge { get; set; }
    //    public string courierVatAmount { get; set; }
    //    public string evCardNumber { get; set; }
    //    public string sdAmount { get; set; }
    //    public string totalAmount { get; set; }
    //}

    public enum Step
    {
        NOT_DEFINED = 0,
        STEP_ONE = 1,
        STEP_TWO = 2,
        STEP_THREE = 3
    }
}