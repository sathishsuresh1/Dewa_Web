using DEWAXP.Feature.FormsExtensions.XDb.Model;

namespace DEWAXP.Feature.FormsExtensions.XDb
{
    public interface IXDbContactFactory
    {
        IXDbContact CreateContact(string identifierValue);

        IXDbContactWithEmail CreateContactWithEmail(string email);
    }
}