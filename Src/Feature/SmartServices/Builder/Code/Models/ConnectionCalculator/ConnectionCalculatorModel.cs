using DEWAXP.Foundation.Content.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Builder.Models.ConnectionCalculator
{
    public class ConnectionCalculatorModel : GenericPageWithIntro
    {
        public decimal RequiredLoad { get; set; }
        public decimal ProposedLoad { get; set; }
    }
}