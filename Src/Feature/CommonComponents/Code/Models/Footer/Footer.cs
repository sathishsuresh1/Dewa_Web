using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Footer
{
    public class Footer : ContentBase
    {
        //public Footer()
        //{
        //    Images = new List<LinkedImage>();
        //    PrimaryLinks = new List<ContentBase>();
        //    Column1 = new List<ContentBase>();
        //    Column2 = new List<ContentBase>();
        //    Column3 = new List<ContentBase>();
        //    Column4 = new List<ContentBase>();
        //}

        [SitecoreField("FooterHeading")]
        public virtual string FooterHeading { get; set; }

        [SitecoreField("FooterParagraph")]
        public virtual string FooterParagraph { get; set; }

        [SitecoreField("GooglePlayStore")]
        public virtual Image GooglePlayStore { get; set; }

        [SitecoreField("GooglePlayStoreLink")]
        public virtual Link GooglePlayStoreLink { get; set; }

        [SitecoreField("AppleStore")]
        public virtual Image AppleStore { get; set; }

        [SitecoreField("AppleStoreLink")]
        public virtual Link AppleStoreLink { get; set; }

        [SitecoreField("Images")]
        public virtual IEnumerable<LinkedImage> Images { get; set; }

        [SitecoreField("Column1")]
        public virtual IEnumerable<ContentBase> Column1 { get; set; }

        [SitecoreField("Column2")]
        public virtual IEnumerable<ContentBase> Column2 { get; set; }

        [SitecoreField("Column3")]
        public virtual IEnumerable<ContentBase> Column3 { get; set; }

        [SitecoreField("Column4")]
        public virtual IEnumerable<ContentBase> Column4 { get; set; }

        [SitecoreField("Primary Links")]
        public virtual IEnumerable<ContentBase> PrimaryLinks { get; set; }

        [SitecoreField("Subnote")]
        public virtual string Subnote { get; set; }

        [SitecoreField("Note")]
        public virtual string Note { get; set; }

       public virtual DateTime Updated { get; set; }

        [SitecoreField("Show Rammas")]
        public virtual bool ShowRammas { get; set; }

        [SitecoreField("Rammas URL")]
        public virtual string RammasURL { get; set; }

        [SitecoreField("Rammas Secret ID")]
        public virtual string RammasSecretID { get; set; }

        [SitecoreField("Rammas User ID")]
        public virtual string RammasUserID { get; set; }

        [SitecoreField("Rammas Bot")]
        public virtual string RammasBot { get; set; }

        [SitecoreField("IsSubscribe")]
        public virtual bool IsSubscribe { get; set; }

        public virtual bool IsPopupModal { get; set; } = false;
        public virtual string PopupModal { get; set; }
    }
}