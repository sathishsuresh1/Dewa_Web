using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ChatBot.Models.DirectLine
{
    public static class Constants
    {
        /*public static List<string> LevelOneResponse = new List<string>() { CONSUMER, BUILDER, SUPPLIER, PARTNER, STUDENT, ABOUTUS, BILLPAYMENT, PAYFORFRIEND, TRACAPPLICATION, APPLYFORAJOB, VIEWBILL, TRACKELECWATERCON, EASYPAY, TRENDINGARTICLES, DEWASTORE };*/

        public const string CONSUMER = "consumer"; //1
        public const string CONSUMER_AR = "مستهلك";
        public const string BUILDER = "builder"; //2
        public const string BUILDER_AR = "باني مشروع";
        public const string SUPPLIER = "supplier"; //3
        public const string SUPPLIER_AR = "مورد";
        public const string PARTNER = "partner"; //4
        public const string PARTNER_AR = "شريك";
        public const string STUDENT = "student"; //5
        public const string STUDENT_AR = "طالب";
        public const string ABOUTUS = "about us"; //6
        public const string ABOUTUS_AR = "عن الهيئة";
        public const string BILLPAYMENT = "bill payment"; //7
        public const string BILLPAYMENT_AR = "دفع الفاتورة";
        public const string PAYFORFRIEND = "pay for a friend"; //8
        public const string PAYFORFRIEND_AR = "الدفع عن صديق";
        public const string TRACAPPLICATION = "track application"; //9
        public const string TRACAPPLICATION_AR = "تتبع الطلبات";
        public const string APPLYFORAJOB = "apply for a job"; //10
        public const string APPLYFORAJOB_AR = "تقديم على وظيفة";
        public const string VIEWBILL = "view bill"; //11
        public const string VIEWBILL_AR = "عرض الفاتورة";
        public const string TRACKBUILDERSERVICES = "track builder services"; //12
        public const string TRACKBUILDERSERVICES_AR = "تتبع طلبات بناة المشاريع";
        public const string EASYPAY = "easy pay"; //13
        public const string EASYPAY_AR = "الدفع السهل";
        public const string TRENDINGARTICLES = "trending articles"; //14
        public const string TRENDINGARTICLES_AR = "المقالات الرائجة";
        public const string DEWASTORE = "dewa store"; //15
        public const string DEWASTORE_AR = "متجر ديوا";
        public const string OnDemandSurvey = "startoverondemandsurvey"; //16
        public const string CustomerServeyTesting = "customer survey testing"; //16
        public const string CUSTOMERSURVEYTESTING_AR = "اختبار استطلاع العملاء";
        public const string EASYPAY_VAL_REGEX = @"([^\d]|^)\d{6,10}([^\d]|$)";

        public const string SHOWMORE = "View more offers";
        public const string SHOWMORE_AR = "إظهار المزيد";
        public static string[] ToValArray(this string val, char splitter = '_')
        {
            return string.IsNullOrEmpty(val) ? new string[] { "" } : val.Split(splitter);
        }
        public static bool IsRootLevel(this string s, out byte o)
        {
            o = 0;
            switch (s.ToLower())
            {
                case CONSUMER:
                case CONSUMER_AR:
                    o = 1;
                    return true;
                case BUILDER:
                case BUILDER_AR:
                    o = 2;
                    return true;
                case SUPPLIER:
                case SUPPLIER_AR:
                    o = 3;
                    return true;
                case PARTNER:
                case PARTNER_AR:
                    o = 4;
                    return true;
                case STUDENT:
                case STUDENT_AR:
                    o = 5;
                    return true;
                case ABOUTUS:
                case ABOUTUS_AR:
                    o = 6;
                    return true;
                case BILLPAYMENT:
                case BILLPAYMENT_AR:
                    o = 7;
                    return true;
                case PAYFORFRIEND:
                case PAYFORFRIEND_AR:
                    o = 8;
                    return true;
                case TRACAPPLICATION:
                case TRACAPPLICATION_AR:
                    o = 9;
                    return true;
                case APPLYFORAJOB:
                case APPLYFORAJOB_AR:
                    o = 10;
                    return true;
                case VIEWBILL:
                case VIEWBILL_AR:
                    o = 11;
                    return true;
                case TRACKBUILDERSERVICES:
                case TRACKBUILDERSERVICES_AR:
                    o = 12;
                    return true;
                case EASYPAY:
                case EASYPAY_AR:
                    o = 13;
                    return true;
                case TRENDINGARTICLES:
                case TRENDINGARTICLES_AR:
                    o = 14;
                    return true;
                case DEWASTORE:
                case DEWASTORE_AR:
                    o = 15;
                    return true;
                case OnDemandSurvey:
                    o = 16;
                    return true;
                case CustomerServeyTesting:
                case CUSTOMERSURVEYTESTING_AR:
                case "المشاركة في الاستبيان":
                case "take survey":
                    o = 18;
                    return true;
                default:
                    return false;

            }
        }
        public static byte GetPreviousLevel(this byte b)
        {
            switch (b)
            {

                case 131:
                    return 13;
                case 151:
                    return 15;
                case 161:
                    return 16;
                default:
                    return 0;
            }
            //return b < 100 ? Convert.ToByte(b + 100) : Convert.ToByte(b + 1);
        }
        public static byte GetNextLevel(this byte b)
        {
            switch (b)
            {
                case 13:
                    return 131;
                case 131:
                    return 132;
                case 15:
                    return 151;
                case 151:
                    return 152;
                case 16:
                    return 161;
                case 161:
                    return 162;
                default:
                    return b;
            }
            //return b < 100 ? Convert.ToByte(b + 100) : Convert.ToByte(b + 1);
        }
        public static bool IsValidEPayNo(this string n)
        {

            var rx = new System.Text.RegularExpressions.Regex(EASYPAY_VAL_REGEX, System.Text.RegularExpressions.RegexOptions.Compiled);
            return string.IsNullOrEmpty(n) ? false : rx.IsMatch(n);
        }

        public static string EncodeAstarics(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.StartsWith("**")) { return s; }
            if (s.Contains("***"))
            {
                return s.Replace("*", "&#42;");
            }
            return s;
        }
    }

}