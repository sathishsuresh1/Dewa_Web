using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.Bills.CollectiveStatement
{
    public class ConfirmModel : ContentBase
    {
        public string rc { get; set; }

        public string ca { get; set; }

        public string ErrorMessage { get; set; }

        public string ButtonUrl { get; set; }
    }
}