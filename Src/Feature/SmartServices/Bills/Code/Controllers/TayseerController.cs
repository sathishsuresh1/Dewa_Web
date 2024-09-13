using DEWAXP.Feature.Bills.Helpers;
using DEWAXP.Feature.Bills.Models.Tayseer;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static Glass.Mapper.Constants;
using DEWAXP.Foundation.Integration.Requests.Tayseer;
using DEWAXP.Foundation.Integration.Responses.Tayseer;
using System.Web.Services;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Feature.Bills.Models.EasyPay;
using DEWAXP.Foundation.Content.Repositories;
using System.Globalization;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class TayseerController : BaseController
    {
        #region Global Variables

        private List<bool> isChecked = null;
        private decimal maxAllowedAmount = 999999999.99M;

        #endregion Global Variables

        #region Properties

        private DataTable m_dtErrors = null;

        public DataTable ErrorTable
        {
            get
            {
                if (Session["errors"] != null)
                {
                    m_dtErrors = Session["errors"] as DataTable;
                }
                return m_dtErrors;
            }
            set
            {
                Session["errors"] = value;
            }
        }

        private DataTable m_dtFileUpload = null;

        public DataTable FileUploadTable
        {
            get
            {
                if (Session["exceltbl"] != null)
                {
                    m_dtFileUpload = Session["exceltbl"] as DataTable;
                }
                return m_dtFileUpload;
            }
            set
            {
                Session["exceltbl"] = value;
            }
        }

        private DataTable m_dt = null;

        public DataTable AccountsTable
        {
            get
            {
                if (Session["accounts"] != null)
                {
                    m_dt = Session["accounts"] as DataTable;
                }
                return m_dt;
            }
            set
            {
                Session["accounts"] = value;
            }
        }

        private List<string> m_lstAccounts = null;

        public List<string> AccountList
        {
            get
            {
                if (Session["accountlist"] != null)
                {
                    m_lstAccounts = (List<string>)(Session["accountlist"]);
                }
                return m_lstAccounts;
            }
            set
            {
                Session["accountlist"] = value;
            }
        }

        private List<string> m_lstAmounts = null;

        public List<string> AmountList
        {
            get
            {
                if (Session["amountlist"] != null)
                {
                    m_lstAmounts = (List<string>)(Session["amountlist"]);
                }
                return m_lstAmounts;
            }
            set
            {
                Session["amountlist"] = value;
            }
        }

        private List<string> m_lstfinalBill = null;

        public List<string> FinalBillList
        {
            get
            {
                if (Session["finalbilllist"] != null)
                {
                    m_lstfinalBill = (List<string>)(Session["finalbilllist"]);
                }
                return m_lstfinalBill;
            }
            set
            {
                Session["finalbilllist"] = value;
            }
        }

        #endregion Properties

        #region TayseerExitingAccount

        [HttpGet]
        public ActionResult TayseerExitingAccount()
        {
            DataTable dtSource = null;
            DataTable dtAccounts = null;
            DataTable dtNewAccounts = null;
            DataTable dtOutstanding = null;
            DataTable dtMerged = null;
            List<string> accountList = new List<string>();
            AccountDetails[] accounts = new AccountDetails[0];
            List<TayseerFileUploadModel> tayseerExistingAccountList = new List<TayseerFileUploadModel>();

            try
            {
                ViewBag.ReviewURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_PAYMENT_REVIEW_EXISTING);

                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                // if/else condition for Get existing Generate reference number.
                if (Request.QueryString["refno"] != null && !string.IsNullOrEmpty(Request.QueryString["refno"].ToString()))
                {
                    //var response = DewaApiClient.GetRefNoDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.QueryString["refno"].ToString(), RequestLanguage, Request.Segment());
                    var responseData = TayseerClient.TayseerDetails(new TayseerDetailsRequest
                    {
                        referencenumberinputs = new Referencenumberinputs
                        {
                            dewareferencenumber = Request.QueryString["refno"].ToString(),
                        }
                    }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);
                    if (responseData != null && responseData.Payload != null && responseData.Succeeded)
                    {
                        dtNewAccounts = new DataTable();
                        dtNewAccounts.Columns.Add("AccountNumber", typeof(string));
                        dtNewAccounts.Columns.Add("ContractAccount", typeof(string));
                        dtNewAccounts.Columns.Add("Selected", typeof(bool));
                        dtNewAccounts.Columns.Add("AmountToPay", typeof(decimal));
                        dtNewAccounts.Columns.Add("Amount", typeof(decimal));
                        dtNewAccounts.Columns.Add("AmountPaid", typeof(decimal));
                        dtNewAccounts.Columns.Add("collectiveaccount", typeof(string));

                        foreach (var itemAcct in responseData.Payload.accountlist)
                        {
                            DataRow dr = dtNewAccounts.NewRow();
                            dr["Selected"] = true;
                            dr["AmountToPay"] = itemAcct.amount;
                            dr["Amount"] = itemAcct.amount;
                            dr["AccountNumber"] = itemAcct.contractaccount;
                            dr["ContractAccount"] = itemAcct.contractaccount;
                            dr["AmountPaid"] = itemAcct.amountpaid;
                            dr["collectiveaccount"] = itemAcct.collectiveaccount;
                            dtNewAccounts.Rows.Add(dr);
                        }
                    }
                    if (dtNewAccounts != null)
                    {
                        accountList = GetAccountList(dtNewAccounts);
                        dtOutstanding = GetOutstandingAmount(accountList);
                        dtMerged = MergeTables(dtNewAccounts, dtOutstanding);
                        AccountsTable = dtMerged;
                        ViewBag.ReferenceNo = Request.QueryString["refno"].ToString();
                    }
                    foreach (DataRow row in AccountsTable.Rows)
                    {

                        var tayseerExistingAccount = new TayseerFileUploadModel();
                        if (string.IsNullOrEmpty(row["collectiveaccount"].ToString()))
                        {
                            tayseerExistingAccount.SrNo = -1;
                        }
                        tayseerExistingAccount.ContractAccountNo = row["ContractAccount"].ToString();
                        tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();

                        if (!string.IsNullOrEmpty(tayseerExistingAccount.OutstandingAmount) && Convert.ToDecimal(tayseerExistingAccount.OutstandingAmount) < 0)
                        {
                            tayseerExistingAccount.AmounttoPay = "0";
                            row["AmountToPay"] = 0;
                        }
                        else
                        {
                            row["AmountToPay"] = string.IsNullOrEmpty(tayseerExistingAccount.OutstandingAmount) ? 0 : Convert.ToDecimal(row["OutstandingAmount"]);
                            tayseerExistingAccount.AmounttoPay = string.IsNullOrEmpty(tayseerExistingAccount.OutstandingAmount) ? "0" : row["OutstandingAmount"].ToString();
                        }

                        if (row["AmountToPay"] != null && !string.IsNullOrEmpty(row["AmountToPay"].ToString()) && (row["AmountToPay"].ToString() == "0" || row["AmountToPay"].ToString() == "0.00" || row["AmountToPay"].ToString() == "0.0"))
                            row["Selected"] = false;
                        

                        tayseerExistingAccount.Indicator = row["Indicator"].ToString();
                        tayseerExistingAccount.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                        tayseerExistingAccountList.Add(tayseerExistingAccount);
                    }
                    AccountsTable.AcceptChanges();
                    ViewBag.SetTotal = SetTotals(AccountsTable);
                }
                else
                {
                    //var response = DewaApiClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());
                    var response = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, true, RequestLanguage, Request.Segment());

                    if (response.Succeeded)
                    {
                        accounts = response.Payload;
                    }
                    dtSource = TayseerHelper.ConvertArrayToDatatable(accounts);
                    if (dtSource != null && response.Succeeded)
                    {
                        #region Deleted Columns

                        dtSource.Columns.Remove("Water");
                        dtSource.Columns.Remove("Electricity");
                        dtSource.Columns.Remove("Sewerage");
                        dtSource.Columns.Remove("DM");
                        dtSource.Columns.Remove("Cooling");
                        dtSource.Columns.Remove("Others");
                        dtSource.Columns.Remove("AccountName");
                        dtSource.Columns.Remove("NickName");
                        dtSource.Columns.Remove("Category");

                        #endregion Deleted Columns

                        dtSource.Columns.Add("Selected", typeof(bool));
                        dtSource.Columns.Add("AmountToPay", typeof(decimal));
                        dtAccounts = dtSource.Clone();

                        dtAccounts.Columns["Balance"].DataType = typeof(decimal);
                        isChecked = new List<bool>();

                        foreach (DataRow row in dtSource.Rows)
                        {
                            var _sBalance = row["Balance"] != null && !string.IsNullOrEmpty(row["Balance"].ToString()) ? row["Balance"].ToString() : "0.0";
                            var convertDecimal = Decimal.Parse(_sBalance, NumberStyles.Any, CultureInfo.InvariantCulture);

                            row["Selected"] = convertDecimal <= 0 ? false : true;
                            row["AmountToPay"] = convertDecimal <= 0 ? "0" : row["Balance"];
                            dtAccounts.ImportRow(row);
                        }
                        accountList = GetExistingAccountList(dtAccounts);
                        dtOutstanding = GetOutstandingAmount(accountList);
                        dtMerged = MergeExistingTables(dtAccounts, dtOutstanding);
                        AccountsTable = dtMerged;
                        ViewBag.SetTotal = SetTotals(dtMerged);
                    }
                    foreach (DataRow row in AccountsTable.Rows)
                    {
                        var tayseerExistingAccount = new TayseerFileUploadModel();
                        if (string.IsNullOrEmpty(row["PremiseNumber"].ToString()))
                        {
                            tayseerExistingAccount.SrNo = -1;
                        }
                        tayseerExistingAccount.ContractAccountNo = row["AccountNumber"].ToString();
                        tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();
                        tayseerExistingAccount.AmounttoPay = row["AmountToPay"].ToString();
                        tayseerExistingAccount.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                        tayseerExistingAccountList.Add(tayseerExistingAccount);
                    }
                }

                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAccount.cshtml", tayseerExistingAccountList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAccount.cshtml", tayseerExistingAccountList);
            }
        }

        #endregion TayseerExitingAccount

        #region TayseerAddExistingAccount

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerAddExistingAccount(string ContractAccountNo, string AmounttoPay, string selectedAccount)
        {
            string error = string.Empty;
            List<TayseerFileUploadModel> tayseerExistingAddAccountList = new List<TayseerFileUploadModel>();
            try
            {
                List<string> accountList = new List<string>();
                DataTable dtOutstanding = null;
                DataTable dtMerged = null;
                DataRow drow = null;
                DataTable dataTable = null;
                string Oustandingamount = string.Empty;
                string serviceError = string.Empty;
                dataTable = AccountsTable;

                if (TayseerHelper.ValidateBeforeAdd(ContractAccountNo, AmounttoPay, out error))
                {
                    if (GetBillDetails(ContractAccountNo, out Oustandingamount, out serviceError))
                    {
                        if (dataTable != null)
                        {
                            if (dataTable.Select("AccountNumber = '" + ContractAccountNo + "'").Length == 0)
                            {
                                drow = dataTable.NewRow();
                                drow["AccountNumber"] = ContractAccountNo;
                                drow["AmountToPay"] = AmounttoPay;
                                if (dataTable.Columns["Selected"] != null)
                                    drow["Selected"] = true;
                                dataTable.Rows.InsertAt(drow, 0);

                                accountList = GetExistingAccountList(dataTable);

                                dtOutstanding = GetOutstandingAmount(accountList);
                                dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                                dataTable = dtMerged;
                                AccountsTable = dataTable;
                                ViewBag.SetTotal = SetTotals(dtMerged);
                            }
                            else
                            {
                                //ViewBag.Message = string.Format("The entered account is already in the list");
                                ViewBag.Message = Translate.Text("ValidationAlreadyList");
                                ViewBag.SetTotal = SetTotals(dataTable);
                            }
                        }
                    }
                    else
                    {
                        accountList = GetExistingAccountList(dataTable);
                        dtOutstanding = GetOutstandingAmount(accountList);
                        dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                        dataTable = dtMerged;
                        AccountsTable = _AddselectedAccountTable(dataTable, selectedAccount);
                        ViewBag.SetTotal = SetTotals(dtMerged);
                        ViewBag.Message = serviceError;
                    }
                }
                else
                {
                    accountList = GetExistingAccountList(dataTable);
                    dtOutstanding = GetOutstandingAmount(accountList);
                    dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                    dataTable = dtMerged;
                    AccountsTable = _AddselectedAccountTable(dataTable, selectedAccount);
                    ViewBag.SetTotal = SetTotals(dtMerged);
                    ViewBag.Message = error;
                }
                foreach (DataRow row in AccountsTable.Rows)
                {
                    var tayseerExistingAccount = new TayseerFileUploadModel();
                    if ((AccountsTable.Columns.Contains("PremiseNumber") && string.IsNullOrEmpty(row["PremiseNumber"].ToString())) || (AccountsTable.Columns.Contains("collectiveaccount") && string.IsNullOrEmpty(row["collectiveaccount"].ToString())))
                    {
                        tayseerExistingAccount.SrNo = -1;
                    }
                    tayseerExistingAccount.ContractAccountNo = row["AccountNumber"].ToString();
                    tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();
                    tayseerExistingAccount.AmounttoPay = row["AmountToPay"].ToString();
                    tayseerExistingAccount.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                    tayseerExistingAddAccountList.Add(tayseerExistingAccount);
                }
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAddAccount.cshtml", tayseerExistingAddAccountList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAddAccount.cshtml", tayseerExistingAddAccountList);
            }
        }

        #endregion TayseerAddExistingAccount

        #region TayseerDeleteExistingAccount

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerDeleteExistingAccount(string ContractAccountNo, string selectedAccount)
        {
            DataRow[] rowColl = null;
            string error = string.Empty;
            List<TayseerFileUploadModel> tayseerExistingAddAccountList = new List<TayseerFileUploadModel>();
            try
            {
                if (TayseerHelper.Validateaccountnumber(ContractAccountNo, out string errorMsg, out bool retVal))
                {
                    if (AccountsTable != null && AccountsTable.Rows.Count > 0)
                    {
                        rowColl = AccountsTable.Select("AccountNumber=" + "'" + ContractAccountNo.Trim() + "'");
                        foreach (DataRow row in rowColl)
                        {
                            AccountsTable.Rows.Remove(row);
                        }
                        AccountsTable = _AddselectedAccountTable(AccountsTable, selectedAccount);
                        foreach (DataRow row in AccountsTable.Rows)
                        {
                            var tayseerExistingAccount = new TayseerFileUploadModel();
                            if ((AccountsTable.Columns.Contains("PremiseNumber") && string.IsNullOrEmpty(row["PremiseNumber"].ToString())) || (AccountsTable.Columns.Contains("collectiveaccount") && string.IsNullOrEmpty(row["collectiveaccount"].ToString())))
                            {
                                tayseerExistingAccount.SrNo = -1;
                            }
                            tayseerExistingAccount.ContractAccountNo = row["AccountNumber"].ToString();
                            tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();
                            tayseerExistingAccount.AmounttoPay = row["AmountToPay"].ToString();
                            tayseerExistingAccount.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                            tayseerExistingAddAccountList.Add(tayseerExistingAccount);
                        }
                        ViewBag.SetTotal = SetTotals(AccountsTable);
                    }
                    else
                    {
                        // ShowMessage("error", error, true);
                    }
                }
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAddAccount.cshtml", tayseerExistingAddAccountList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerExistingAddAccount.cshtml", tayseerExistingAddAccountList);
            }
        }

        #endregion TayseerDeleteExistingAccount

        #region TayseerExistingAccountchecked

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerExistingAccountchecked(string ContractAccount, bool AccountCheck)
        {
            try
            {
                DataRow rowColl = null;
                if (TayseerHelper.Validateaccountnumber(ContractAccount, out string errorMsg, out bool retVal))
                {
                    if (!String.IsNullOrEmpty(ContractAccount) && AccountsTable != null && AccountsTable.Rows.Count > 0)
                    {
                        rowColl = AccountsTable.Select("AccountNumber=" + "'" + ContractAccount.Trim() + "'").FirstOrDefault();
                        //rowColl = AccountsTable.Select("AccountNumber=" + ContractAccount).FirstOrDefault();
                        if (rowColl != null)
                        {
                            rowColl["Selected"] = AccountCheck;
                            var total = SetTotals(AccountsTable);
                            return Json(new { status = true, Data = total }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion TayseerExistingAccountchecked

        #region TayseerExistingAccountcheckedAll

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerExistingAccountcheckedAll(bool AccountCheck)
        {
            try
            {
                DataRow[] rowColl = null;

                if (AccountsTable != null && AccountsTable.Rows.Count > 0)
                {
                    rowColl = AccountsTable.Select();
                    foreach (DataRow row in rowColl)
                    {
                        row["Selected"] = AccountCheck;
                    }
                    var total = SetTotals(AccountsTable);
                    return Json(new { status = true, Data = total }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion TayseerExistingAccountcheckedAll

        #region TayseerExistingAccountEdit

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerExistingAccountEdit(string ContractAccountNo, string AmounttoPay)
        {
            try
            {
                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                decimal amount = decimal.Zero;
                DataRow[] rowColl = null;
                string error = string.Empty;
                string _format = "<ul class='parsley-errors-list filled'></li class='parsley-range'>{0}</li></ul>";
                List<TayseerFileUploadModel> tayseerExistingAccountList = new List<TayseerFileUploadModel>();

                if (TayseerHelper.ValidateBeforeAdd(ContractAccountNo, AmounttoPay, out error))
                {
                    //dt = FileUploadTable;
                    if (AccountsTable != null && AccountsTable.Rows.Count > 0)
                    {
                        rowColl = AccountsTable.Select("AccountNumber=" + "'" + ContractAccountNo.Trim() + "'"); ;
                        if (decimal.TryParse(AmounttoPay.Trim(), out amount))
                        {
                            rowColl[0]["AmountToPay"] = AmounttoPay.Trim();
                            AccountsTable.AcceptChanges();

                            var total = SetTotals(AccountsTable);
                            return Json(new { status = true, Data = total, Message = "" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = false, Data = "", Message = string.Format(_format, Translate.Text("ValidAmount")) }, JsonRequestBehavior.AllowGet);
                            //ViewBag.ValidationMsg = "Please enter valid amount";
                            // ShowMessage(GetText("MsgEnterValidAmount"), error, true);
                        }
                    }
                }
                else
                {
                    return Json(new { status = false, Data = "", Message = string.Format(_format, error) }, JsonRequestBehavior.AllowGet);
                    //ViewBag.ValidationMsg = error;
                    //ShowMessage("error", error, true);
                }
                return Json(new { status = "", Data = "", Message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { status = "", Data = "", Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion TayseerExistingAccountEdit

        #region GetExistingAccountList

        private List<string> GetExistingAccountList(DataTable dt)
        {
            List<string> accountList = new List<string>();
            try
            {
                foreach (DataRow drow in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(drow["AccountNumber"])))
                        accountList.Add(Convert.ToString(drow["AccountNumber"]));
                }
                return accountList;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return accountList;
            }
        }

        #endregion GetExistingAccountList

        #region MergeExistingTables

        private DataTable MergeExistingTables(DataTable dtRefs, DataTable dtOutstanding)
        {
            try
            {
                DataRow rowRef = null;
                DataRow[] rowOutstandingCol = null;
                //decimal amount = decimal.Zero;

                if (!dtRefs.Columns.Contains("OutstandingAmount"))
                {
                    dtRefs.Columns.Add("OutstandingAmount", typeof(string));
                    //if (radTabStrip.SelectedTab.Value != "0")
                    dtRefs.Columns.Add("FinalBill", typeof(string));
                    dtRefs.Columns.Add("Indicator", typeof(string));
                }
                for (int count = 0; count < dtRefs.Rows.Count; count++)
                {
                    rowRef = dtRefs.Rows[count];
                    rowOutstandingCol = dtOutstanding.Select("AccountNumber='" + rowRef["AccountNumber"].ToString() + "'");

                    if (rowOutstandingCol != null)
                    {
                        if (rowRef["AccountNumber"] != null && rowOutstandingCol[0]["AccountNumber"] != null)
                        {
                            if (rowRef["AccountNumber"].ToString() == Convert.ToInt64(rowOutstandingCol[0]["AccountNumber"]).ToString())
                            {
                                dtRefs.Rows[count]["OutstandingAmount"] = rowOutstandingCol[0]["Total"].ToString().Trim();
                                dtRefs.Rows[count]["FinalBill"] = rowOutstandingCol[0]["FinalBill"].ToString().Trim();
                                dtRefs.Rows[count]["Indicator"] = rowOutstandingCol[0].Table.Columns.Contains("Indicator") ? rowOutstandingCol[0]["Indicator"].ToString().Trim() : "";
                                if (Convert.ToDecimal(dtRefs.Rows[count]["AmountToPay"]) <= 0 && Convert.ToDecimal(dtRefs.Rows[count]["OutstandingAmount"]) > 0)
                                {
                                    dtRefs.Rows[count]["AmountToPay"] = dtRefs.Rows[count]["OutstandingAmount"];
                                }
                            }
                        }
                    }
                }
                return dtRefs;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return dtRefs;
            }
        }

        #endregion MergeExistingTables

        #region TayseerFileUpload

        [HttpGet]
        public ActionResult TayseerFileUpload()
        {
            try
            {
                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUpload.cshtml");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUpload.cshtml");
            }
        }

        #endregion TayseerFileUpload

        #region TayseerFileUploadPost

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerFileUploadPost()
        {
            try
            {
                DataTable dtExcelRecords = null;
                DataTable dtErrors = null;
                DataTable dtOutstandingAmount = null;
                DataTable dtMerged = null;
                List<string> lstAccounts = null;
                string mimeType = string.Empty;
                ViewBag.ReviewUploadURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_PAYMENT_REVIEW_UPLOAD);

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    mimeType = file.ContentType;

                    if (mimeType != "application/vnd.ms-excel" && mimeType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        //ViewBag.Message = string.Format("Please upload a file with xls or xlsx format.");
                        ViewBag.Message = Translate.Text("ValidFileUploadFile");
                        ViewBag.RedirectError = "1";
                        return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                    }

                    //Importing Excel
                    dtExcelRecords = TayseerHelper.ImportENBDExcel(file.ContentType, file.InputStream);

                    //Validating Excel
                    dtErrors = TayseerHelper.ValidateENBDExcel(dtExcelRecords);

                    List<TayseerModel> tayseerErrorList = new List<TayseerModel>();
                    List<TayseerFileUploadModel> tayseerFileUploadList = new List<TayseerFileUploadModel>();

                    if (dtErrors != null)
                    {
                        if (dtErrors.Rows.Count > 0)
                        {
                            ErrorTable = dtErrors;

                            foreach (DataRow row in dtErrors.Rows)
                            {
                                var tayseerError = new TayseerModel();
                                tayseerError.LineNo = row["LineNo"].ToString();
                                tayseerError.Error = row["Error"].ToString();
                                tayseerErrorList.Add(tayseerError);
                            }

                            ViewBag.Errorlist = tayseerErrorList;
                            return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                        }
                        else if (dtExcelRecords == null || dtExcelRecords.Rows.Count == 0)
                        {
                            //ViewBag.Message = string.Format("No record(s) found.");
                            ViewBag.Message = Translate.Text("ValidUploadNoRecord");
                            return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                        }
                        else
                        {
                            lstAccounts = GetAccountList(dtExcelRecords);
                            dtOutstandingAmount = GetOutstandingAmount(lstAccounts);
                            dtMerged = MergeTables(dtExcelRecords, dtOutstandingAmount);
                            FileUploadTable = dtMerged;
                            if (FileUploadTable.Select("Indicator = 'I' OR Indicator = 'S'").Length > 0)
                            {
                                var i = 1;
                                foreach (DataRow item in FileUploadTable.Select("Indicator = 'I' OR Indicator = 'S'"))
                                {
                                    var tayseerError = new TayseerModel();
                                    tayseerError.LineNo = i.ToString();
                                    tayseerError.Data = item["ContractAccount"].ToString();
                                    tayseerError.Error = TayseerHelper.GetENBDErrorMessage("CA_INVALID");
                                    tayseerErrorList.Add(tayseerError);
                                    i++;
                                }
                                ViewBag.Errorlist = tayseerErrorList;
                            }
                            else
                            {
                                int Srno = 1;

                                foreach (DataRow row in dtMerged.Rows)
                                {
                                    var tayseerFileUpload = new TayseerFileUploadModel();
                                    tayseerFileUpload.SrNo = Srno++;
                                    tayseerFileUpload.ContractAccountNo = row["ContractAccount"].ToString();
                                    tayseerFileUpload.OutstandingAmount = row["OutstandingAmount"].ToString();
                                    tayseerFileUpload.isSelected = true;

                                    if (Convert.ToDecimal(row["AmountToPay"].ToString()) < 0)
                                    {
                                        tayseerFileUpload.AmounttoPay = Convert.ToDecimal("0").ToString("F");
                                    }
                                    else
                                    {
                                        tayseerFileUpload.AmounttoPay = Convert.ToDecimal(row["AmountToPay"]).ToString("F");
                                    }
                                    tayseerFileUploadList.Add(tayseerFileUpload);
                                }
                                ViewBag.SuccessData = tayseerFileUploadList;
                                ViewBag.SetTotal = SetTotals(dtMerged);
                            }

                            // return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                        }
                    }
                    else
                    {
                        ViewBag.Message = Translate.Text("ValidFileUploadFile");
                        ViewBag.RedirectError = "1";
                        return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                    }
                }
                else
                {
                    //ViewBag.Message = string.Format("Please select a file to upload");
                    ViewBag.Message = Translate.Text("ValidUploadFileSelect");
                    ViewBag.RedirectError = "1";
                    return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
        }

        #endregion TayseerFileUploadPost

        #region TayseerEditRow

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerEditRow(string ContractAccountNo, string AmounttoPay)
        {
            try
            {
                decimal amount = decimal.Zero;
                DataRow[] rowColl = null;
                string error = string.Empty;
                List<TayseerFileUploadModel> tayseerFileUploadList = new List<TayseerFileUploadModel>();
                string _format = "<ul class='parsley-errors-list filled'></li class='parsley-range'>{0}</li></ul>";

                if (TayseerHelper.ValidateBeforeAdd(ContractAccountNo, AmounttoPay, out error))
                {
                    //dt = FileUploadTable;
                    if (FileUploadTable != null && FileUploadTable.Rows.Count > 0)
                    {
                        rowColl = FileUploadTable.Select("ContractAccount=" + "'" + ContractAccountNo.Trim() + "'");
                        //rowColl = FileUploadTable.Select("ContractAccount=" + ContractAccountNo.Trim());
                        if (decimal.TryParse(AmounttoPay.Trim(), out amount))
                        {
                            rowColl[0]["AmountToPay"] = AmounttoPay.Trim();
                            FileUploadTable.AcceptChanges();

                            //ViewBag.SetTotal = SetTotals(FileUploadTable);
                            var total = SetTotals(FileUploadTable);
                            return Json(new { status = true, Data = total, Message = "" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = false, Data = "", Message = string.Format(_format, Translate.Text("ValidAmount")) }, JsonRequestBehavior.AllowGet);
                            //ViewBag.ValidationMsg = "Please enter valid amount";
                            // ShowMessage(GetText("MsgEnterValidAmount"), error, true);
                        }
                    }
                }
                else
                {
                    return Json(new { status = false, Data = "", Message = string.Format(_format, error) }, JsonRequestBehavior.AllowGet);
                    //ViewBag.ValidationMsg = error;
                    //ShowMessage("error", error, true);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return Json(new { status = "", Data = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }

        #endregion TayseerEditRow

        #region TayseerDeleteRow

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerDeleteRow(string ContractAccountNo)
        {
            try
            {
                DataRow[] rowColl = null;
                string error = string.Empty;
                List<TayseerFileUploadModel> tayseerFileUploadList = new List<TayseerFileUploadModel>();
                ViewBag.ReviewUploadURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_PAYMENT_REVIEW_UPLOAD);

                if (TayseerHelper.Validateaccountnumber(ContractAccountNo, out string errorMsg, out bool retVal))
                {
                    if (FileUploadTable != null && FileUploadTable.Rows.Count > 0)
                    {
                        rowColl = FileUploadTable.Select("ContractAccount=" + ContractAccountNo.Trim());
                        foreach (DataRow row in rowColl)
                        {
                            FileUploadTable.Rows.Remove(row);
                        }
                        int Srno = 1;

                        foreach (DataRow row in FileUploadTable.Rows)
                        {
                            var tayseerFileUpload = new TayseerFileUploadModel();
                            tayseerFileUpload.SrNo = Srno++;
                            tayseerFileUpload.ContractAccountNo = row["ContractAccount"].ToString();
                            tayseerFileUpload.OutstandingAmount = row["OutstandingAmount"].ToString();
                            tayseerFileUpload.AmounttoPay = row["AmountToPay"].ToString();
                            tayseerFileUpload.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                            tayseerFileUploadList.Add(tayseerFileUpload);
                        }
                        ViewBag.SuccessData = tayseerFileUploadList;
                        ViewBag.SetTotal = SetTotals(FileUploadTable);
                        FileUploadTable.AcceptChanges();
                    }
                    else
                    {
                        // ShowMessage("error", error, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
        }

        #endregion TayseerDeleteRow

        #region TayseerUploadAccountcheckedAll
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerUploadAccountcheckedAll(bool AccountCheck)
        {
            try
            {
                DataRow[] rowColl = null;

                if (FileUploadTable != null && FileUploadTable.Rows.Count > 0)
                {
                    rowColl = FileUploadTable.Select();
                    foreach (DataRow row in rowColl)
                    {
                        row["Selected"] = AccountCheck;
                    }
                    var total = SetTotals(FileUploadTable);
                    return Json(new { status = true, Data = total }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region TayseerUploadAccountchecked
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerUploadAccountchecked(string ContractAccount, bool AccountCheck)
        {
            try
            {
                DataRow rowColl = null;

                if (!String.IsNullOrEmpty(ContractAccount) && FileUploadTable != null && FileUploadTable.Rows.Count > 0)
                {
                    rowColl = FileUploadTable.Select("ContractAccount=" + "'" + ContractAccount.Trim() + "'").FirstOrDefault();
                    if (rowColl != null)
                    {
                        rowColl["Selected"] = AccountCheck;
                        var total = SetTotals(FileUploadTable);
                        return Json(new { status = true, Data = total }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { status = false, Data = "Failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region TayseerAddAccountFileUploadTable
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerAddAccountFileUploadTable(string ContractAccountNo, string AmounttoPay, string selectedAccount)
        {
            string error = string.Empty;
            List<TayseerFileUploadModel> tayseerAddAccountFileUploadList = new List<TayseerFileUploadModel>();
            try
            {
                List<string> accountList = new List<string>();
                DataTable dtOutstanding = null;
                DataTable dtMerged = null;
                DataRow drow = null;
                DataTable dataTable = null;
                string Oustandingamount = string.Empty;
                string serviceError = string.Empty;
                dataTable = FileUploadTable;

                if (TayseerHelper.ValidateBeforeAdd(ContractAccountNo, AmounttoPay, out error))
                {
                    if (GetBillDetails(ContractAccountNo, out Oustandingamount, out serviceError))
                    {
                        if (dataTable != null)
                        {
                            if (dataTable.Select("ContractAccount = '" + ContractAccountNo + "'").Length == 0)
                            {
                                drow = dataTable.NewRow();
                                drow["ContractAccount"] = ContractAccountNo;
                                drow["AmountToPay"] = AmounttoPay;
                                if (dataTable.Columns["Selected"] != null)
                                    drow["Selected"] = true;
                                dataTable.Rows.InsertAt(drow, 0);

                                accountList = GetExistingAccountList(dataTable);

                                dtOutstanding = GetOutstandingAmount(accountList);
                                dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                                dataTable = dtMerged;
                                FileUploadTable = dataTable;
                                ViewBag.SetTotal = SetTotals(dtMerged);
                            }
                            else
                            {
                                //ViewBag.Message = string.Format("The entered account is already in the list");
                                ViewBag.Message = Translate.Text("ValidationAlreadyList");
                                ViewBag.SetTotal = SetTotals(dataTable);
                            }
                        }
                    }
                    else
                    {
                        accountList = GetExistingAccountList(dataTable);
                        dtOutstanding = GetOutstandingAmount(accountList);
                        dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                        dataTable = dtMerged;
                        FileUploadTable = _AddselectedAccountTable(dataTable, selectedAccount);
                        ViewBag.SetTotal = SetTotals(dtMerged);
                        ViewBag.Message = serviceError;
                    }
                }
                else
                {
                    accountList = GetExistingAccountList(dataTable);
                    dtOutstanding = GetOutstandingAmount(accountList);
                    dtMerged = MergeExistingTables(dataTable, dtOutstanding);
                    dataTable = dtMerged;
                    FileUploadTable = _AddselectedAccountTable(dataTable, selectedAccount);
                    ViewBag.SetTotal = SetTotals(dtMerged);
                    ViewBag.Message = error;
                }
                foreach (DataRow row in FileUploadTable.Rows)
                {
                    var tayseerExistingAccount = new TayseerFileUploadModel();
                    tayseerExistingAccount.ContractAccountNo = row["ContractAccount"].ToString();
                    tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();
                    tayseerExistingAccount.AmounttoPay = row["AmountToPay"].ToString();
                    tayseerExistingAccount.isSelected = Convert.ToBoolean(row["Selected"].ToString());
                    tayseerAddAccountFileUploadList.Add(tayseerExistingAccount);
                }
                ViewBag.SuccessData = tayseerAddAccountFileUploadList;
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return PartialView("~/Views/Feature/Bills/Tayseer/TayseerFileUploadPartial.cshtml");
            }
        }
        #endregion

        #region GetAccountList

        private List<string> GetAccountList(DataTable dt)
        {
            List<string> accountList = new List<string>();
            try
            {
                foreach (DataRow drow in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(drow["ContractAccount"])))
                        accountList.Add(Convert.ToString(drow["ContractAccount"]));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return accountList;
        }

        #endregion GetAccountList

        #region MergeTables

        private DataTable MergeTables(DataTable dtRefs, DataTable dtOutstanding)
        {
            try
            {
                DataRow rowRef = null;
                DataRow[] rowOutstandingCol = null;

                if (!dtRefs.Columns.Contains("OutstandingAmount"))
                {
                    dtRefs.Columns.Add("OutstandingAmount", typeof(string));
                    //if (radTabStrip.SelectedTab.Value != "0")
                    dtRefs.Columns.Add("FinalBill", typeof(string));
                    dtRefs.Columns.Add("Indicator", typeof(string));
                    if (!dtRefs.Columns.Contains("Selected"))
                        dtRefs.Columns.Add("Selected", typeof(bool));
                }

                for (int count = 0; count < dtRefs.Rows.Count; count++)
                {
                    rowRef = dtRefs.Rows[count];
                    rowOutstandingCol = dtOutstanding.Select("AccountNumber='" + rowRef["ContractAccount"].ToString() + "'");

                    if (rowOutstandingCol != null)
                    {
                        if (rowRef["ContractAccount"] != null && rowOutstandingCol[0]["AccountNumber"] != null)
                        {
                            if (rowRef["ContractAccount"].ToString() == Convert.ToInt64(rowOutstandingCol[0]["AccountNumber"]).ToString())
                            {
                                dtRefs.Rows[count]["OutstandingAmount"] = rowOutstandingCol[0]["Total"].ToString().Trim();
                                dtRefs.Rows[count]["FinalBill"] = rowOutstandingCol[0]["FinalBill"].ToString().Trim();
                                dtRefs.Rows[count]["Indicator"] = rowOutstandingCol[0]["Indicator"].ToString().Trim();
                                dtRefs.Rows[count]["Selected"] = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return dtRefs;
        }

        #endregion MergeTables

        #region GetOutstandingAmount

        private DataTable GetOutstandingAmount(List<string> lstAccounts)
        {
            DataSet ds = null;
            DataTable dtOutstanding = null;
            try
            {
                var response = DewaApiClient.GetOutstandingAmount(lstAccounts.ToArray(), RequestLanguage, Request.Segment());

                ds = TayseerHelper.ToDataSet(response);

                if (ds != null && ds.Tables.Count > 1)
                {
                    dtOutstanding = ds.Tables[1];
                }
                else if (ds != null && ds.Tables[0].Rows[0]["ResponseCode"].ToString() == "000")
                {
                    dtOutstanding = ds.Tables[0];
                    //ShowMessage("emptyCANo", Convert.ToString(ds.Tables[0].Rows[0]["Description"]), true);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return dtOutstanding;
        }

        #endregion GetOutstandingAmount

        #region SetTotals

        private string SetTotals(DataTable dataTable)
        {
            try
            {
                if (dataTable != null)
                {
                    object sum = null;
                    int count = 0;
                    decimal grandTotal = decimal.Zero;

                    if (dataTable.Columns["Selected"] == null)
                    {
                        sum = dataTable.Compute("Sum(AmountToPay)", "AmountToPay > 0");
                        count = dataTable.Rows.Count;
                        decimal.TryParse(sum.ToString(), out grandTotal);
                    }
                    else
                    {
                        DataView dv = new DataView(dataTable);
                        dv.RowFilter = "Selected=True";
                        DataTable dt = dv.ToTable();
                        decimal.TryParse(dt.Compute("Sum(AmountToPay)", "AmountToPay > 0").ToString(), out grandTotal);
                        count = dt.Rows.Count;
                    }
                    //var SetTotal = Translate.Text("Total Accounts") + ": <strong>" + count.ToString() + "</strong>" + Translate.Text("Commas") + " " + Translate.Text("Total Amount") + ": <strong>" + string.Format("{0:#,###0.00}", grandTotal) + " " + Translate.Text("Currency") + "</strong>";
                    var SetTotal = " <div class='total-ac'>" + Translate.Text("Total Accounts") + ": <span class='total-ac-num'>" + count.ToString() + "</span></div><div class='total-am'>" + Translate.Text("Total Amount") + ": <span class='total-am-num'>" + string.Format("{0:#,###0.00}", grandTotal) + " " + Translate.Text("Currency") + "</span></div>";
                    //var SetTotal = "Total Accounts: <span class='mustbold'>" + count.ToString() + " </span>," + "Total Amount: <span class='mustbold'>"
                    //                 + grandTotal.ToString("#,0.00") + "</span>";
                    ViewBag.GrandTotal = string.Format("{0:#,###0.00}", grandTotal);
                    return SetTotal;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return string.Empty;
            }
        }

        #endregion SetTotals

        #region SetReviewTotals

        //private string SetReviewTotals(DataTable dataTable)
        //{
        //    try
        //    {
        //        if (dataTable != null)
        //        {
        //            object sum = null;
        //            int count = 0;
        //            decimal grandTotal = decimal.Zero;
        //            dataTable.Columns[1].DataType = typeof(Decimal);

        //            if (dataTable.Columns["Selected"] == null)
        //            {
        //                sum = dataTable.Compute("Sum(Amount)", "Amount > 0");
        //                count = dataTable.Rows.Count;
        //                decimal.TryParse(sum.ToString(), out grandTotal);
        //            }
        //            else
        //            {
        //                DataView dv = new DataView(dataTable);
        //                dv.RowFilter = "Selected=True";
        //                DataTable dt = dv.ToTable();
        //                decimal.TryParse(dt.Compute("Sum(Amount)", "Amount > 0").ToString(), out grandTotal);
        //                count = dt.Rows.Count;
        //            }
        //            var SetTotal = Translate.Text("Total Accounts") + ": " + count.ToString() + ", " + Translate.Text("Total Amount") + ": " + grandTotal.ToString("#,0.00");
        //            //var SetTotal = "Total Accounts: <span class='mustbold'>" + count.ToString() + " </span>," + "Total Amount: <span class='mustbold'>"
        //            //                 + grandTotal.ToString("#,0.00") + "</span>";
        //            ViewBag.GrandTotal = grandTotal.ToString("#,0.00");
        //            return SetTotal;
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion SetReviewTotals

        #region TayseerReviewPayment

        [HttpGet]
        public ActionResult TayseerReviewPayment()
        {
            if (!IsLoggedIn)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

            TayseerFileUploadDetail _uploadDetail = new TayseerFileUploadDetail();
            try
            {
                int Srno = 1;
                List<TayseerFileUploadModel> tayseerFileUploadList = new List<TayseerFileUploadModel>();
                List<SelectListItem> items = new List<SelectListItem>();
                LoadForm();
                items = GetnBindCustomerDetails();
                _uploadDetail._emaildropdown = items;

                ViewBag.SetTotal = SetTotals(AccountsTable);
                List<TayseerFileUploadModel> tayseerExistingAddAccountList = new List<TayseerFileUploadModel>();
                foreach (DataRow row in AccountsTable.Rows)
                {
                    //if (row["Selected"].ToString() == "True")
                    //{
                    var tayseerExistingAccount = new TayseerFileUploadModel();
                    tayseerExistingAccount.SrNo = Srno++;
                    tayseerExistingAccount.ContractAccountNo = row["ContractAccount"].ToString();
                    //tayseerExistingAccount.OutstandingAmount = row["OutstandingAmount"].ToString();
                    tayseerExistingAccount.OutstandingAmount = string.IsNullOrEmpty(row["OutstandingAmount"].ToString()) ? "0.00" : row["OutstandingAmount"].ToString();
                    tayseerExistingAccount.AmounttoPay = row["AmountToPay"].ToString();
                    tayseerExistingAccount.Remarks = row["Remarks"].ToString();
                    tayseerExistingAddAccountList.Add(tayseerExistingAccount);
                    //}
                }

                _uploadDetail.TayseerFileUploadDetails = tayseerExistingAddAccountList;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return PartialView("~/Views/Feature/Bills/Tayseer/TayseerReviewPayment.cshtml", _uploadDetail);
        }

        #endregion TayseerReviewPayment

        #region TayseerReviewPaymentClick

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerReviewPaymentClick(string selectedTab)
        {
            try
            {
                List<string> accountList = null;
                List<string> amountList = null;
                List<string> finalBillList = null;
                DataTable dtResponse = null;
                DataTable dtAccounts = null;
                DataRow rowRes = null;
                string error = string.Empty;

                //File Upload
                if (selectedTab == "fileUploadTab" && FileUploadTable != null)
                {
                    if (FileUploadTable.Select("Indicator = 'I' OR Indicator = 'S'").Length > 0)
                    {
                        return Json(new { status = false, Data = "", Message = Translate.Text("ValidUploadReview") }, JsonRequestBehavior.AllowGet);
                    }
                }

                //Existing Accounts
                if (selectedTab == "existingAccountsTab" && AccountsTable != null && AccountsTable.Select("Selected = 'true'").Count() > 0)
                {
                    DataTable dtSelect = null;
                    dtSelect = AccountsTable.Select("Selected = 'true'").CopyToDataTable();
                    if (dtSelect.Select("Selected = 'true'").Length > 0)
                    {
                        if (dtSelect.Select("Indicator = 'I' OR Indicator = 'S'").Length > 0)
                        {
                            return Json(new { status = false, Data = "", Message = Translate.Text("ValidUploadReview") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                //Existing Accounts
                if (selectedTab == "existingAccountsTab")
                    GetDataList(out accountList, out amountList, out finalBillList, out error);
                else if (selectedTab == "fileUploadTab") //File Upload
                    GetDataListFileUpload(out accountList, out amountList, out finalBillList);

                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { status = false, Data = "", Message = error }, JsonRequestBehavior.AllowGet);
                }

                decimal total = amountList.Sum(x => Convert.ToDecimal(x));

                if (total > maxAllowedAmount)
                {
                    return Json(new { status = false, Data = "", Message = Translate.Text("ValidMaxAmount") }, JsonRequestBehavior.AllowGet);
                }

                if (total == Convert.ToDecimal(0))
                {
                    return Json(new { status = false, Data = "", Message = Translate.Text("PleaseEnterValidZero") }, JsonRequestBehavior.AllowGet);
                }


                if (accountList != null && accountList.Count() > 1)
                {
                    ValidateAccounts(accountList, amountList, finalBillList, out dtResponse, out dtAccounts);
                    if (dtResponse != null && dtResponse.Rows.Count > 0)
                    {
                        rowRes = dtResponse.Rows[0];
                        if (rowRes["ResponseCode"].ToString() == "000" || rowRes["ResponseCode"].ToString() == "350")
                        {
                            Session["accountlist"] = accountList;
                            Session["amountlist"] = amountList;
                            Session["finalbilllist"] = finalBillList;
                            return Json(new { status = true, Data = "", Message = "Success" }, JsonRequestBehavior.AllowGet);
                            //Response.Redirect("~/Modules/Business/PaymentReview.aspx?tab=" + selectedTab);
                        }
                        else
                        {
                            return Json(new { status = false, Data = "", Message = rowRes["Description"].ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if (accountList.Count == 1)
                {
                    return Json(new { status = false, Data = "", Message = Translate.Text("ValidSingleAccount") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return Json(new { status = false, Data = "", Message = Translate.Text("InvalidAccountSelection") }, JsonRequestBehavior.AllowGet);
        }

        #endregion TayseerReviewPaymentClick

        #region GetDataList

        private void GetDataList(out List<string> lstAccounts, out List<string> lstAmounts, out List<string> lstFinalBill, out string error)
        {
            try
            {
                List<string> accountList = null;
                List<string> amountList = null;
                List<string> finalBillList = null;
                decimal amount = decimal.Zero;
                DataRow[] rowsColl = null;
                error = string.Empty;

                if (AccountsTable != null)
                {
                    accountList = new List<string>();
                    amountList = new List<string>();
                    finalBillList = new List<string>();

                    rowsColl = AccountsTable.Select("Selected = True");

                    foreach (DataRow drow in rowsColl)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(drow["AmountToPay"])))
                        {
                            decimal.TryParse(Convert.ToString(drow["AmountToPay"]), out amount);
                            if (amount > 0)
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(drow["AccountNumber"])))
                                    accountList.Add("00" + Convert.ToString(drow["AccountNumber"]));

                                if (String.IsNullOrEmpty(Convert.ToString(drow["FinalBill"])))
                                    finalBillList.Add(string.Empty);
                                else
                                    finalBillList.Add("X");

                                amountList.Add(Convert.ToString(drow["AmountToPay"]));
                            }
                        }
                    }
                    if (accountList.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(Request.UrlReferrer.Query))
                        {
                            error = Translate.Text("ValidSingleAccount");
                        }
                        else
                        {
                            error = Translate.Text("ValidatLeastAccount");
                        }
                    }
                }
                lstAccounts = accountList;
                lstAmounts = amountList;
                lstFinalBill = finalBillList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion GetDataList

        #region GetDataListFileUpload

        private void GetDataListFileUpload(out List<string> lstAccounts, out List<string> lstAmounts, out List<string> lstFinalBill)
        {
            try
            {
                List<string> accountList = null;
                List<string> amountList = null;
                List<string> finalBillList = null;
                decimal amount = decimal.Zero;
                DataRow[] rowsColl = null;

                if (FileUploadTable != null)
                {
                    accountList = new List<string>();
                    amountList = new List<string>();
                    finalBillList = new List<string>();
                    rowsColl = FileUploadTable.Select("Selected = True");

                    foreach (DataRow drow in rowsColl)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(drow["AmountToPay"])))
                        {
                            decimal.TryParse(Convert.ToString(drow["AmountToPay"]), out amount);
                            if (amount > 0)
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(drow["ContractAccount"])))
                                    accountList.Add("00" + Convert.ToString(drow["ContractAccount"]));

                                if (String.IsNullOrEmpty(Convert.ToString(drow["FinalBill"])))
                                    finalBillList.Add(string.Empty);
                                else
                                    finalBillList.Add("X");

                                amountList.Add(Convert.ToString(drow["AmountToPay"]));
                            }
                        }
                    }
                }
                lstAccounts = accountList;
                lstAmounts = amountList;
                lstFinalBill = finalBillList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion GetDataListFileUpload

        #region ValidateAccounts

        public DataSet ValidateAccounts(List<string> accountList, List<string> amountList, List<string> finalbillList, out DataTable dtResponse, out DataTable dtAccounts)
        {
            DataSet dsRemarks = null;
            string script = string.Empty;
            dtResponse = null;
            dtAccounts = null;
            try
            {
                var response = DewaApiClient.ValidateAccounts(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, accountList.ToArray(), amountList.ToArray(), finalbillList.ToArray(), RequestLanguage, Request.Segment());
                dsRemarks = TayseerHelper.ToDataSet(response);

                if (dsRemarks != null)
                {
                    dtResponse = (dsRemarks.Tables.Count > 0) ? dsRemarks.Tables[0] : null;
                    dtAccounts = (dsRemarks.Tables.Count > 1) ? dsRemarks.Tables[1] : null;
                }
                return dsRemarks;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return dsRemarks;
            }
        }

        #endregion ValidateAccounts

        #region GetnBindCustomerDetails

        private List<SelectListItem> GetnBindCustomerDetails()
        {
            //DataTable dt = null;
            //ListItem item = null;
            List<BusinessPartner> _business = new List<BusinessPartner>();
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                var response = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    _business = response.Payload.BusinessPartners;
                }
                if (_business.Count > 0)
                {
                    items.Add(new SelectListItem { Text = Translate.Text("Please Select"), Value = "" });

                    items.Add(new SelectListItem { Text = Translate.Text("Others"), Value = "-2" });

                    foreach (var _item in _business)
                    {
                        items.Add(new SelectListItem { Text = _item.email + " | " + _item.mobilenumber, Value = _item.email + " | " + _item.mobilenumber });
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return items;
            }
        }

        #endregion GetnBindCustomerDetails

        #region TayseerGenerateReference

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult lnkGenerateReference()
        {
            try
            {
                string strDDLValue = Request.Form["drpEmailDropdown"].ToString();

                List<string> accountList = null;
                List<string> amountList = null;
                List<string> finalBillList = null;
                string script = string.Empty;
                string email = string.Empty;
                string mobileNo = string.Empty;
                string[] selectedVal = null;
                char[] splitChar = new char[] { '|' };
                string chequePaymentAllowed = string.Empty;
                string error = string.Empty;
                //DataTable dtResponse = null;
                //DataTable dtAccounts = null;
                //DataRow rowRes = null;

                string _chequePayment = Request.Form["hdnPaymentchecked"].ToString();

                if (_chequePayment == "true")
                    chequePaymentAllowed = "True";

                if (!String.IsNullOrEmpty(strDDLValue) && strDDLValue != "-2")
                {
                    selectedVal = strDDLValue.Split(splitChar);

                    if (selectedVal[0] != null)
                        email = selectedVal[0];

                    if (selectedVal[1] != null)
                        mobileNo = selectedVal[1];
                }
                else if (strDDLValue == "-2")
                {
                    email = Request.Form["emailaddress"].ToString();
                    //mobileNo = Request.Form["mobilecode"].ToString() + Request.Form["mobilenumber"].ToString();
                    mobileNo = "0" + Request.Form["mobilenumber"].ToString();
                }
                List<DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist> referenceaccountlist = new List<DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist>();

                GetGenerateDataList(out accountList, out amountList, out finalBillList, out error);
                GetRefAccountList(out referenceaccountlist, out error);

                var responseData = TayseerClient.TayseerCreateReference(new DEWAXP.Foundation.Integration.Requests.Tayseer.TayseerDetailsRequest
                {
                    referencenumberinputs = new DEWAXP.Foundation.Integration.Requests.Tayseer.Referencenumberinputs
                    {
                        referenceaccountlist = referenceaccountlist,
                        emailid = email,
                        mobilenumber = mobileNo,
                    }
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);
                // DataSet dsRemarks = TayseerHelper.ToDataSet(response);

                if (responseData != null && responseData.Succeeded)
                {
                    var _urlbyID = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_GENERATE_REF_NO);
                    Session["DewaReference"] = responseData.Payload.dewareferencenumber;// rowRes["DewaReference"].ToString();
                    Session["TotalAccounts"] = responseData.Payload.totalaccounts; // rowRes["TotalAccounts"].ToString();
                    Session["TotalAmount"] = responseData.Payload.totalamount;// rowRes["TotalAmount"].ToString();
                    Session["DateTime"] = DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                    Session["chequePaymentAllowed"] = chequePaymentAllowed;
                    return Redirect(_urlbyID);
                }
                else
                {
                    ViewBag.ErrorMessage = responseData.Message;
                }

                //var response = DewaApiClient.GenerateRefNo(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, accountList.ToArray(), amountList.ToArray(), finalBillList.ToArray(), email, mobileNo.Trim(), RequestLanguage, Request.Segment());
                //DataSet dsRemarks = TayseerHelper.ToDataSet(response);

                //if (dsRemarks != null && dsRemarks.Tables.Count > 0)
                //{
                //    if (dsRemarks != null)
                //    {
                //        dtResponse = (dsRemarks.Tables.Count > 0) ? dsRemarks.Tables[0] : null;
                //        dtAccounts = (dsRemarks.Tables.Count > 1) ? dsRemarks.Tables[1] : null;
                //    }
                //    rowRes = dtResponse.Rows[0];
                //    if (rowRes["ResponseCode"].ToString() == "000" && !String.IsNullOrEmpty(rowRes["DewaReference"].ToString()))
                //    {
                //        var _urlbyID = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_GENERATE_REF_NO);
                //        Session["DewaReference"] = rowRes["DewaReference"].ToString();
                //        Session["TotalAccounts"] = rowRes["TotalAccounts"].ToString();
                //        Session["TotalAmount"] = rowRes["TotalAmount"].ToString();
                //        Session["DateTime"] = DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                //        Session["chequePaymentAllowed"] = chequePaymentAllowed;
                //        return Redirect(_urlbyID);
                //        //return Redirect(_urlbyID + "?r=" + rowRes["DewaReference"].ToString() +
                //        //                 "&c=" + rowRes["TotalAccounts"].ToString() + "&a=" + rowRes["TotalAmount"].ToString() +
                //        //                 "&d=" + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "&chmsg=" + chequePaymentAllowed);
                //    }
                //    else
                //    {
                //        //lblErrorMsg.Visible = true;
                //        //lblErrorMsg.Text = dicRes["Description"].ToString();
                //        ViewBag.ErrorMessage = rowRes["Description"].ToString();
                //    }
                //}
                //return View();
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TAYSEER_PAYMENT_REVIEW_EXISTING);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TAYSEER_PAYMENT_REVIEW_EXISTING); ;
            }
        }

        #endregion TayseerGenerateReference

        #region GetGenerateDataList

        private void GetGenerateDataList(out List<string> lstAccounts, out List<string> lstAmounts, out List<string> lstFinalBill, out string error)
        {
            try
            {
                List<string> accountList = null;
                List<string> amountList = null;
                List<string> finalBillList = null;
                decimal amount = decimal.Zero;
                DataRow[] rowsColl = null;
                error = string.Empty;

                if (AccountsTable != null)
                {
                    accountList = new List<string>();
                    amountList = new List<string>();
                    finalBillList = new List<string>();

                    rowsColl = AccountsTable.Select("Selected = True");

                    foreach (DataRow drow in rowsColl)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(drow["AmountToPay"])))
                        {
                            decimal.TryParse(Convert.ToString(drow["AmountToPay"]), out amount);
                            if (amount > 0)
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(drow["ContractAccount"])))
                                    accountList.Add("00" + Convert.ToString(drow["ContractAccount"]));

                                if (String.IsNullOrEmpty(Convert.ToString(drow["FinalBill"])))
                                    finalBillList.Add(string.Empty);
                                else
                                    finalBillList.Add("X");

                                amountList.Add(Convert.ToString(drow["AmountToPay"]));
                            }
                        }
                    }
                    if (accountList.Count == 0)
                    {
                        error = Translate.Text("ValidatLeastAccount");
                    }
                }

                lstAccounts = accountList;
                lstAmounts = amountList;
                lstFinalBill = finalBillList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void GetRefAccountList(out List<DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist> lstrefAccounts, out string error)
        {
            List<DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist> accountList = null;
            error = string.Empty;
            try
            {
                decimal amount = decimal.Zero;
                DataRow[] rowsColl = null;
                if (AccountsTable != null)
                {
                    accountList = new List<DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist>();
                    rowsColl = AccountsTable.Select("Selected = True");
                    foreach (DataRow drow in rowsColl)
                    {
                        decimal.TryParse(Convert.ToString(drow["AmountToPay"]), out amount);
                        if (amount > 0)
                        {
                            DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist refAct = new DEWAXP.Foundation.Integration.Requests.Tayseer.Referenceaccountlist();
                            if (!String.IsNullOrEmpty(Convert.ToString(drow["ContractAccount"])))
                            {
                                refAct.contractaccount = "00" + Convert.ToString(drow["ContractAccount"]);
                            }

                            if (String.IsNullOrEmpty(Convert.ToString(drow["FinalBill"])))
                            {
                                refAct.finalbillflag = string.Empty;
                            }
                            else
                            {
                                refAct.finalbillflag = "X";
                            }
                            refAct.amount = Convert.ToString(drow["AmountToPay"]);
                            refAct.businesspartner = Convert.ToString(drow["BusinessPartner"]);
                            refAct.paidamount = Convert.ToString(drow["PaidAmount"]);
                            refAct.remarks = Convert.ToString(drow["Remarks"]);
                            refAct.remarkscode = Convert.ToString(drow["RemarksCode"]);
                            accountList.Add(refAct);
                        }
                    }
                }
                if (accountList.Count == 0)
                {
                    error = Translate.Text("ValidatLeastAccount");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }

            lstrefAccounts = accountList;
        }
        #endregion GetGenerateDataList

        #region GetBillDetails

        private bool GetBillDetails(string ContractAccountNo, out string Oustandingamount, out string serviceError)
        {
            bool retVal = false;
            Oustandingamount = string.Empty;
            serviceError = string.Empty;
            try
            {
                var response = DewaApiClient.GetBill(ContractAccountNo, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null && response.Payload.ResponseCode == 0)
                {
                    Oustandingamount = response.Payload.Balance.ToString();
                    retVal = true;
                }
                else
                {
                    serviceError = response.Message;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return retVal;
            }
        }

        #endregion GetBillDetails

        #region TayseerDeleteReviewPayment

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerDeleteReviewPayment(string ContractAccountNo)
        {
            try
            {
                DataRow[] rowColl = null;
                string error = string.Empty;
                List<TayseerFileUploadModel> tayseerExistingAddAccountList = new List<TayseerFileUploadModel>();
                if (TayseerHelper.Validateaccountnumber(ContractAccountNo, out string errorMsg, out bool retVal))
                {
                    if (AccountsTable != null && AccountsTable.Rows.Count > 0)
                    {
                        //rowColl = AccountsTable.Select("ContractAccount=" + ContractAccountNo.Trim());
                        rowColl = AccountsTable.Select("ContractAccount=" + "'" + ContractAccountNo.Trim() + "'");
                        foreach (DataRow row in rowColl)
                        {
                            AccountsTable.Rows.Remove(row);
                        }
                        AccountsTable.AcceptChanges();
                        List<string> accounts = AccountsTable.AsEnumerable().Select(r => r.Field<string>("ContractAccount")).ToList();
                        accounts = accounts.Select(a => "00" + a).ToList();
                        List<string> amounts = AccountsTable.AsEnumerable().Select(r => r.Field<string>("Amount")).ToList();
                        //List<string> finalBillAccounts = AccountsTable.AsEnumerable().Select(f => f.Field<string>("FinalBill")).ToList();
                        List<string> finalBillAccounts = AccountsTable.AsEnumerable().Select(f => string.IsNullOrEmpty(f.Field<string>("FinalBill")) ? string.Empty : f.Field<string>("FinalBill")).ToList();

                        DataTable dtRes = null;
                        DataTable dtAccounts = null;
                        DataRow rowRes = null;

                        ValidateAccounts(accounts, amounts, finalBillAccounts, out dtRes, out dtAccounts);
                        if (dtRes != null && dtRes.Rows.Count > 0)
                        {
                            rowRes = dtRes.Rows[0];
                            if (rowRes["ResponseCode"].ToString() == "000" || rowRes["ResponseCode"].ToString() == "350")
                            {
                                Session["accountlist"] = accounts;
                                Session["amountlist"] = amounts;
                                Session["finalbilllist"] = finalBillAccounts;
                                return Json(new { status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, Message = Translate.Text(rowRes["Description"].ToString()) }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { status = false, Message = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // ShowMessage("error", error, true);
                        return Json(new { status = false, Message = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    // ShowMessage("error", error, true);
                    return Json(new { status = false, Message = "Failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return null;
            }
        }

        #endregion TayseerDeleteReviewPayment

        #region LoadForm

        private void LoadForm()
        {
            try
            {
                DataTable dtOutstandingAmount = null;
                DataTable dtResponse = null;
                DataRow rowRes = null;
                DataTable dtAccounts = null;
                DataTable dtMerged = null;
                List<string> lstAccounts = null;

                ValidateAccounts(AccountList, AmountList, FinalBillList, out dtResponse, out dtAccounts);
                if (dtResponse != null && dtResponse.Rows.Count > 0)
                {
                    rowRes = dtResponse.Rows[0];
                    if (rowRes["ResponseCode"].ToString() == "000" || rowRes["ResponseCode"].ToString() == "350")
                    {
                        if (rowRes["ChequePaymentAllowed"].ToString().ToUpper() == "X") //Accounts with NO errors/warnings
                        {
                            ViewBag.ChequePayment = "true";
                        }
                        else
                        {
                            ViewBag.CashPayment = "true";
                        }
                        ViewBag.GenerateRefNo = true;

                        dtAccounts.Columns.Add("Selected", typeof(bool));
                        dtAccounts.Columns.Add("AmountToPay", typeof(decimal));
                        foreach (DataRow row in dtAccounts.Rows)
                        {
                            row["Selected"] = true;
                            row["AmountToPay"] = row["Amount"];
                            //dtAccounts.ImportRow(row);
                        }

                        lstAccounts = GetAccountList(dtAccounts);
                        dtOutstandingAmount = GetOutstandingAmount(lstAccounts);
                        dtMerged = MergeTables(dtAccounts, dtOutstandingAmount);
                        AccountsTable = dtMerged;
                        //SetTotals(AccountsTable);
                    }
                    else
                    {
                        ViewBag.GenerateRefNo = false;
                        ViewBag.ChequePayment = "false";
                        ViewBag.ErrorMessage = rowRes["Description"].ToString();
                        //string script = Helpers.GetAlertRedirectScript(rowRes["Description"].ToString(), "Modules/Business/RefDetails.aspx?refNo=" + RefNo);
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "noselection", script, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        #endregion LoadForm

        #region TayseerGenerateReference

        public ActionResult TayseerGenerateReference()
        {
            try
            {
                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                string error;
                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }


                TayseerPayModel model = new TayseerPayModel();
                model.EasyPayNumber = Session["DewaReference"] != null ? Convert.ToString(Session["DewaReference"]) : string.Empty;
                model.TotalAmount = Session["TotalAmount"] != null ? Convert.ToDecimal(Session["TotalAmount"]) : 0;


                return View("~/Views/Feature/Bills/Tayseer/GenerateReferenceNo.cshtml", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("~/Views/Feature/Bills/Tayseer/GenerateReferenceNo.cshtml");
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerGenerateReference(TayseerPayModel model)
        {
            try
            {
                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                var _fc = FetchFutureCenterValues();
                var response = DewaApiClient.GetEasyPayEnquiry(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.EasyPayNumber, RequestLanguage, Request.Segment(), _fc.Branch);
                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse _response;
                    _response = response.Payload;
                    #region [MIM Payment Implementation]
                    var payRequest = new CipherPaymentModel();
                    payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                    payRequest.PaymentData.contractaccounts = model.EasyPayNumber;
                    payRequest.PaymentData.easypaynumber = model.EasyPayNumber;
                    payRequest.PaymentData.movetoflag = "R";
                    payRequest.PaymentData.easypayflag = "X";
                    payRequest.ServiceType = ServiceType.PayBill;
                    payRequest.PaymentMethod = model.paymentMethod;
                    payRequest.IsThirdPartytransaction = false;
                    payRequest.BankKey = model.bankkey;
                    payRequest.SuqiaValue = model.SuqiaDonation;
                    payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                    #endregion
                    var payResponse = ExecutePaymentGateway(payRequest);
                    if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                    {
                        CacheProvider.Store(CacheKeys.TAYSEER_PAYMENT_PATH, new CacheItem<string>("tayseerpaymentpath"));
                        CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse>(_response));
                        return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View("~/Views/Feature/Bills/Tayseer/GenerateReferenceNo.cshtml", model);
        }
        #endregion TayseerGenerateReference

        #region TayseerGetReferenceNoList

        [HttpGet]
        public ActionResult TayseerGetReferenceNoList()
        {
            try
            {
                if (!IsLoggedIn)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                DataSet ds = null;
                DataTable dtRefs = null;
                List<TayseerReferenceList> _referenceList = new List<TayseerReferenceList>();
                TayseerReferenceListDetail _referenceListDetail = new TayseerReferenceListDetail();
                string status = string.Empty;
                ViewBag.SelectExistingURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_EXISTING_ACCOUNT);

                //var response = DewaApiClient.GetRefNoList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                //ds = TayseerHelper.ToDataSet(response);

                //if (ds != null && ds.Tables.Count > 1)
                //{
                //    dtRefs = ds.Tables[1];
                //}
                var responseData = TayseerClient.TayseerList(new TayseerDetailsRequest(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);

                if (responseData != null && responseData.Payload != null && responseData.Succeeded)
                {
                    if (responseData.Succeeded & responseData.Payload != null)
                    {
                        _referenceList = responseData.Payload.referencelist.Select(x => new TayseerReferenceList()
                        {
                            DewaReferenceNumber = x.dewareferencenumber,
                            TotalAccounts = x.totalaccounts,
                            TotalAmount = x.totalamount,
                            CreatedDate = TayseerHelper.FormatNewDate(x.createddate) + " " + TayseerHelper.FormatTime(x.createdtime),
                            ReferenceNumberPaidAccount = x.paidaccountsreferencenumber,
                            ReferenceNumberPaidAmount = Convert.ToDecimal(x.paidamountreferencenumber),
                            PaymentStatus = (x.status.Trim() == "5") ? Translate.Text("Paid") : Translate.Text("Not Paid"),
                            Status = x.status,
                            CheckNumber = x.checknumber,
                            NoCheckPay = x.nocheckpay,
                            OkPay = x.okpay
                        }).ToList();
                    }
                }
                else
                {
                    ViewBag.Message = Translate.Text("NoDetailsReferenceNumber");
                    return View("~/Views/Feature/Bills/Tayseer/TayseerReference.cshtml", _referenceListDetail);
                }
                //foreach (DataRow row in dtRefs.Rows)
                //{
                //    var referenceList = new TayseerReferenceList();
                //    referenceList.DewaReferenceNumber = row["DewaReferenceNumber"].ToString();
                //    referenceList.TotalAccounts = row["TotalAccounts"].ToString();
                //    referenceList.TotalAmount = row["TotalAmount"].ToString();
                //    referenceList.CreatedDate = TayseerHelper.FormatDate(row["CreatedDate"].ToString()) + " " + row["CreatedTime"].ToString();

                //    status = (row["Status"] == null) ? string.Empty : row["Status"].ToString();

                //    referenceList.PaymentStatus = (status.Trim() == "5") ? "Paid" : "Not Paid";
                //    _referenceList.Add(referenceList);
                //}
                _referenceListDetail.TayseerReferenceListDetails = _referenceList;
                return View("~/Views/Feature/Bills/Tayseer/TayseerReference.cshtml", _referenceListDetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("~/Views/Feature/Bills/Tayseer/TayseerReference.cshtml");
            }
        }

        #endregion TayseerGetReferenceNoList

        #region TayseerReferenceNoHistory

        [HttpGet]
        public ActionResult TayseerReferenceNoHistory()
        {
            if (!IsLoggedIn)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

            CacheProvider.Remove(CacheKeys.Tayseer_EasyPay_Transaction_Type);

            List<TayseerReferenceList> _referenceList = new List<TayseerReferenceList>();
            TayseerReferenceListDetail _referenceListDetail = new TayseerReferenceListDetail();
            string status = string.Empty;
            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            try
            {
                ViewBag.SelectReferenceDetailsURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_RERERENCE_DETAILS_HISTORY);
                ViewBag.SelecEasyPayURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.EASYPAY);
                var responseData = TayseerClient.TayseerList(new TayseerDetailsRequest(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);
                if (responseData.Succeeded & responseData.Payload != null)
                {
                    _referenceList = responseData.Payload.referencelist.Select(x => new TayseerReferenceList()
                    {
                        DewaReferenceNumber = x.dewareferencenumber,
                        TotalAccounts = x.totalaccounts,
                        TotalAmount = x.totalamount,
                        CreatedDate = TayseerHelper.FormatNewDate(x.createddate) + " " + TayseerHelper.FormatTime(x.createdtime),
                        ReferenceNumberPaidAccount = x.paidaccountsreferencenumber,
                        ReferenceNumberPaidAmount = Convert.ToDecimal(x.paidamountreferencenumber),
                        PaymentStatus = (x.status.Trim() == "5") ? Translate.Text("Paid") : Translate.Text("Not Paid"),
                        Status = x.status,
                        CheckNumber = x.checknumber,
                        NoCheckPay = x.nocheckpay,
                        OkPay = x.okpay
                    }).ToList();

                    _referenceListDetail.TayseerReferenceListDetails = _referenceList;
                    return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceNoHistory.cshtml", _referenceListDetail);
                }
                else
                {
                    ViewBag.Message = Translate.Text("NoDetailsReferenceNumber");
                    return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceNoHistory.cshtml", _referenceListDetail);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceNoHistory.cshtml", _referenceListDetail);
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TayseerReferenceNoHistory(TayseerReferenceListDetail _referenceListDetail)
        {
            if (!IsLoggedIn)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            try
            {
                var _fc = FetchFutureCenterValues();
                var response = DewaApiClient.GetEasyPayEnquiry(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, _referenceListDetail.PaymentDewaReferenceNumber, RequestLanguage, Request.Segment(), _fc.Branch);
                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse _response;
                    _response = response.Payload;

                    #region [MIM Payment Implementation]
                    var payRequest = new CipherPaymentModel();

                    payRequest.PaymentData.contractaccounts = _response.@return.easypaynumber?.Trim();
                    payRequest.PaymentData.amounts = Convert.ToString(_referenceListDetail.TotalAmountPay);
                    payRequest.PaymentData.movetoflag = "R";
                    payRequest.IsThirdPartytransaction = false;
                    payRequest.ServiceType = ServiceType.PayBill; //"PayBill",
                    payRequest.PaymentData.easypaynumber = _response.@return.easypaynumber?.Trim();
                    payRequest.PaymentData.easypayflag = "X";
                    payRequest.PaymentMethod = _referenceListDetail.paymentMethod;
                    payRequest.BankKey = _referenceListDetail.bankkey;
                    payRequest.SuqiaValue = _referenceListDetail.SuqiaDonation;
                    payRequest.SuqiaAmt = _referenceListDetail.SuqiaDonationAmt;
                    #endregion

                    var payResponse = ExecutePaymentGateway(payRequest);

                    if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                    {
                        CacheProvider.Store(CacheKeys.TAYSEER_HISTORY_PAYMENT_PATH, new CacheItem<string>("tayseerhistorypayment"));
                        CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse>(_response));
                        CacheProvider.Store(CacheKeys.Tayseer_EasyPay_Transaction_Type, new CacheItem<bool>(true));
                        return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
                List<TayseerReferenceList> _referenceList = new List<TayseerReferenceList>();

                string status = string.Empty;
                ViewBag.SelectReferenceDetailsURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.TAYSEER_RERERENCE_DETAILS_HISTORY);
                ViewBag.SelecEasyPayURL = GetSitecoreUrlByID(SitecoreItemIdentifiers.EASYPAY);
                var responseDataVal = TayseerClient.TayseerList(new TayseerDetailsRequest(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);
                if (responseDataVal.Succeeded & responseDataVal.Payload != null)
                {
                    _referenceList = responseDataVal.Payload.referencelist.Select(x => new TayseerReferenceList()
                    {
                        DewaReferenceNumber = x.dewareferencenumber,
                        TotalAccounts = x.totalaccounts,
                        TotalAmount = x.totalamount,
                        CreatedDate = TayseerHelper.FormatNewDate(x.createddate) + " " + TayseerHelper.FormatTime(x.createdtime),
                        ReferenceNumberPaidAccount = x.paidaccountsreferencenumber,
                        ReferenceNumberPaidAmount = Convert.ToDecimal(x.paidamountreferencenumber),
                        PaymentStatus = (x.status.Trim() == "5") ? Translate.Text("Paid") : Translate.Text("Not Paid"),
                        Status = x.status,
                        CheckNumber = x.checknumber,
                        NoCheckPay = x.nocheckpay,
                        OkPay = x.okpay
                    }).ToList();
                    _referenceListDetail.TayseerReferenceListDetails = _referenceList;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceNoHistory.cshtml", _referenceListDetail);
        }
        #endregion TayseerReferenceNoHistory

        #region TayseerReferenceDetailsHistory

        public ActionResult TayseerReferenceDetailsHistory()
        {
            if (!IsLoggedIn)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

            //DataTable dtSource = null;
            DataTable dtOutstanding = null;
            // DataTable dtAccounts = null;
            DataTable dtNewAccounts = null;

            DataTable dtMerged = null;
            List<string> accountList = new List<string>();
            List<TayseerReferenceList> _referenceList = new List<TayseerReferenceList>();
            TayseerReferenceListDetail _referenceListDetail = new TayseerReferenceListDetail();

            try
            {
                if (Request.QueryString["refno"] != null && !string.IsNullOrEmpty(Request.QueryString["refno"].ToString()))
                {
                    var responseData = TayseerClient.TayseerDetails(new TayseerDetailsRequest
                    {
                        referencenumberinputs = new Referencenumberinputs
                        {
                            dewareferencenumber = Request.QueryString["refno"].ToString(),
                        }
                    }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, Request.Segment(), RequestLanguage);

                    if (responseData != null && responseData.Succeeded)
                    {
                        dtNewAccounts = new DataTable();
                        dtNewAccounts.Columns.Add("AccountNumber", typeof(string));
                        dtNewAccounts.Columns.Add("ContractAccount", typeof(string));
                        dtNewAccounts.Columns.Add("Selected", typeof(bool));
                        dtNewAccounts.Columns.Add("AmountToPay", typeof(decimal));
                        dtNewAccounts.Columns.Add("Amount", typeof(decimal));
                        dtNewAccounts.Columns.Add("AmountPaid", typeof(decimal));
                        dtNewAccounts.Columns.Add("collectiveaccount", typeof(string));

                        foreach (var itemAcct in responseData.Payload.accountlist)
                        {
                            DataRow dr = dtNewAccounts.NewRow();
                            dr["Selected"] = true;
                            dr["AmountToPay"] = itemAcct.amount;
                            dr["Amount"] = itemAcct.amount;
                            dr["AccountNumber"] = itemAcct.contractaccount;
                            dr["ContractAccount"] = itemAcct.contractaccount;
                            dr["AmountPaid"] = itemAcct.amountpaid;
                            dr["collectiveaccount"] = itemAcct.collectiveaccount;
                            dtNewAccounts.Rows.Add(dr);
                        }
                    }

                    if (dtNewAccounts != null && dtNewAccounts.Rows.Count > 1)
                    {
                        //dtSource.Columns.Add("AccountNumber", typeof(string));
                        //dtSource.Columns.Add("AmountToPay", typeof(decimal));

                        //dtAccounts = dtSource.Clone();

                        //foreach (DataRow row in dtSource.Rows)
                        //{
                        //    row["AmountToPay"] = row["Amount"];
                        //    row["AccountNumber"] = row["ContractAccount"];
                        //    dtAccounts.ImportRow(row);
                        //}

                        accountList = GetAccountList(dtNewAccounts);
                        dtOutstanding = GetOutstandingAmount(accountList);
                        dtMerged = MergeTables(dtNewAccounts, dtOutstanding);
                        AccountsTable = dtMerged;
                        ViewBag.SetTotal = SetTotals(dtMerged);
                        ViewBag.ReferenceNo = Request.QueryString["refno"].ToString();
                    }
                    else
                    {
                        ViewBag.Message = Translate.Text("NoDetailsReferenceNumber");
                        return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceDetailsHistory.cshtml", _referenceListDetail);
                    }
                    foreach (DataRow row in dtNewAccounts.Rows)
                    {
                        var referenceList = new TayseerReferenceList();
                        referenceList.ContractAccountNumber = row["ContractAccount"].ToString();
                        referenceList.TotalAmount = row["Amount"].ToString();
                        referenceList.ReferenceNumberPaidAmount = Convert.ToDecimal(row["AmountPaid"]);
                        _referenceList.Add(referenceList);
                    }
                    _referenceListDetail.TayseerReferenceListDetails = _referenceList;
                }
                return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceDetailsHistory.cshtml", _referenceListDetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("~/Views/Feature/Bills/Tayseer/TayseerReferenceDetailsHistory.cshtml", _referenceListDetail);
            }
        }

        #endregion TayseerReferenceDetailsHistory

        #region _AddselectedAccountTable

        public DataTable _AddselectedAccountTable(DataTable AccountTable, string selectedAccount)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedAccount) && AccountTable != null && AccountTable.Rows.Count > 0)
                {
                    var selectedAccountList = selectedAccount.Split(',').ToList();
                    foreach (DataRow row in AccountsTable.Rows)
                    {
                        if (selectedAccountList.Any(x => selectedAccountList.Contains(row["AccountNumber"])))
                        {
                            row["Selected"] = true;
                        }
                        else
                        {
                            row["Selected"] = false;
                        }
                    }
                    AccountTable.AcceptChanges();
                }
            }
            catch(Exception ex) { }
            return AccountTable;
        }

        #endregion _AddselectedAccountTable
    }
}