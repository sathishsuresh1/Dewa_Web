using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    public class M9TeaserSetWithSearchTextModel
    {
        public virtual string SearchText { get; set; }

        public virtual string DatasourceId { get; set; }

        public virtual string pageNo { get; set; }
       public virtual List<CarouselSlide> CarouselSlides { get; set; }

        public PaginationModel PaginationInfo { get; set; }

        public ContentBase CurrentPage { get; set; }
    }
}