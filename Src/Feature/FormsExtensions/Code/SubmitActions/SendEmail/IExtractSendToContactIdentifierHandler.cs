using System.Collections.Generic;
using Sitecore.ExperienceForms.Processing;
using Sitecore.XConnect;

namespace DEWAXP.Feature.FormsExtensions.SubmitActions.SendEmail
{
    public interface IExtractSendToContactIdentifierHandler
    {
        IList<ContactIdentifier> GetContacts(SendEmailExtendedData data, FormSubmitContext formSubmitContext);
    }

}