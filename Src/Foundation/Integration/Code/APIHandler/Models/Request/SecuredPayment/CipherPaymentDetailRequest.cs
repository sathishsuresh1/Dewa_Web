using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SecuredPayment
{
    public class CipherPaymentDetailRequest
    {
        public CipherPaymentDetailInputs paymentparams { get; set; }
    }

    public class CipherPaymentDetailInputs
    {
        /// <summary>
        /// User id
        /// <para/>Old Name : u
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// Contract Accounts . Comma separated incase of multiple Eg: 20012345678, 20012345679
        /// <para/><para/>Old Name : c
        /// </summary>
        public string contractaccounts { get; set; }
        /// <summary>
        /// Amounts Comma separated incase of multiple Eg: 500, 7
        /// <para/>Old Name : a
        /// </summary>
        public string amounts { get; set; }
        /// <summary>
        /// Session id
        /// <para/>Old Name : sxid
        /// </summary>
        public string sessionid { get; set; }
        /// <summary>
        /// Business partner
        /// <para/>Old Name : b
        /// </summary>
        public string businesspartner { get; set; }
        /// <summary>
        /// Easy payflag
        /// <para/>Old Name : epf
        /// </summary>
        public string easypayflag { get; set; }
        /// <summary>
        /// Easypay number
        /// <para/>Old Name : epnum
        /// </summary>
        public string easypaynumber { get; set; }
        /// <summary>
        /// Branch
        /// <para/>Old Name : branch
        /// </summary>
        public string branch { get; set; }
        /// <summary>
        /// X coordinate
        /// <para/>Old Name : xcord
        /// </summary>
        public string xcoordinate { get; set; }
        /// <summary>
        /// Y coordinate
        /// <para/>Old Name : ycord
        /// </summary>
        public string ycoordinate { get; set; }
        /// <summary>
        /// Moveout flag
        /// <para/>Old Name : mout
        /// </summary>
        public string moveoutflag { get; set; }
        /// <summary>
        /// Moveto flag
        /// <para/>Old Name : mto
        /// </summary>
        public string movetoflag { get; set; }
        /// <summary>
        /// Redirect flag
        /// <para/>Old Name : rd
        /// </summary>
        public string redirectflag { get; set; }
        /// <summary>
        /// Mobile redirect flag
        /// <para/>Old Name : mr
        /// </summary>
        public string mobileredirectflag { get; set; }
        /// <summary>
        /// Tradelicence number
        /// <para/>Old Name : tlic
        /// </summary>
        public string tradelicencenumber { get; set; }
        /// <summary>
        /// Emirates id
        /// <para/>Old Name : u
        /// </summary>
        public string emiratesid { get; set; }
        /// <summary>
        /// Estimate number
        /// <para/>Old Name : est
        /// </summary>
        public string estimatenumber { get; set; }
        /// <summary>
        /// Transaction type. This will be shared later and are specific to the Payment Types
        /// <para/>Old Name : t
        /// </summary>
        public string transactiontype { get; set; }
        /// <summary>
        /// Lang, Eg. EN or AR
        /// <para/>Old Name : l / lang
        /// </summary>
        public string lang { get; set; }
        /// <summary>
        /// Email
        /// <para/>Old Name : em
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Clearance transaction
        /// <para/>Old Name : x
        /// </summary>
        public string clearancetransaction { get; set; }
        /// <summary>
        /// Mobile Number 
        /// <para/>Old Name : mb
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// Owner business partner number
        /// <para/>Old Name : owner
        /// </summary>
        public string ownerbusinesspartnernumber { get; set; }
        /// <summary>
        /// Consultant businesspartner number
        /// <para/>Old Name : cons
        /// </summary>
        public string consultantbusinesspartnernumber { get; set; }
        /// <summary>
        /// Notification number 
        /// <para/>Old Name : ntf
        /// </summary>
        public string notificationnumber { get; set; }
        /// <summary>
        /// Tender device
        /// <para/>Old Name : dev
        /// </summary>
        public string tenderdevice { get; set; }
        /// <summary>
        /// vendorid
        /// <para/>Old Name : v
        /// </summary>
        public string vendorid { get; set; }
        /// <summary>
        /// User id
        /// <para/>Old Name : key1=val1,key2=val2
        /// </summary>
        public string otherkeyvalue1 { get; set; }
        /// <summary>
        /// contractaccounts
        /// <para/>Old Name : key3=val3,key4=val4
        /// </summary>
        public string otherkeyvalue2 { get; set; }
        /// <summary>
        /// paymode
        /// <para/>Old Name : paymode
        /// </summary>
        public string paymode { get; set; }
        /// <summary>
        /// d
        /// <para/>Old Name : d
        /// </summary>
        public string tendertransactionid { get; set; }
        /// <summary>
        /// pnusr
        /// <para/>Old Name : pnusr
        /// </summary>
        public string pnusr { get; set; }
        /// <summary>
        /// bank key as par of netbanking method
        /// </summary>
        public string bk { get; set; }
        /// <summary>
        /// suqia donation amount
        /// </summary>
        public string suqiaamt { get; set; }
    }
}
