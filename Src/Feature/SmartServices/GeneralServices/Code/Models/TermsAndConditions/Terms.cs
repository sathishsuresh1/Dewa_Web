using DEWAXP.Foundation.Content.Models.Common;

namespace DEWAXP.Feature.GeneralServices.TermsAndConditions
{
    public class Terms : GenericPageWithIntro
    {
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
    }
}