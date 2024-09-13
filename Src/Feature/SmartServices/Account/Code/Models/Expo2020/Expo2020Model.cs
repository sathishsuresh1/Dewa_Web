using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Models.Expo2020
{
    [Serializable]
    public class Expo2020Model
    {
        public string EXPOParticipant { get; set; }
        public string EXPOParticipantName { get; set; }
        public string EXPOEmailID { get; set; }
        public string EXPOMobile { get; set; }
        public string EXPODiscussionArea { get; set; }
        public string EXPODiscussionSubject { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsLoggedIn { get; set; }

        #region DropdownlistItems
        public List<SelectListItem> EXPODiscussionAreaList { get; set; }
        #endregion
    }
}