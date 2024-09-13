using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateId = "{7E872502-6672-4BA7-8123-9C02F56EEA8D}", AutoMap = true)]
    public class JourneyMap : ContentBase
    {
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        //public JourneyMap()
        //{
        //    Personas = new List<JourneyPersona>();
        //    TileSets = new List<JourneyTileSet>();
        //}

        [SitecoreField("Personas")]
        public virtual IEnumerable<JourneyPersona> Personas { get; set; }

        [SitecoreField("Tiles")]
        public virtual IEnumerable<JourneyTileSet> TileSets { get; set; }
    }

    [SitecoreType(TemplateId = "{B626F9F9-3432-4AE2-B805-C69477A41874}", AutoMap = true)]
    public class JourneyPersona : ContentBase
    {
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        [SitecoreField("Background Image")]
        public virtual Image BackgroudImage { get; set; }

        [SitecoreField("Color")]
        public virtual JourneyColor ColorClass { get; set; }

        [SitecoreField("SubTitle")]
        public virtual string SubTitle { get; set; }
    }

    [SitecoreType(TemplateId = "{950FAA0C-F2FD-46A2-923D-D37AC148E9F3}", AutoMap = true)]
    public class JourneyTileSet : ContentBase
    {
        //public JourneyTileSet()
        //{
        //    TileGroups = new List<JourneyTileGroup>();
        //}

        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        //[SitecoreField("Tiles")]
        [SitecoreChildren]
        public virtual IEnumerable<JourneyTileGroup> TileGroups { get; set; }
    }

    [SitecoreType(TemplateId = "{73D5BE8F-6E19-449C-90A1-272C94D0F601}", AutoMap = true)]
    public class JourneyTileGroup : ContentBase
    {
        [SitecoreField("Order")]
        public virtual string OrderNo { get; set; }

        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        [SitecoreField("Grouping Set")]
        public virtual JourneyGroupingSet JourneyGroupingSet { get; set; }
        

        //public JourneyTileGroup()
        //{
        //    Tiles = new List<JourneyTile>();
        //}

        //[SitecoreField("Tiles")]
        [SitecoreChildren]
        public virtual IEnumerable<JourneyTile> Tiles { get; set; }
    }

    [SitecoreType(TemplateId = "{EFBF1B03-7BB2-4D33-AB6B-6D951AA8E47A}", AutoMap = true)]
    public class JourneyTile : ContentBase
    {
        //public JourneyTile()
        //{
        //    TilePersona = new JourneyPersona();
        //}

        [SitecoreField("Order")]
        public virtual string OrderNo { get; set; }

        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        [SitecoreField(FieldName = "Steps", Setting = SitecoreFieldSettings.RichTextRaw)]
        public virtual string Step { get; set; }

        [SitecoreField(FieldName = "Note", Setting = SitecoreFieldSettings.RichTextRaw)]
        public virtual string Note { get; set; }

        [SitecoreField("Icon")]
        public virtual JourneyIcon IconClass { get; set; }

        [SitecoreField("Color")]
        public virtual JourneyColor ColorClass { get; set; }

        [SitecoreField("Persona")]
        public virtual JourneyPersona Persona { get; set; }
    }

    [SitecoreType(TemplateId = "{38F783C4-B4D6-434E-A123-16BC81F96126}", AutoMap = true)]
    public class JourneyIcon : ContentBase
    {
        [SitecoreField(FieldName = "IconClass")]
        public virtual string IconClass { get; set; }
    }

    [SitecoreType(TemplateId = "{EB4C65AC-002F-476A-8A7B-84C4BC5193F7}", AutoMap = true)]
    public class JourneyColor : ContentBase
    {
        [SitecoreField(FieldName = "ColorClass")]
        public virtual string ColorClass { get; set; }
    }
    [SitecoreType(TemplateId = "{79A6D392-798D-47E6-9A01-2B7D2B4EBFA3}", AutoMap = true)]
    public class JourneyGroupingSet
    {
        [SitecoreField("Persona")]
        public virtual JourneyPersona Persona { get; set; }
        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }
        
    }
}