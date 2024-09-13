using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class LinksTabs : ContentBase
    {
       //public LinksTabs()
       // {
       //     Children = new List<LinksTab>();
       // }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<LinksTab> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{726AA771-261B-4A87-A614-EA2A13E6CE25}", AutoMap = true)]
    public class FormattedTabs : ContentBase
    {
        //public FormattedTabs()
        //{
        //    Children = new List<Tab>();
        //    ExpanderChildren = new List<ExpanderTabs>();
        //}
        [SitecoreChildren]
        public virtual IEnumerable<Tab> Children { get; set; }

        [SitecoreChildren]
        public virtual IEnumerable<ExpanderTabs> ExpanderChildren { get; set; }

        [SitecoreField("Tab Title")]
        public virtual string TabTitle { get; set; }

        [SitecoreField("Button1")]
        public virtual bool Button { get; set; }

        [SitecoreField("Button Text1")]
        public virtual string ButtonText { get; set; }

        [SitecoreField("Button Link1")]
       public virtual Link ButtonLink { get; set; }

        [SitecoreField("Button2")]
        public virtual bool Button2 { get; set; }

        [SitecoreField("Button Text2")]
        public virtual string ButtonText2 { get; set; }

        [SitecoreField("Button Link2")]
       public virtual Link ButtonLink2 { get; set; }

        [SitecoreField("Button3")]
        public virtual bool Button3 { get; set; }

        [SitecoreField("Button Text3")]
        public virtual string ButtonText3 { get; set; }

        [SitecoreField("Button Link3")]
       public virtual Link ButtonLink3 { get; set; }
    }

    [SitecoreType(TemplateId = "{BA4041E6-42A4-4D52-AAEC-289A6A5DA661}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class ExpanderTabs : ContentBase
    {
        //public ExpanderTabs()
        //{
        //    Children = new List<SubsectionTab>();
        //}

        public virtual string SelectableExpanderId { get { return ExpandTabTitle.Replace(" ", "-").ToLower(); } }

        [SitecoreChildren]
        public virtual IEnumerable<SubsectionTab> Children { get; set; }

        [SitecoreField("Expand Tab Title")]
        public virtual string ExpandTabTitle { get; set; }

        [SitecoreField("Expand Tab content")]
        public virtual string ExpandTabcontent { get; set; }
    }

    public class Tabs : ContentBase
    {
        //public Tabs()
        //{
        //    Children = new List<Tab>();
        //}

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<Tab> Children { get; set; }

        [SitecoreField("Displays in a modal dialog")]
        public virtual bool InModal { get; set; }
    }

    [SitecoreType(TemplateId = "{0F61D581-46C7-4313-B0BF-B2AEBB8CC1FE}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class Tab : ContentBase
    {
        public virtual string Title { get; set; }

        public virtual string Body { get; set; }

        [SitecoreField("Icon Class")]
        public virtual string IconClass { get; set; }

        public virtual Item Target { get; set; }

        public virtual string SelectableId { get { return Title.Replace(" ", "-").ToLower(); } }

        [SitecoreField("Tab Icon")]
        public TabIcon tabicon { get; set; }

        [SitecoreField("Module")]
        public virtual bool Module { get; set; }

        [SitecoreField("Module Placeholder")]
        public virtual string ModulePlaceholder { get; set; }
       
        [SitecoreField("ActiveTabSource")]
        public virtual IEnumerable<ContentBase> ActivePageList { get; set; }

        [SitecoreField("Interlink Tab")]
        public virtual IEnumerable<Tab> InterlinkTab { get; set; }
    }

    [SitecoreType(TemplateId = "{DC540953-D317-4450-A459-8C3226612275}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class SubsectionTab : ContentBase
    {

        [SitecoreField("Subsection Title")]
        public virtual string SubsectionTitle { get; set; }

        [SitecoreField("Subsection content")]
        public virtual string Subsectioncontent { get; set; }
    }

    public class MultipleTargetTab : ContentBase
    {
        public virtual string Title { get; set; }

        public virtual Item Target { get; set; }

        [SitecoreField("Second Target")]
        public virtual Item SecondTarget { get; set; }

        [SitecoreField("ActivePageList")]
        public virtual IEnumerable<ContentBase> ActivePageList { get; set; }

        [SitecoreField("Class Name")]
        public virtual string ClassName { get; set; }

        [SitecoreField("Hide by default")]
        public virtual bool Hidebydefault { get; set; }
    }

    public class MultipleTargetTabs : ContentBase
    {
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<MultipleTargetTab> Children { get; set; }

        [SitecoreField("Displays in a modal dialog")]
        public virtual bool InModal { get; set; }
    }

    public class LinksTab : ContentBase
    {
        public virtual string Title { get; set; }

        [SitecoreField("Links")]
        public virtual IEnumerable<PageBase> Links { get; set; }

        [SitecoreField("Icon Class")]
        public virtual string IconClass { get; set; }
        [SitecoreField("Tab Icon")]
        public TabIcon tabicon { get; set; }
    }

    [SitecoreType(TemplateId = "{db59fcfc-787f-490a-b773-e5b8aa1f5f1e}", AutoMap = true)]
    public class TabIcon : ContentBase
    {
        [SitecoreField(FieldName = "IconClass")]
        public virtual string IconClass { get; set; }
    }
}