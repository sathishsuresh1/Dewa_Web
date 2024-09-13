using Sitecore.Data;

namespace DEWAXP.Feature.IdealHome.Models.IdealHome
{
    public class SurveyLogin
    {
        public string EntityCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MediaUrl { get; set; }
        public ID UserItemID { get; set; }

    }
}