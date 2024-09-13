using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.QmsSvc
{
    [XmlRoot(ElementName = "WsGetServiceStatusResp")]
    public class BranchServiceStatusResponse
    {
        [XmlElement(ElementName = "BranchStatusList")]
        public BranchStatusList BranchStatusList { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
    [XmlRoot(ElementName = "BranchStatusList")]
    public class BranchStatusList
    {
        [XmlElement(ElementName = "BranchStatus")]
        public List<BranchStatus> BranchStatus { get; set; }
    }
    public class BranchStatus
    {
        [XmlElement(ElementName = "Branch")]
        public Branch Branch { get; set; }

        [XmlElement(ElementName = "ServiceStatusList")]
        public ServiceStatusList ServiceStatusList { get; set; }
    }
    [XmlRoot(ElementName = "Branch")]
    public class Branch
    {
        [XmlElement(ElementName = "IndBranch")]
        public IndBranch IndBranch { get; set; }
    }
    [XmlRoot(ElementName = "IndBranch")]
    public class IndBranch
    {
        [XmlElement(ElementName = "BranchCode")]
        public string BranchCode { get; set; }

        [XmlElement(ElementName = "BranchName")]
        public string BranchName { get; set; }
    }
}
