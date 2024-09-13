using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Integration.Responses;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Glass.Mapper.Sc.Web;
using DEWAXP.Foundation.Logger;
using ExcelDataReader;
using static Glass.Mapper.Constants;

namespace DEWAXP.Feature.Bills.Helpers
{
    public class TayseerHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        #region ImportENBDExcel

        public static DataTable ImportENBDExcel(string mimeType, Stream fileStream)
        {
            DataTable dtAccountsNew = null;
            try
            {
                IExcelDataReader excelReader = null;
                DataSet result = null;
                DataTable dtAccounts = null;

                if (mimeType == "application/vnd.ms-excel")
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                else if (mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                if (excelReader != null)
                {
                    result = excelReader.AsDataSet();
                    excelReader.Close();

                    if (result != null)
                    {
                        if (result.Tables.Count > 0)
                        {
                            dtAccounts = result.Tables[0];
                            dtAccountsNew = dtAccounts.Clone();
                            if (dtAccountsNew.Columns.Count > 1)
                            {
                                dtAccountsNew.Columns[1].DataType = typeof(decimal);
                            }
                            foreach (DataRow drow in dtAccounts.Rows)
                            {
                                if (!String.IsNullOrWhiteSpace(Convert.ToString(drow[0])) || !String.IsNullOrWhiteSpace(Convert.ToString(drow[1])))
                                {
                                    if (drow[1].ToString().Any(char.IsLetter))
                                    {
                                        drow[1] = 0;
                                    }
                                    drow.AcceptChanges();
                                    dtAccountsNew.ImportRow(drow);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return dtAccountsNew;
            }
            return dtAccountsNew;
        }

        #endregion ImportENBDExcel

        #region GetENBDErrorMessage

        public static string GetENBDErrorMessage(string errorCode)
        {
            string message = string.Empty;
            try
            {
                switch (errorCode)
                {
                    case "ONE_COLUMN":
                        message = Translate.Text("ValidUploadCorrect");
                        break;

                    case "MAX_ACCOUNT":
                        message = Translate.Text("ValidUploadSheetMax");
                        break;

                    case "CA_AMT_EMPTY":
                        message = Translate.Text("ValidCAAMTEMPTY");
                        break;

                    case "CA_EMPTY":
                        message = Translate.Text("ValidCAEMPTY");
                        break;

                    case "AMT_EMPTY":
                        message = Translate.Text("ValidAMTEMPTY");
                        break;

                    case "CA_AMT_INVALID":
                        message = Translate.Text("ValidCAAMTINVALID");
                        break;

                    case "CA_INVALID":
                        message = Translate.Text("ValidCAINVALID");
                        break;

                    case "AMT_INVALID":
                        message = Translate.Text("ValidAMTINVALID");
                        break;

                    case "CA_AMT_INVALID_LENGTH":
                        message = Translate.Text("ValidCAINVALID");
                        break;

                    case "DUPLICATE_DATA":
                        message = Translate.Text("ValidDUPLICATEDATA");
                        break;

                    case "AMT_LIMIT":
                        message = Translate.Text("ValidAMTLIMIT");
                        break;
                }
                return message;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return message;
            }
        }

        #endregion GetENBDErrorMessage

        #region IsDataTableValid

        public static bool IsDataTableValid(DataTable dt)
        {
            bool retVal = false;
            try
            {
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    { retVal = true; }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                return retVal;
            }
        }

        #endregion IsDataTableValid

        #region ColumnEqual

        public static bool ColumnEqual(object A, object B)
        {
            // Compares two values to see if they are equal. Also compares DBNULL.Value.
            // Note: If your DataTable contains object fields, then you must extend this
            // function to handle them in a meaningful way if you intend to group on them.
            try
            {
                if (A == DBNull.Value && B == DBNull.Value) //  both are DBNull.Value
                    return true;
                if (A == DBNull.Value || B == DBNull.Value) //  only one is DBNull.Value
                    return false;
                return (A.Equals(B));  // value type standard comparison
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                return (A.Equals(B));
            }
        }

        #endregion ColumnEqual

        #region GetDistinctCount

        public static int GetDistinctCount(string TableName, DataTable SourceTable, string FieldName)
        {
            DataTable dt = null;
            try
            {
                if (IsDataTableValid(SourceTable))
                {
                    dt = new DataTable(TableName);
                    dt.Columns.Add(FieldName, SourceTable.Columns[FieldName].DataType);

                    object LastValue = null;
                    foreach (DataRow dr in SourceTable.Select("", FieldName))
                    {
                        if (LastValue == null || !(ColumnEqual(LastValue, dr[FieldName])))
                        {
                            LastValue = dr[FieldName];
                            dt.Rows.Add(new object[] { LastValue });
                        }
                    }
                }
                return (Convert.ToInt32(dt?.Rows?.Count ?? 0));
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                return (Convert.ToInt32(dt?.Rows?.Count ?? 0));
            }
        }

        #endregion GetDistinctCount

        #region CreateErrorTable

        private static DataTable CreateErrorTable()
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable("Errors");
                DataColumn col = new DataColumn();

                dt.Columns.Add("LineNo", typeof(int));
                dt.Columns.Add("Error", typeof(string));
                dt.Columns.Add("Data", typeof(string));

                dt.Columns["LineNo"].AllowDBNull = true;
                return dt;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return dt;
            }
        }

        #endregion CreateErrorTable

        #region AddErrorRow

        private static DataTable AddErrorRow(DataTable errorTable, int lineNo, string error, string data)
        {
            try
            {
                DataRow row = null;

                row = errorTable.NewRow();
                row["LineNo"] = lineNo;
                row["Error"] = error;
                row["Data"] = data;
                errorTable.Rows.Add(row);

                return errorTable;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return errorTable;
            }
        }

        #endregion AddErrorRow

        #region ValidateENBDExcel

        public static DataTable ValidateENBDExcel(DataTable dtSource)
        {
            string message = string.Empty;
            DataTable errorTable = null;
            int lineNo = 0;
            long accountNo = 0;
            decimal amount = decimal.Zero;
            decimal maxAmountLimit = 999999999.99M;
            int distinctCount = 0;
            string alertMessage = string.Empty;

            try
            {
                var accountlimit = _contentRepository.GetItem<DataSourceItems>(new GetItemByIdOptions(Guid.Parse(DataSources.TAYSEER_ACCOUNT_LIMIT)));
                int accountExcellimit = accountlimit.Value != null ? Convert.ToInt32(accountlimit.Value) : 300;

                if (dtSource != null)
                {
                    if (dtSource.Rows.Count > 0)
                    {
                        dtSource.TableName = "ExcelRows";
                        errorTable = CreateErrorTable();

                        if (dtSource.Columns.Count < 2)
                        {
                            message = TayseerHelper.GetENBDErrorMessage("ONE_COLUMN");
                            AddErrorRow(errorTable, 0, message, string.Empty);
                        }

                        if (errorTable.Rows.Count == 0)
                        {
                            dtSource.Columns[0].ColumnName = "ContractAccount";
                            dtSource.Columns[1].ColumnName = "AmountToPay";

                            distinctCount = TayseerHelper.GetDistinctCount("ExcelRows", dtSource, "ContractAccount");
                            if (dtSource.Rows.Count != distinctCount)
                            {
                                message = TayseerHelper.GetENBDErrorMessage("DUPLICATE_DATA");
                                AddErrorRow(errorTable, 1, message, string.Empty);
                                //gvFileUpload.DataSource = null;
                                //gvFileUpload.DataBind();
                            }

                            if (dtSource.Rows.Count > accountExcellimit)
                            {
                                message = TayseerHelper.GetENBDErrorMessage("MAX_ACCOUNT");
                                AddErrorRow(errorTable, 1, message, string.Empty);
                            }

                            foreach (DataRow drow in dtSource.Rows)
                            {
                                lineNo = dtSource.Rows.IndexOf(drow) + 1;

                                if (String.IsNullOrWhiteSpace(Convert.ToString(drow[0])) && String.IsNullOrWhiteSpace(Convert.ToString(drow[1])))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("CA_AMT_EMPTY");
                                    AddErrorRow(errorTable, dtSource.Rows.IndexOf(drow) + 1, message, string.Empty);
                                }
                                else if (String.IsNullOrWhiteSpace(Convert.ToString(drow[0])))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("CA_EMPTY");
                                    AddErrorRow(errorTable, lineNo, message, string.Empty);
                                }
                                else if (String.IsNullOrWhiteSpace(Convert.ToString(drow[1])))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("AMT_EMPTY");
                                    AddErrorRow(errorTable, lineNo, message, string.Empty);
                                }
                                else if (!long.TryParse(drow[0].ToString(), out accountNo) && !decimal.TryParse(drow[1].ToString(), out amount))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("CA_AMT_INVALID");
                                    AddErrorRow(errorTable, lineNo, message, drow[0] + ", " + drow[1]);
                                }
                                else if (!long.TryParse(drow[0].ToString(), out accountNo))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("CA_INVALID");
                                    AddErrorRow(errorTable, lineNo, message, drow[0].ToString());
                                }
                                else if (!decimal.TryParse(drow[1].ToString(), out amount))
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("AMT_INVALID");
                                    AddErrorRow(errorTable, lineNo, message, drow[1].ToString());
                                }
                                else if (Convert.ToDecimal(drow[1].ToString()) > maxAmountLimit)
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("AMT_LIMIT");
                                    AddErrorRow(errorTable, lineNo, message, drow[1].ToString());
                                }
                                else if (accountNo.ToString().Length < 10 || accountNo.ToString().Length > 10)
                                {
                                    message = TayseerHelper.GetENBDErrorMessage("CA_AMT_INVALID_LENGTH");
                                    AddErrorRow(errorTable, lineNo, message, drow[0].ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        //alertMessage = GetText("MsgCorrectAccounts");
                        //ShowMessage("validationfalse", "Please upload the file with correct Contract Accounts and Amount. Kindly refer to the sample file.", true);
                    }
                }
                else
                {
                    //alertMessage = GetText("MsgCorrectAccounts");
                    //ShowMessage("validationfalse1", "Please upload the file with correct Contract Accounts and Amount. Kindly refer to the sample file.", true);
                }
                return errorTable;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return errorTable;
            }
        }

        #endregion ValidateENBDExcel

        #region ValidateBeforeAdd

        public static bool ValidateBeforeAdd(string contractAccountNo, string amount, out string errorMsg)
        {
            string validationMsg = string.Empty;
            bool retVal = true;
            decimal amountToPay = decimal.Zero;
            decimal maxAmountLimit = 999999999.99M;
            errorMsg = string.Empty;

            try
            {
                Validateaccountnumber(contractAccountNo, out errorMsg, out retVal);
                if(!retVal)
                {
                    return retVal;
                }

                if (String.IsNullOrWhiteSpace(amount))
                {
                    //errorMsg = string.Format("Please enter Amount");
                    errorMsg = Translate.Text("PleaseEnterAmount");
                    retVal = false;
                    return retVal;
                }

                if (!decimal.TryParse(amount, out amountToPay))
                {
                    //errorMsg = string.Format("Please enter valid amount");
                    errorMsg = Translate.Text("ValidAmount");
                    retVal = false;
                    return retVal;
                }

                //if (amountToPay <= 0.00M)
                if (amountToPay < 1)
                {
                    //errorMsg = string.Format("Please enter a valid amount. Zero amount is not valid.");
                    errorMsg = Translate.Text("PleaseEnterValidZero");
                    retVal = false;
                    return retVal;
                }
                if (amountToPay > maxAmountLimit)
                {
                    // errorMsg = string.Format("Amount limit for Reference Number payments cannot exceed 999,999,999.99");
                    errorMsg = Translate.Text("ValidAMTLIMIT");
                    retVal = false;
                    return retVal;
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return retVal;
            }
        }

        public static bool Validateaccountnumber(string contractAccountNo,out string errorMsg,out bool retVal)
        {
            errorMsg = string.Empty;
            retVal = true;
            try { 
            if (String.IsNullOrWhiteSpace(contractAccountNo))
            {
                //errorMsg = string.Format("Please enter Contract Account Number");
                errorMsg = Translate.Text("PleaseEnterAccountNumber");
                retVal = false;
                return retVal;
            }

            if (contractAccountNo.Length < 10 && contractAccountNo.Length > 12)
            {
                //errorMsg = string.Format("Please enter a valid Contract Account Number");
                errorMsg = Translate.Text("PleaseEnterValidAccountNumber");
                retVal = false;
                return retVal;
            }
            long i = 0;
            if (!long.TryParse(contractAccountNo, out i))
            {
                errorMsg = Translate.Text("PleaseEnterValidAccountNumber");
                retVal = false;
                return retVal;
            }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return retVal;
        }

        #endregion ValidateBeforeAdd

        #region ToDataSet

        internal static DataSet ToDataSet(string xml)
        {
            DataSet dataSet = null;
            StringReader strReader = null;
            try
            {
                dataSet = new DataSet();
                strReader = new StringReader(xml);
                dataSet.ReadXml(strReader, XmlReadMode.Auto);
                return dataSet;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return dataSet;
            }
        }

        #endregion ToDataSet

        #region IsDataSetValid

        public static bool IsDataSetValid(DataSet ds, string tableName)
        {
            bool retVal = false;
            try
            {
                if (ds != null)
                {
                    if (null != ds.Tables[tableName])
                    {
                        if (ds.Tables[tableName].Rows.Count > 0)
                        { retVal = true; }
                    }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return retVal;
            }
        }

        #endregion IsDataSetValid

        #region ConvertArrayToDatatable

        public static DataTable ConvertArrayToDatatable(AccountDetails[] arrList)
        {
            DataTable dt = new DataTable();
            try
            {
                if (arrList.Count() > 0)
                {
                    Type arrype = arrList[0].GetType();
                    dt = new DataTable(arrype.Name);

                    foreach (PropertyInfo propInfo in arrype.GetProperties())
                    {
                        dt.Columns.Add(new DataColumn(propInfo.Name));
                    }

                    foreach (object obj in arrList)
                    {
                        DataRow dr = dt.NewRow();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            dr[dc.ColumnName] = obj.GetType().GetProperty(dc.ColumnName).GetValue(obj, null);
                        }
                        dt.Rows.Add(dr);
                    }
                }

                return dt;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return dt;
            }
        }

        #endregion ConvertArrayToDatatable

        #region FormatDate

        public static string FormatDate(string val)
        {
            string retVal = string.Empty;
            DateTime dt = DateTime.Now;

            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    if (DateTime.TryParse(val, out dt))
                    {
                        retVal = dt.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        retVal = val;
                    }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex,new object());
                return retVal;
            }
        }
        public static string FormatNewDate(string val)
        {
            string retVal = string.Empty;
            DateTime dt = DateTime.Now;

            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    if (DateTime.TryParse(val, out dt))
                    {
                        retVal = dt.ToString("MMM dd, yyyy");
                    }
                    else
                    {
                        retVal = val;
                    }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                return retVal;
            }
        }
        public static string FormatTime(string val)
        {
            string retVal = string.Empty;
            DateTime dt = DateTime.Now;

            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    if (DateTime.TryParse(val, out dt))
                    {
                        retVal = dt.ToString("hh:mm tt");
                    }
                    else
                    {
                        retVal = val;
                    }
                }
                return retVal;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                return retVal;
            }
        }
        #endregion FormatDate
    }
}