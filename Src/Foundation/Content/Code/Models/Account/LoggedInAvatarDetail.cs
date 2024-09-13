using DEWAXP.Foundation.Content.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    public class LoggedInAvatarDetail
    {
        public string BPNumber { get; set; }
        public string UserName { get; set; }
        public ProfilePhotoModel ProfileImage { get; set; }
    }
}