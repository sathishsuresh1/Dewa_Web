using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.SupplyManagement.Movein;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class BaseMoveInController : BaseWorkflowController<MoveInViewModelv3>
    {
        protected override string Name
        {
            get { return CacheKeys.MOVE_IN_3_WORKFLOW_STATE; }
        }

        protected override void Clear()
        {
            CacheProvider.Remove("DDEMIRATES");

            base.Clear();
        }
        #region Methods
        protected IEnumerable<SelectListItem> GetMoveinIdTypes(string type)
        {
            try
            {
                if (type == "P")
                {
                    var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_ID_TYPES_RES));
                    var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems;
                }
                else
                {
                    var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_ID_TYPES_NON_RES));
                    var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems;
                }
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting NonRes ID Types");
            }
        }

        protected IEnumerable<SelectListItem> GetNonResMoveinIdTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_ID_TYPES_NON_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting NonRes ID Types");
            }
        }

        public IEnumerable<SelectListItem> GetCustomerTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_CUST_TYPES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Customer Types");
            }
        }

        public IEnumerable<SelectListItem> GetAccountTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_ACC_TYPES)); 
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Account Types");
            }
        }

        public IEnumerable<SelectListItem> GetOwnerType()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.MOVE_IN_OWNER_TYPES)); 
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Owner Types");
            }
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
                             Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownEmiratesValues),
                             Value = itm.DisplayName
                         };

            return result.ToList();
        }

        private Dictionary<string, string> DropDownEmiratesValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet("DDEMIRATES", out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store("DDEMIRATES", new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        public Dictionary<string, string> DropDownNationalitiesValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet("DDNationalities", out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store("DDNationalities", new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }
        #endregion

    }
}