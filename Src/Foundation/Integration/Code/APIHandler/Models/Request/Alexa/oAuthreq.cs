using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Alexa
{
    public class oAuthreq
    {
        [Required]
        public string state { get; set; }

        [Required]
        public string client_id { get; set; }

        [Required]
        public string response_type { get; set; }

        public string scope { get; set; }

        public string redirect_uri { get; set; }
    }
}