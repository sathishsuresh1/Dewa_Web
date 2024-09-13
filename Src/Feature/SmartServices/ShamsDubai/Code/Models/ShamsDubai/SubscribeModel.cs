using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ShamsDubai.Models.ShamsDubai
{
    [Serializable]
    public class SubscribeModel
    {
        public string UniqueID { get; set; }
        public string Name { get; set; }
        public string SubscriberType { get; set; }
        public string Others { get; set; }
        public string Email { get; set; }
        public string UnsubscribeEmail { get; set; }
        public string Message { get; set; }
        public List<SelectListItem> CustomerTypeList { get; set; }
        public string q { get; set; }
        public string ProcessingType { get; set; }
    }
    public enum ShamsDubai
    {
        Subscribe,
        Unsubscribe,
        Confirm
    }
}