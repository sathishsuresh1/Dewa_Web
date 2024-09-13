using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;
    using Glass.Mapper.Sc.Fields;

    // <summary>
    /// Defines the <see cref="M108Folder" />.
    /// </summary>
    [SitecoreType(TemplateId = "{4346777C-D5C5-4110-9141-7831872F055E}", AutoMap = true)]
    public class M108Folder 
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the SubTitle.
        /// </summary>
        [SitecoreField("SubTitle")]
        public virtual string SubTitle { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [SitecoreField("URL")]
        public virtual Link URL { get; set; }
        /// <summary>
        /// Gets or sets the Personas.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M108ListContainer> ListItems { get; set; }
    }
}