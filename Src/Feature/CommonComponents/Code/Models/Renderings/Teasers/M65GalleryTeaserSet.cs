using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    public class M65GalleryTeaserSet : SectionTitle
    {
        public virtual IEnumerable<M65GalleryTeaser> VideoChildren { get; set; }
        public virtual IEnumerable<M65GalleryTeaser> ImageChildren { get; set; }
    }

    [SitecoreType(TemplateId = "{A72A1FCC-CDB9-4949-842F-A8001075A7EA}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class M65GalleryTeaser : ContentBase
    {
        [SitecoreField("Header")]
        public virtual string GalleryHeader { get; set; }

        [SitecoreField("Teaser Image")]
        public virtual Image BackgroundImage { get; set; }

        [SitecoreField("Publish Date")]
        public virtual DateTime PublishDate { get; set; }
    }
}