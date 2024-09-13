using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Dashboard.Models
{
	[Serializable]
	public class UserModel
	{
        public string Name { get; set; }

        public string Username { get; set; }
        
        public string SessionToken { get; set; }

	    public string EmailAddress { get; set; }

	    public string MobileNumber { get; set; }

	    public bool UserHasProfilePhoto { get; set; }

	    public byte[] UserProfilePhoto { get; set; }
	}
}