using DEWAXP.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;

namespace DEWAXP.Foundation.Helpers
{
    public static class ReraHelper
    {
        /// <summary>
        /// Return business parnet number and contract number from encrypted rera qs
        /// </summary>
        /// <param name="qs"></param>
        /// <returns>BusinessPartner|ContractAccount</returns>
        public static string GetReraValuesFromQuerystring(string qs)
        {
            //first convert to base 64
            var decbpnum = Base64Decode(qs);

            if (string.IsNullOrEmpty(decbpnum))
                return null;

            string deco = decbpnum;
            var sb = new StringBuilder();
            var temp = new StringBuilder();

            for (int i = 0; i < deco.Length - 1; i += 2)
            {
                string output = deco.Substring(i, 2);
                int decimalN = Convert.ToByte(output, 16);
                sb.Append((char)decimalN);
            }

            decbpnum = sb.ToString();
            var in458 = new BigInteger(458);
            var by458 = BigInteger.Divide(BigInteger.Parse(decbpnum), in458);
            var by458min6 = BigInteger.Parse(by458.ToString().Substring(0, by458.ToString().Length - 5));

            var ran6 = BigInteger.Parse(by458.ToString().Substring(by458.ToString().Length - 5));

            var by458min6byran6 = BigInteger.Divide(by458min6, ran6);
            var bpnum = by458min6byran6.ToString().Substring(0, 8);
            var canum = by458min6byran6.ToString().Substring(8);

            return string.Format("{0}|{1}", bpnum, canum);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, null);
            }
            return string.Empty;
        }
    }
}