using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    public class EVSDPaymentViewModel
    {
        public EVSDPaymentViewModel()
        {
            Vehiclelists = new List<VehiclelistModel>();
        }

        public string accounts { get; set; }
        public List<VehiclelistModel> Vehiclelists { get; set; }
        public IPagedList<VehiclelistModel> PagedAccounts { get; set; }
        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public string Businesspartnernumber { get; set; }
        public string Carregistrationcountry { get; set; }
        public string Carregistrationregion { get; set; }
        public string Fullname { get; set; }
        public string Customercategory { get; set; }
        public string Trafficfileno { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }

        public static EVSDPaymentViewModel From(EVDeepLinkResponse serviceResponse, int? page)
        {
            EVSDPaymentViewModel model = new EVSDPaymentViewModel();
            if (serviceResponse != null && serviceResponse.Vehiclelist != null && serviceResponse.Vehiclelist.Count > 0)
            {
                model = new EVSDPaymentViewModel
                {
                    Businesspartnernumber = serviceResponse.Businesspartnernumber,
                    Carregistrationcountry = serviceResponse.Carregistrationcountry,
                    Carregistrationregion = serviceResponse.Carregistrationregion,
                    Customercategory = serviceResponse.Customercategory,
                    Trafficfileno = serviceResponse.Trafficfileno,
                    Fullname = serviceResponse.Fullname
                };
                serviceResponse.Vehiclelist.ForEach(x => model.Vehiclelists.Add(new VehiclelistModel
                {
                    Courierfee = x.Courierfee,
                    Courierfeevatamount = x.Courierfeevatamount,
                    Currencykey = x.Currencykey,
                    Platecategorycode = x.Platecategorycode,
                    Platecategorydescription = x.Platecategorydescription,
                    Platecode = x.Platecode,
                    Platecodedescription = x.Platecodedescription,
                    Platenumber = x.Platenumber,
                    Sdamount = x.Sdamount,
                    Totalamount = x.Totalamount,
                    Vatrate = x.Vatrate,
                }));
            }
            int count = 10;
            int totalPages = 0;
            var pageNumber = page ?? 1;
            totalPages = Pager.CalculateTotalPages(model.Vehiclelists.Count, count);
            model.PagedAccounts = model.Vehiclelists.ToPagedList<VehiclelistModel>(pageNumber, count);
            //model.page = page;
            //int count = 10;
            //model.totalpage = DEWAXP.Foundation.Helpers.Pager.CalculateTotalPages(model.Vehiclelists.Count(), count);
            //model.pagination = model.totalpage > 1 ? true : false;
            //model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();
            //model.Vehiclelists = model.Vehiclelists.Skip((page - 1) * count).Take(count).ToList();
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

    public class VehiclelistModel
    {
        public string Courierfee { get; set; }
        public string Courierfeevatamount { get; set; }
        public string Currencykey { get; set; }
        public string Platecategorycode { get; set; }
        public string Platecategorydescription { get; set; }
        public string Platecode { get; set; }
        public string Platecodedescription { get; set; }
        public string Platenumber { get; set; }
        public string Sdamount { get; set; }

        public string Totalamount { get; set; }
        public string Vatrate { get; set; }
    }
}