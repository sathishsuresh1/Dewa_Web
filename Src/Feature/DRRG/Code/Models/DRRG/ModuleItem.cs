using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.DRRG.Models
{
    public class ModuleItem
    {
        public string manufacturerCode { get; set; }
        public string pvId { get; set; }
        public string inverterId { get; set; }
        public string interfaceId { get; set; }
        public string usageCategory { get; set; }
        public string serialnumber { get; set; }
        public string referenceNumber { get; set; }
        public string representative { get; set; }
        public string applicationtype { get; set; }
        public string type { get; set; }
        public string equipmentType { get; set; }
        public string modelName { get; set; }
        public string manufacturerName { get; set; }
        public string dateSubmitted { get; set; }
        public DateTime? datedtSubmitted { get; set; }
        public DateTime? updatedDate { get; set; }
        public string status { get; set; }
        public string statusText { get; set; }
        public string remarks { get; set; }
        public long id { get; set; }
        public string testMethod { get; set; }
        public long fileId { get; set; }
        public string celltechnology { get; set; }
        public string nominalpower { get; set; }
        public string ratedpower { get; set; }
        public string acparentpower { get; set; }
        public string application { get; set; }
        public string communicationprotocol { get; set; }
        public string EvaluatedBy { get; set; }
        public string extraCompliance { get; set;}
    }
    public class FilteredDRRGModules
    {
        public FilteredDRRGModules()
        {
            Lstmodule = new List<ModuleItem>();
            LastRecords = new ApplicationLogComposite();
            Lstevaluator = new List<EvaluatorItem>();
        }
        /// <summary>
        /// Gets or sets the lstpasses.
        /// </summary>
        public List<ModuleItem> Lstmodule { get; set; }
        public ApplicationLogComposite LastRecords { get; set; }
        public List<EvaluatorItem> Lstevaluator { get; set; }

        /// <summary>
        /// Gets or sets the totalpage.
        /// </summary>
        public int totalpage { get; set; }
        public int firstitem { get; set; }
        public int lastitem { get; set; }
        public int totalitem { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pagination.
        /// </summary>
        public bool pagination { get; set; }

        ///// <summary>
        ///// Gets or sets the pagenumbers.
        ///// </summary>
        //public IEnumerable<int> pagenumbers { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public string keywords { get; set; }

        /// <summary>
        /// Gets or sets the namesort.
        /// </summary>
        public string namesort { get; set; }

        /// <summary>
        /// Gets or sets the strdataindex.
        /// </summary>
        public string strdataindex { get; set; }
    }

    public class ApplicationLog : ModuleItem
    {
        public string ProcessingHistory { get; set; }
        public string Evaluator { get; set; }
        public string Representativename { get; set; }
        public string ManufacturerCode { get; set; }
    }

    public class DRRGModuleList
    {
        public DRRGModuleList()
        {
            ModuleItem = new List<ModuleItem>();
        }
        public List<ModuleItem> ModuleItem { get; set; }
        public bool isAdmin { get; set; } = false;
    }
    #region CommonMethods
    public class KeyValueList : List<KeyValuePair<string, string>>
    {
        public void Add(string key, string value)
        {
            var element = new KeyValuePair<string, string>(key, value);
            this.Add(element);
        }
    }
    #endregion

    public class ApplicationLogComposite
    {
        public ApplicationLogComposite()
        {
            ManufacturerList = new KeyValueList();
            ApplicationLog = new List<ApplicationLog>();
        }
        public List<ApplicationLog> ApplicationLog { get; set; }

        public KeyValueList ManufacturerList { get; set; }

        public List<string> EvaluatorList { get; set; }

        public List<string> EvaluatedDate { get; set; }
    }
}