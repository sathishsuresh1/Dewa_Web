using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Serializable]
    public class Services
    {
        public string Service { get; set; }

        public UserType SelectedMethod { get; set; }

        public int Step { get; set; }

        public bool IsUserLoggedIn { get; set; }
    }
}