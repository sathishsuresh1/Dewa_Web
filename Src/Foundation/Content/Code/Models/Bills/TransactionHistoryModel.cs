using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Foundation.Content.Models.Bills
{
    public class TransactionHistoryModel
    {
        public TransactionHistoryModel()
        {
            Transactions = new TransactionModel[0];
        }

        public IEnumerable<TransactionModel> Transactions { get; private set; }
        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public bool downloadstmt { get; set; }
        public string sortby { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }

        public static TransactionHistoryModel From(BillHistoryResponse serviceResponse, int page, string sortby)
        {
            bool sortbyflag = string.IsNullOrWhiteSpace(sortby) ? true : false;
            bool invoiceflag = (!string.IsNullOrWhiteSpace(sortby) && sortby.ToLower().Equals("invoice")) ? true : false;
            TransactionHistoryModel model = new TransactionHistoryModel();
            var mergedSet = new List<TransactionModel>();
            model.downloadstmt = false;
            model.sortby = sortby;
            var invoicelist = serviceResponse.invoicelist != null && serviceResponse.invoicelist.Count > 0 ? serviceResponse.invoicelist.Where(x => !string.IsNullOrWhiteSpace(x.Invoiceno)).ToList() : null;
            if (invoicelist != null && invoicelist.Count() > 0)
            {
                model.downloadstmt = true;
            }
            if (sortbyflag)
            {
                if (invoicelist != null && invoicelist.Count() > 0)
                {
                    mergedSet.AddRange(invoicelist.Select(InvoiceModel.FromAPI));
                }
                if (serviceResponse.paymentlist != null && serviceResponse.paymentlist.Count() > 0)
                {
                    mergedSet.AddRange(serviceResponse.paymentlist.Where(x => string.IsNullOrWhiteSpace(x.Chequereturn) || (!string.IsNullOrWhiteSpace(x.Chequereturn) && !x.Chequereturn.Equals("X"))).Select(ReceiptModel.FromAPI));
                }
            }
            else
            {
                if (invoiceflag)
                {
                    if (invoicelist != null && invoicelist.Count() > 0)
                    {
                        mergedSet.AddRange(invoicelist.Select(InvoiceModel.FromAPI));
                    }
                }
                else
                {
                    if (serviceResponse.paymentlist != null && serviceResponse.paymentlist.Count() > 0)
                    {
                        mergedSet.AddRange(serviceResponse.paymentlist.Where(x => string.IsNullOrWhiteSpace(x.Chequereturn) || (!string.IsNullOrWhiteSpace(x.Chequereturn) && !x.Chequereturn.Equals("X"))).Select(ReceiptModel.FromAPI));
                    }
                }
            }

            var Transactions = mergedSet.OrderByDescending(t => t.Date).ThenBy(t => t.DegTransactionReference);
            model.page = page;
            int count = TransactionHistoryConfiguration.TransactionsCount();
            model.totalpage = Pager.CalculateTotalPages(mergedSet.Count(), count);
            model.pagination = model.totalpage > 1 ? true : false;
            model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();
            model.Transactions = Transactions.Skip((page - 1) * count).Take(count);
            return model;
        }

        private static IEnumerable<int> GetPaginationRange(int currentPage, int totalPages)
        {
            const int desiredCount = 5;
            var returnint = new List<int>();

            var start = currentPage - 1;
            var projectedEnd = start + desiredCount;
            if (projectedEnd > totalPages)
            {
                start = start - (projectedEnd - totalPages);
                projectedEnd = totalPages;
            }

            int p = start;
            while (p++ < projectedEnd)
            {
                if (p > 0)
                {
                    returnint.Add(p);
                }
            }
            return returnint;
        }
    }
}