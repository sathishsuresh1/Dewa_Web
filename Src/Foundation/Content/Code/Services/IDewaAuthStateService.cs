namespace DEWAXP.Foundation.Content.Services
{
    public interface IDewaAuthStateService
    {
        void Save(DewaProfile profile);

        DewaProfile GetActiveProfile();
    }
}