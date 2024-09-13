using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.DewaSvc;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.Refund
{
    public class RefundHistoryModel
    {
        public RefundHistoryModel()
        {
            Refunds = new RefundModel[0];
        }

        public IEnumerable<RefundModel> Refunds { get; private set; }
        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public string sortby { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }

        public static RefundHistoryModel From(refundHistory refundHistory, int page, string sortby)
        {
            if (refundHistory.refundlist != null)
            {
                bool sortbyflag = string.IsNullOrWhiteSpace(sortby) ? true : false;
                bool invoiceflag = (!string.IsNullOrWhiteSpace(sortby) && sortby.ToLower().Equals("invoice")) ? true : false;
                RefundHistoryModel model = new RefundHistoryModel();
                var mergedSet = new List<RefundModel>();
                var lstdatasource = GetLstDataSource(DataSources.REFUNDHISTORY_FILTERS).ToList();
                var lstaccounttransfer = refundHistory.refundstatuslist.Where(x => x.refundprocesstype == "ATRF").OrderBy(x => x.serialnumber).Select(c => new SelectListItem() { Text = c.statusdescription, Value = c.refundstatus }).ToList();//GetLstDataSource(DataSources.ACCOUNTTRANSFERSTATUS).ToList();
                var lstcheque = refundHistory.refundstatuslist.Where(x => x.refundprocesstype == "CQRF").OrderBy(x => x.serialnumber).Select(c => new SelectListItem() { Text = c.statusdescription, Value = c.refundstatus }).ToList();//GetLstDataSource(DataSources.CHEQUESTATUS).ToList();
                var lstibanstatus = refundHistory.refundstatuslist.Where(x => x.refundprocesstype == "IBRF").OrderBy(x => x.serialnumber).Select(c => new SelectListItem() { Text = c.statusdescription, Value = c.refundstatus }).ToList();//GetLstDataSource(DataSources.IBANSTATUS).ToList();
                var lstwesternunion = refundHistory.refundstatuslist.Where(x => x.refundprocesstype == "EWRF").OrderBy(x => x.serialnumber).Select(c => new SelectListItem() { Text = c.statusdescription, Value = c.refundstatus }).ToList(); //GetLstDataSource(DataSources.WESTERNUNIONSTATUS).ToList();
                var lstrefunds = refundHistory.refundlist.ToList();
                if (!sortbyflag)
                {
                    lstrefunds = lstrefunds.Where(x => x.refundtype.Equals(sortby)).ToList();
                }
                mergedSet.AddRange(lstrefunds.Select(x => RefundModel.From(x, lstdatasource, lstaccounttransfer, lstcheque, lstibanstatus, lstwesternunion)));
                model.sortby = sortby;

                var Transactions = mergedSet.OrderByDescending(t => t.Date);
                model.page = page;
                int count = TransactionHistoryConfiguration.RefundCount();
                model.totalpage = Pager.CalculateTotalPages(mergedSet.Count(), count);
                model.pagination = model.totalpage > 1 ? true : false;
                model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();
                model.Refunds = Transactions.Skip((page - 1) * count).Take(count);
                return model;
            }
            return null;
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

        public ibanListchild[] IbanValues { get; set; }
        #region

        public static IEnumerable<SelectListItem> GetLstDataSource(string datasource)
        {
            try
            {
                IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
                var dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(datasource));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Datasource");
            }
        }

        #endregion
    }
}