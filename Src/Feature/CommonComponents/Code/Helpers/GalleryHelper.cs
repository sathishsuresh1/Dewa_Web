using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using DEWAXP.Foundation.Content.Repositories;
using Glass.Mapper.Sc;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.CommonComponents.Helpers
{
    public class GalleryHelper
    {
        private IContentRepository _contentRepository;

        public GalleryHelper()
        {
        }

        public GalleryHelper(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public IEnumerable<M65GalleryTeaser> GetGallery(Guid itemId, out int count)
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));
            if (listing == null)
            {
                count = 0;
                return null;
            }
            var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<M65GalleryTeaser>(new GetItemByItemOptions(c))).Where(c => c != null && (c.TemplateId == Guid.Parse("{A72A1FCC-CDB9-4949-842F-A8001075A7EA}")))
                .ToList();
            IEnumerable<M65GalleryTeaser> filteredSet = children.OrderByDescending(c => c.PublishDate).Take(4);

            count = children.Count;
            return filteredSet;
        }
    }
}