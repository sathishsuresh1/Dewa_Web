using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class FileUploader : ContentBase
	{
		[SitecoreField("Upload description")]
		public virtual string UploadDescription { get; set; }

		[SitecoreField("Upload button text")]
		public virtual string UploadButtonText { get; set; }

		[SitecoreField("Remove button text")]
		public virtual string RemoveButtonText { get; set; }

		[SitecoreField("Notes")]
		public virtual string Notes { get; set; }

		[SitecoreField("Upload name")]
		public virtual string UploadName { get; set; }

		[SitecoreField("Image data source")]
		public virtual string ImageDataSource { get; set; }
	}
}