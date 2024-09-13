using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    public class ModuleDetail
    {
        public ModuleDetail()
        {
            moduleSectionRows = new List<ModuleSectionRow>();
        }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public List<ModuleSectionRow> moduleSectionRows { get; set; }
        public string UserRole { get; set; }
        public bool itemLock { get; set; } = false;
    }
    public class ModuleSectionRow
    {
        public ModuleSectionRow()
        {
            moduleSections = new List<ModuleSection>();
        }
        public string Title { get; set; }
        public List<ModuleSection> moduleSections { get; set; }
    }
    public class ModuleSection
    {
        public ModuleSection()
        {
            moduleItems = new List<ModuleKeyValueItem>();
        }
        public List<ModuleKeyValueItem> moduleItems { get; set; }
    }
    public class ModuleKeyValueItem
    {
        public ModuleKeyValueItem(string key, string value, bool linkvalue = false, bool filelink = false)
        {
            Key = key;
            Value = value;
            Linkvalue = linkvalue;
            FileLink = filelink;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Linkvalue { get; set; }
        public bool FileLink { get; set; }
    }

}