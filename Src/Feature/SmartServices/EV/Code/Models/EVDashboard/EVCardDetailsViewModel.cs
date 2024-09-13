using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    public class EVCardDetailsViewModel
    {
        public EVCardDetailsViewModel()
        {
            eVCardDetails = new List<EVCardDetailModel>();
        }

        public List<EVCardDetailModel> eVCardDetails { get; private set; }
        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }

        public static EVCardDetailsViewModel From(List<Evcarddetail> evcarddetails, int page, string keyword)
        {
            EVCardDetailsViewModel model = new EVCardDetailsViewModel();
            evcarddetails = evcarddetails.OrderByDescending(x => x.bookmarkflag).ToList();
            var CacheProvider = DependencyResolver.Current.GetService<ICacheProvider>();
            if (CacheProvider.TryGet(CacheKeys.EV_SELECTEDCARD, out string selectedcard))
            {
                evcarddetails = evcarddetails.OrderByDescending(x => selectedcard.Contains(x.cardnumber)).ToList();
            }
            if (evcarddetails != null && evcarddetails.Count > 0)
            {
                evcarddetails.ForEach(x => model.eVCardDetails.Add(new EVCardDetailModel
                {
                    bookmarkflag = x.bookmarkflag,
                    contractaccountname = x.contractaccountname,
                    cardnumber = x.cardnumber,
                    contractaccount = x.contractaccount,
                    nickname = x.nickname,
                    platenumber = x.platenumber,
                    rfid = x.rfid,
                    serialnumber = x.serialnumber,
                    username = x.username,
                    activationdate = x.cardactivationdate,
                    deactivationdate = x.deactivationdate,
                }));
            }
            if (!string.IsNullOrWhiteSpace(keyword) && model.eVCardDetails != null && model.eVCardDetails.Count > 0)
            {
                keyword = keyword.Trim().ToLower();
                model.eVCardDetails = model.eVCardDetails.Where(x => (!string.IsNullOrWhiteSpace(x.nickname) && x.nickname.ToLower().Contains(keyword)) ||
               (!string.IsNullOrWhiteSpace(x.username) && x.username.ToLower().Contains(keyword)) ||
               (!string.IsNullOrWhiteSpace(x.cardnumber) && x.cardnumber.ToLower().Contains(keyword)) ||
               (!string.IsNullOrWhiteSpace(x.contractaccountname) && x.contractaccountname.ToLower().Contains(keyword)) ||
               (!string.IsNullOrWhiteSpace(x.platenumber) && x.platenumber.ToLower().Contains(keyword))).ToList();
            }
            model.page = page;
            int count = 10;
            model.totalpage = Pager.CalculateTotalPages(model.eVCardDetails.Count(), count);
            model.pagination = model.totalpage > 1 ? true : false;
            model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();
            model.eVCardDetails = model.eVCardDetails.Skip((page - 1) * count).Take(count).ToList();
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

    public class EVCardDetailModel
    {
        public string bookmarkflag { get; set; }
        public string contractaccountname { get; set; }
        public string cardnumber { get; set; }
        public string contractaccount { get; set; }
        public string nickname { get; set; }
        public string platenumber { get; set; }
        public string rfid { get; set; }
        public string serialnumber { get; set; }
        public string username { get; set; }
        public string activationdate { get; set; }
        public string deactivationdate { get; set; }
    }
}