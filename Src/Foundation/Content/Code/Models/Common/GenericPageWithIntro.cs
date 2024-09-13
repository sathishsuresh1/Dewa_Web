using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Foundation.Content.Models.Common
{
	public class GenericPageWithIntro : PageBase
	{
		public virtual string Intro { get; set; }
        public virtual bool MandatoryNote { get; set; }
	}
}