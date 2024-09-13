// <copyright file="CSVfileimport.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Utils
{
    using DEWAXP.Feature.GatePass.Models.ePass;
    using DEWAXP.Feature.GatePass.Models.WorkPermit;
    using DEWAXP.Foundation.Content;
    using ExcelDataReader;
    using FileHelpers;
    using FileHelpers.ExcelNPOIStorage;
    using Sitecore.IO;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.DynamicData;

    /// <summary>
    /// Defines the <see cref="CSVfileimport" />
    /// </summary>
    public static class CSVfileimport
    {
        /// <summary>
        /// The fileread
        /// </summary>
        public static CSVfileimportwitherror fileread(HttpPostedFileBase file)
        {
            CSVfileimportwitherror model = new CSVfileimportwitherror();
            var engine = new FileHelperEngine<CSVfileformat>();
            engine.Options.IgnoreEmptyLines = true;
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            engine.AfterReadRecord += Engine_AfterReadRecord;
            var records = engine.ReadStream(new StreamReader(file.InputStream), Int32.MaxValue);
            model.errorlist = engine.ErrorManager != null && engine.ErrorManager.Errors != null ? engine.ErrorManager.Errors.ToList() : null;
            model.passedrecords = records != null ? records.ToList() : null;
            return model;
        }

        public static CSVfileimportwitherror wpfileread(HttpPostedFileBase file)
        {
            CSVfileimportwitherror model = new CSVfileimportwitherror();
        
            var dataTable = new DataTable();

            if (file != null && file.ContentLength > 0)
            {
                //ExcelDataReader works on binary excel file
                Stream stream = file.InputStream;
                //We need to written the Interface.
                IExcelDataReader reader = null;
                if (file.FileName.EndsWith(".xls"))
                {
                    //reads the excel file with .xls extension
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (file.FileName.EndsWith(".xlsx"))
                {
                    //reads excel file with .xlsx extension
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                //// reader.IsFirstRowAsColumnNames
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(conf);

                // Now you can get data from each sheet by its index or its "name"
                dataTable = dataSet.Tables[0];


                List<DataRow> list = dataTable.AsEnumerable().ToList();

                List<WorkPermitCSVfileformat> WorkPermit = new List<WorkPermitCSVfileformat>();
                WorkPermit = (from DataRow row in dataTable.Rows

                              select new WorkPermitCSVfileformat
                              {
                                  CustomerName = row[0].ToString(),
                                  Profession = row[1].ToString(),
                                  Phonenumber = row[2].ToString().Substring(1),
                                  Emailid = row[3].ToString(),
                                  EmiratesID = row[4].ToString(),
                                  EidDate = Convert.ToDateTime(row[5].ToString()),
                                  Visanumber = row[6].ToString(),
                                  VisaexpDate = Convert.ToDateTime(row[7].ToString()),
                                  Passportnumber = row[8].ToString(),
                                  PassportexpDate = Convert.ToDateTime(row[9].ToString()),


                              }).ToList();


                //model.wppassedrecords = WorkPermit != null ? WorkPermit.ToList() : null;


                StringBuilder fileContent = new StringBuilder();

                foreach (var col in dataTable.Columns)
                {
                    fileContent.Append(col.ToString() + ",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);

                foreach (DataRow dr in dataTable.Rows)
                {
                    foreach (var column in dr.ItemArray)
                    {
                        fileContent.Append("\"" + column.ToString() + "\",");
                    }

                    fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
                }

                var bytes = Encoding.ASCII.GetBytes(fileContent.ToString());

                using (MemoryStream stream1 = new MemoryStream(bytes))
                {
                    var engine = new FileHelperEngine<WorkPermitCSVfileformat>();
                    engine.Options.IgnoreEmptyLines = true;
                    engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
                    engine.AfterReadRecord += Engine_AfterReadRecord;
                    var records = engine.ReadStream(new StreamReader(stream1), Int32.MaxValue);
                    model.errorlist = engine.ErrorManager != null && engine.ErrorManager.Errors != null ? engine.ErrorManager.Errors.ToList() : null;
                    model.wppassedrecords = records != null ? records.ToList() : null;
                }

            }
           


            return model;
        }

        private static void Engine_AfterReadRecord(EngineBase engine, FileHelpers.Events.AfterReadEventArgs<CSVfileformat> e)
        {
            e.Record.ID = e.LineNumber - 1;
        }

        private static void Engine_AfterReadRecord(EngineBase engine, FileHelpers.Events.AfterReadEventArgs<WorkPermitCSVfileformat> e)
        {
            e.Record.ID = e.LineNumber - 1;
        }
    }

    public class CSVfileimportwitherror
    {
        public List<ErrorInfo> errorlist { get; set; }
        public List<CSVfileformat> passedrecords { get; set; }
        public List<WorkPermitCSVfileformat> wppassedrecords { get; set; }
    }

  

}