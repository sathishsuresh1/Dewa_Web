// <copyright file="AccountListModel.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using DEWAXP.Foundation.Integration.GatePassSvc;
    using X.PagedList;

    /// <summary>
    /// Defines the <see cref="AccountListModel" />
    /// </summary>
    public class AccountListModel
    {
        /// <summary>
        /// Gets or sets the Search
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Gets or sets the PageNo
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or sets the ITEMPagedList
        /// </summary>
        public IPagedList<poDetails> ITEMPagedList { get; set; }

        /// <summary>
        /// Gets or sets the supplierid
        /// </summary>
        public string supplierid { get; set; }

        /// <summary>
        /// Gets or sets the suppliername
        /// </summary>
        public string suppliername { get; set; }

        /// <summary>
        /// Gets or sets the suppliertelephonenumber
        /// </summary>
        public string suppliertelephonenumber { get; set; }
    }
}
