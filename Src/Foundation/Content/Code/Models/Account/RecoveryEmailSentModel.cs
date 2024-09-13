using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    [Serializable]
    public class RecoveryEmailSentModel
    {
        public string EmailAddress { get; set; }

        public RecoveryContext Context { get; set; }
    }

    public enum RecoveryContext
    {
        Username = 0,
        Password = 1,
        CandidateProfileUsername = 2
    }
}