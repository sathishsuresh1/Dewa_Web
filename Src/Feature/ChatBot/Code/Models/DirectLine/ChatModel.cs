using DEWAXP.Foundation.DataAnnotations;
using System.Web.Mvc;

namespace DEWAXP.Feature.ChatBot.Models.DirectLine
{
    public class ChatModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [AllowHtml]
        public string Chat { get; set; }

        public string captcha { get; set; }

        public string InitialJson { get; set; }
        public bool IsLoggedInUser { get; set; }
    }
}