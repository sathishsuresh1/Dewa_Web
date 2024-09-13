using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    public class EVTransactionListViewModel
    {
        public EVTransactionListViewModel()
        {
            transactionDetails = new List<EVTransactionsDetail>();
        }

        public List<EVTransactionsDetail> transactionDetails { get; private set; }
        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }
        public string discountamount { get; set; }
        public string salesunit { get; set; }

        public string sddcoumentcurrency { get; set; }

        
        public string totalconsumption { get; set; }

       
        public string totalcost { get; set; }


        public static EVTransactionListViewModel From(EVTransactionalResponse eVTransactional,bool dashboard, int page,string sortby)
        {
            EVTransactionListViewModel model = new EVTransactionListViewModel();
            if (eVTransactional != null)
            {
                model.discountamount = eVTransactional.Discountamount;
                model.salesunit = eVTransactional.Salesunit;
                model.sddcoumentcurrency = eVTransactional.Sddcoumentcurrency;
                model.totalconsumption = eVTransactional.Totalconsumption;
                model.totalcost = eVTransactional.Totalcost;
                if (eVTransactional.Transactiondetails != null && eVTransactional.Transactiondetails.Count > 0)
                {
                    eVTransactional.Transactiondetails.ForEach(x => model.transactionDetails.Add(new EVTransactionsDetail
                    {
                        amount= x.Amount,
                        cardnumber=x.Cardnumber,
                        consumption=x.Consumption,
                        descriptionforcharges= x.Descriptionforcharges,
                        discount =x.Discount,
                        duration=x.Duration,
                        locationname= x.Locationname,
                        platenumber = x.Platenumber,
                        salesunit=x.Salesunit,
                        sddocumentcurrency=x.Sddocumentcurrency,
                        durationinseconds= x.Durationinseconds,
                        transactiondate= DateTime.ParseExact(x.Transactiondate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", global::Sitecore.Context.Culture),
                        strtransactiondate= DateTime.ParseExact(x.Transactiondate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                        transactionid =x.Transactionid,
                        transactiontime=x.Transactiontime
                    }));
                }
                if(!string.IsNullOrWhiteSpace(sortby))
                {
                    if(sortby.ToLower().Equals("date"))
                    {
                        model.transactionDetails = model.transactionDetails.OrderByDescending(x => x.Dttransactiondate).ToList();
                    }
                    else if (sortby.ToLower().Equals("duration"))
                    {
                        model.transactionDetails = model.transactionDetails.OrderBy(x => x.durationinseconds).ToList();
                    }
                }
            }
            
            model.page = page;
            if (dashboard)
            {
                model.totalpage = 1;
                model.pagination = false;
                model.transactionDetails = model.transactionDetails.Take(5).ToList();
            }
            else
            {
                int count = 10;
                model.totalpage = Pager.CalculateTotalPages(model.transactionDetails.Count(), count);
                model.pagination = model.totalpage > 1 ? true : false;
                model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();
                model.transactionDetails = model.transactionDetails.Skip((page - 1) * count).Take(count).ToList();
            }
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
    
    public class EVTransactionsDetail
{
        public string amount { get; set; }

        public string cardnumber { get; set; }

        
        public string consumption { get; set; }

       
        public string descriptionforcharges { get; set; }

        
        public string discount { get; set; }

        public string duration { get; set; }
        public int durationinseconds { get; set; }

        
        public string locationname { get; set; }

        
        public string platenumber { get; set; }

       
        public string salesunit { get; set; }

        
        public string sddocumentcurrency { get; set; }

        
        public string transactiondate { get; set; }
        public string strtransactiondate { get; set; }
        public DateTime Dttransactiondate => DateTime.ParseExact(strtransactiondate, "dd MMM yyyy", CultureInfo.InvariantCulture).Add(TimeSpan.Parse(transactiontime));

        
        public string transactionid { get; set; }

        
        public string transactiontime { get; set; }
    }
}