using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    [Serializable]
    public class RecoveryEmail
    {    
        public string EmailAddress { get; set; }

        public RecoveryContext Context { get; set; }
    }

    public enum RecoveryContext
    {
        Username = 0,
        Password = 1
    }
}