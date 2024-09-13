using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.EasyPay
{
    public class BeneficiaryDetail
    {
        public string ContractAccount { get; set; }
        public string Name { get; set; }
    }

    public enum BeneficiaryManagemode
    {
        /// <summary>
        /// Get
        /// </summary>
        G = 0,
        /// <summary>
        /// Add
        /// </summary>
        A = 1,
        /// <summary>
        /// Edit
        /// </summary>
        E = 2,
        /// <summary>
        /// Delete
        /// </summary>
        D = 3

    }
}