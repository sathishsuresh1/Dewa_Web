// <copyright file="AccountListModel.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.WorkPermit
{
    using DEWAXP.Foundation.Integration.Responses.VendorSvc;
    using X.PagedList;

    /// <summary>
    /// Defines the <see cref="AccountListModel" />
    /// </summary>
    public class PoDetailsListModel
    {
        /// <summary>
        /// Gets or sets the Search
        /// </summary>
        public string Search { get; set; }

        public string SelectedAccount { get; set; }

        /// <summary>
        /// Gets or sets the PageNo
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or sets the ITEMPagedList
        /// </summary>
        public IPagedList<POITEM> ITEMPagedList { get; set; }

       
    }
}
