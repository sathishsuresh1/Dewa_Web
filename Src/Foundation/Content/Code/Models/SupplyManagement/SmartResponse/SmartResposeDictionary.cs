using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.SmartResponseModel
{
    [SitecoreType(TemplateId = "{E3D15B23-4A00-4158-A869-1BF1AB55282C}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class SmartResposeDictionary : ContentBase
    {
        public virtual string KeyEnglish { get; set; }

        public virtual string Arabic { get; set; }
        public virtual string Chinese { get; set; }
        public virtual string Philippines { get; set; }

        public virtual string Urdu { get; set; }
    }

    public class SmartResposeDictionarys : ContentBase
    {
        //public SmartResposeDictionarys()
        //{
        //    Children = new List<SmartResposeDictionary>();
        //}

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<SmartResposeDictionary> Children { get; set; }
    }
    public enum SmlangCode
    {
        /// <summary>
        /// English
        /// </summary>
        en,

        /// <summary>
        /// arabic
        /// </summary>
        ar,

        /// <summary>
        /// chinese
        /// </summary>
        zh,

        /// <summary>
        /// urdu
        /// </summary>
        ur,

        /// <summary>
        /// filipino(Tagalog)
        /// </summary>
        tl
    }
}