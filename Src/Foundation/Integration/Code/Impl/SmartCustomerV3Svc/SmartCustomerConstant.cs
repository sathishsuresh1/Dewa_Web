using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc
{
    public static class SmartCustomerConstant
    {
        /// <summary>
        /// Defines the DISPLAY_TRAINING.
        /// </summary>
        internal const string GET_SLABCAPS = "slabcaps";
        /// <summary>
        /// Define the Get Active contract accounts
        /// </summary>
        internal const string GET_ACTIVE_CONTRACT_ACCOUNTS = "activecontractaccounts";
        internal const string GET_ALL_CONTRACT_ACCOUNTS = "allcontractaccounts";

        internal const string GET_EMIRATESID_DETAILS = "visadetailsforeid";
        /// <summary>
        /// Defines the get Smart meter.
        /// </summary>
        internal const string GET_SMART_METER_RESPONSE = "smartmeterresponse";

        #region Ev Smart Charger
        /// <summary>
        /// Defines the get Smart meter.
        /// </summary>
        internal const string EVSMARTCHARGER = "onetimecharge";
        #endregion

        #region Ev Dashboard
        
        internal const string FETCH_ACTIVEEVCARDS = "ev/activecards/fetch";
        internal const string UPDATEEVCARD = "ev/activecards/update";
        internal const string EVCONSUMPTION = "ev/consumption";
        internal const string EVTRANSCATIONS = "ev/transactiondetails";
        internal const string OUTSTANDINGBREAKDOWN = "outstandingbreakdown";
        internal const string EVSDPAYMENT = "ev/deeplink";
        #endregion

        internal const string BILLPAYMENTHISTORY = "billpaymenthistory";

        /// <summary>
        /// Defines the Infrastructure Noc list.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_LIST = "poinfranoclist";
        /// <summary>
        /// Defines the Infrastructure Noc dropdown values.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_WORKTYPES = "infranocpodropdown";
        /// <summary>
        /// Defines the Infrastructure Noc new submit request.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_SUBMIT = "postpoinfranocsubmission";
        /// <summary>
        /// Defines the Infrastructure Noc new Re-submit request.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_RESUBMIT = "postpoinfranocresubmission";
        /// <summary>
        /// Defines the Infrastructure vew NOC request.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_DETAILS = "viewpoinfranoc";
        /// <summary>
        /// Defines the Infrastructure Noc download document.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_DOWNLOAD_FILE = "poinfranocattachment";
        /// <summary>
        /// Defines the Infrastructure Noc view status and comment.
        /// </summary>
        internal const string INFRASTRUCTURE_NOC_STATUS = "poinfranocstatus";

        internal const string FORGOTUSERIDPWD = "forgotpass/customer";

        internal const string UNLOCKUSERID = "unlock/customer";

        internal const string LOGIN_USER = "login/customer";

        internal const string UAEPGSLIST = "uaepgslist";
        /// <summary>
        /// Defines the LANGUAGE.
        /// </summary>
        internal const string LANGUAGE = "lang";
    }
}
