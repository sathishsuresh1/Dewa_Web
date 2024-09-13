using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    public class BehaviourinsightCustomerController : BaseApiController
    {
        public HttpResponseMessage Get(string uniqueID)
        {
            var _response = DewaApiClient.GetBehaviourinsightCustomer(uniqueID, null, RequestLanguage, Request.Segment());

            var _behaviourinsightCustomerRequest = new BehaviourinsightCustomerRequest();

            if (_response.Payload != null)
            {
                _behaviourinsightCustomerRequest.Items = _response.Payload.AccountNo.Electricity.Item.First();

                _behaviourinsightCustomerRequest.IsShowTrophy = false;

                var consumptionValue = Convert.ToDecimal(_behaviourinsightCustomerRequest.Items.ConsumtionValue);
                var avgConsumptionValue = Convert.ToDecimal(_behaviourinsightCustomerRequest.Items.AvgConsumtionValue);
                var lowConsumptionValue = Convert.ToDecimal(_behaviourinsightCustomerRequest.Items.LowConsumtionValue);

                if (consumptionValue > avgConsumptionValue)
                {
                    _behaviourinsightCustomerRequest.TitleText = Translate.Text("001");
                }
                else if ((consumptionValue < avgConsumptionValue) && (consumptionValue > lowConsumptionValue))
                {
                    _behaviourinsightCustomerRequest.TitleText = Translate.Text("002");
                }
                else if (consumptionValue < lowConsumptionValue)
                {
                    _behaviourinsightCustomerRequest.IsShowTrophy = true;
                    _behaviourinsightCustomerRequest.TitleText = Translate.Text("003");
                }
                else if (consumptionValue == lowConsumptionValue)
                {
                    _behaviourinsightCustomerRequest.IsShowTrophy = true;
                    _behaviourinsightCustomerRequest.TitleText = Translate.Text("004");
                }
                else if ((consumptionValue == avgConsumptionValue) && (consumptionValue > lowConsumptionValue))
                {
                    _behaviourinsightCustomerRequest.TitleText = Translate.Text("005");
                }

                _behaviourinsightCustomerRequest.LongBillingDateMonth = GetLongDate(_behaviourinsightCustomerRequest.Items.BillingDateMonth);

                return Request.CreateResponse(HttpStatusCode.OK, _behaviourinsightCustomerRequest);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available");
        }

        private string GetLongDate(string strDate)
        {
            string _dateString = strDate;
            DateTime _dateObject;
            _dateObject = new DateTime();

            _dateObject = DateTime.ParseExact(_dateString, "yyyy/MM", null);

            var months = DateHelper.GetMonthsList();

            var _currentMonth = months.FirstOrDefault(x => x.Value == _dateObject.Month.ToString().TrimStart('0'));

            string _responseDateFormat = _currentMonth.Text + "  " + _dateObject.Year;

            return _responseDateFormat;
        }

        public class BehaviourinsightCustomerRequest
        {
            public bool IsShowTrophy
            {
                get;
                set;
            }

            public string LongBillingDateMonth { get; set; }

            public string TitleText { get; set; }

            public string AccountNumber { get; set; }

            public string UniqueID { get; set; }

            public BehaviourinsightCustomerItem Items { get; set; }
        }
    }
}