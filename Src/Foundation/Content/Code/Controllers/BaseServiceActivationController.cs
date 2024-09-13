using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Controllers
{
    public abstract class BaseWorkflowController<TState> : BaseController
        where TState : class, new()
    {
        /// <summary>
        /// Gets the current state of the workflow
        /// </summary>
        protected TState State
        {
            get
            {
                TState state;
                if (!CacheProvider.TryGet(Name, out state))
                {
                    state = new TState();
                    Save(state);
                }
                return state;
            }
        }

        /// <summary>
        /// Gets the unique name or description of the workflow
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// Clears any state associated with the workflow
        /// </summary>
        protected virtual void Clear()
        {
            CacheProvider.Remove(Name);
        }

        /// <summary>
        /// Persists the current state of the workflow
        /// </summary>
        protected virtual void Save(TState overwrite = null)
        {
            CacheProvider.Store(Name, new CacheItem<TState>(overwrite ?? State));
        }

        /// <summary>
        /// Determines whether the workflow is currently in progress
        /// </summary>
        /// <returns></returns>
        protected virtual bool InProgress()
        {
            return CacheProvider.HasKey(Name);
        }
    }

    public class BaseServiceActivationController : BaseWorkflowController<BaseServiceWorkflowModel>
    {
        protected override string Name
        {
            get { return CacheKeys.MOVE_IN_WORKFLOW_STATE; }
        }

        public Dictionary<string, string> DropDownTermValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet(CacheKeys.TERMS, out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store(CacheKeys.TERMS, new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        protected override void Clear()
        {
            CacheProvider.Remove(CacheKeys.TERMS);

            base.Clear();
        }

        #region List Population

        protected IEnumerable<SelectListItem> GetResMoveinIdTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_IDTYPES_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ID Types");
            }
        }

        protected IEnumerable<SelectListItem> GetNonResMoveinIdTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_IDTYPES_NON_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ID Types");
            }
        }

        protected List<SelectListItem> GetIdTypes(string customercategory, string customertype)
        {
            if (customercategory == "O" && (customertype == "T" || customertype == "O"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Trade License".AddToTermDictionary("TN", DropDownTermValues), Value = "TN"}
                };
            }
            if (customercategory == "P" && (customertype == "T" || customertype == "O"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Emarites ID".AddToTermDictionary("ED", DropDownTermValues), Value = "ED"},
                };
            }
            if (customercategory == "P" && (customertype == "G"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Passport".AddToTermDictionary("PN", DropDownTermValues), Value = "PN"},
                };
            }
            return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Emarites ID".AddToTermDictionary("ED", DropDownTermValues), Value = "ED"},
                };
        }

        protected List<SelectListItem> GetCustomerCategories(string bptype)
        {
            if (string.IsNullOrEmpty(bptype))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Person".AddToTermDictionary("P", DropDownTermValues), Value = "P"},
                    new SelectListItem() {Text = "Organisation".AddToTermDictionary("O", DropDownTermValues), Value = "O"}
                };
            }
            else
            {
                if (bptype == BPTypes.PERSON)
                {
                    return new List<SelectListItem>
                    {
                        new SelectListItem() {Text = "Person".AddToTermDictionary("P", DropDownTermValues), Value = "P"},
                    };
                }
                else if (bptype == BPTypes.ORGANISATION)
                {
                    return new List<SelectListItem>
                    {
                        new SelectListItem() {Text = "Organisation".AddToTermDictionary("O", DropDownTermValues), Value = "O"}
                    };
                }
            }
            return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Person".AddToTermDictionary("P", DropDownTermValues), Value = "P"},
                    new SelectListItem() {Text = "Organisation".AddToTermDictionary("O", DropDownTermValues), Value = "O"}
                };
        }

        public List<SelectListItem> GetCustomerTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = "Landlord".AddToTermDictionary("O", DropDownTermValues, "CT"), Value = "O"},
                new SelectListItem() {Text = "Tenant".AddToTermDictionary("T", DropDownTermValues, "CT"), Value = "T"},
                new SelectListItem() {Text = "GccCustom".AddToTermDictionary("G", DropDownTermValues, "CT"), Value = "G"},
                new SelectListItem() {Text = "Owner".AddToTermDictionary("W", DropDownTermValues, "CT"), Value = "W"}
            };
        }

        public IEnumerable<SelectListItem> GetResMoveinCustomerTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_CUST_TYPES_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ResMoveinCustomerTypes");
            }
        }

        public IEnumerable<SelectListItem> GetNonResMoveinCustomerTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_CUST_TYPES_NON_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting NonResMoveinCustomerTypes");
            }
        }

        public List<SelectListItem> GetCustomerTypes(string customerCategory)
        {
            if (string.IsNullOrEmpty(customerCategory))
            {
                return new List<SelectListItem>();
            }

            if (customerCategory == "P")
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Owner".AddToTermDictionary("O", DropDownTermValues, "CT"), Value = "O"},
                    new SelectListItem() {Text = "Tenant".AddToTermDictionary("T", DropDownTermValues, "CT"), Value = "T"},
                    new SelectListItem() {Text = "GccCustom".AddToTermDictionary("G", DropDownTermValues, "CT"), Value = "G"},
                };
            }

            return new List<SelectListItem>
            {
                new SelectListItem() {Text = "Tenant".AddToTermDictionary("T", DropDownTermValues, "CT"), Value = "T"},
                new SelectListItem() {Text = "Owner".AddToTermDictionary("O", DropDownTermValues, "CT"), Value = "O"}
            };
        }

        public List<SelectListItem> GetAccountTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = "UAE National".AddToTermDictionary("U", DropDownTermValues), Value = "U"},
                new SelectListItem() {Text = "Industrial".AddToTermDictionary("I", DropDownTermValues), Value = "I"},
                new SelectListItem() {Text = "Expatriate".AddToTermDictionary("E", DropDownTermValues), Value = "E"},
                new SelectListItem() {Text = "Commercial".AddToTermDictionary("C", DropDownTermValues), Value = "C"}
            };
        }

        public List<SelectListItem> GetNumberOfRooms()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = Translate.Text(DictionaryKeys.MoveIn.Studio), Value = "1"},
                new SelectListItem() {Text = "1 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "2"},
                new SelectListItem() {Text = "2 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "3"},
                new SelectListItem() {Text = "3 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "4"},
                new SelectListItem() {Text = "4 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "5"},
                new SelectListItem() {Text = "5 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "6"},
                new SelectListItem() {Text = "6 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "7"},
                new SelectListItem() {Text = "6+ " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "8"}
            };
        }

        protected List<SelectListItem> GetEmirates()
        {
            var emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates);

            var result = from itm in emirates.ToList()
                         select new SelectListItem()
                         {
                             Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownTermValues),
                             Value = itm.DisplayName
                         };

            return result.ToList();
        }

        protected string GetBusinessPartnerType(string Bpnumber)
        {
            if (string.IsNullOrEmpty(Bpnumber)) return string.Empty;
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
                RequestLanguage, Request.Segment());
            var bp = UserDetails.Payload.BusinessPartners.FirstOrDefault(c => c.businesspartnernumber == Bpnumber);
            if (bp != null) return bp.CustomerType;
            return string.Empty;
        }

        #endregion List Population
    }
}