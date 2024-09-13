using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep2Model
    {
        // Constructor
        public TenderBidingStep2Model()
        {
            BidItemList = new List<BidItem>();
        }
        public List<BidItem> BidItemList { get; set; }
        public string TenderBidRefNumber { get; set; }
        public string TenderNumber { get; set; }
        public string SubmitType { get; set; }
        public string BidStatus { get; set; }
        public string successDesc { get; set; }
        public bool Success { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderARDescription { get; set; }
        public string bidMode { get; set; }
        /// <summary>
        /// Gets or sets the totalpage
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether pagination
        /// </summary>
        public bool Pagination { get; set; }
        /// <summary>
        /// Gets or sets the pagenumbers
        /// </summary>
        public IEnumerable<int> Pagenumbers { get; set; }
        public int TotalRecords { get; set; }
    }

    public class BidItem
    {
        public string bidBond { get; set; }
        public string bomComponent { get; set; }
        public string bomItemnumber { get; set; }
        public string componentQuantity { get; set; }
        public string componentUnit { get; set; }
        public string materialDescription { get; set; }
        public string serialNumber { get; set; }
        public string netPrice { get; set; }
        public string netValue { get; set; }
        public string storageLocation { get; set; }
        public string totalValue { get; set; }
        public string vatAmount { get; set; }
    }
}