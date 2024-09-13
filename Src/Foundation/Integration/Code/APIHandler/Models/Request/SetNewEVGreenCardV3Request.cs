using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request
{
    public class SetNewEVGreenCardV3Request : ApiBaseRequest
    {
        public string file1Data { get; set; }
        public string file1Name { get; set; }
        public string file2Data { get; set; }
        public string file2Name { get; set; }
        public string file3Data { get; set; }
        public string file3Name { get; set; }
        public string file4Data { get; set; }
        public string file4Name { get; set; }
        public string addressTitle { get; set; }
        public string attachment { get; set; }
        public string bpCategory { get; set; }
        public string bpFirstName { get; set; }
        public string bpLastName { get; set; }
        public string bpNumber { get; set; }
        public string bpRegion { get; set; }
        public string branchName { get; set; }
        public string carCategory { get; set; }
        public string carIdNumber { get; set; }
        public string carPlateCode { get; set; }
        public string carRegistratedCountry { get; set; }
        public string carRegistratedRegion { get; set; }
        public string carIdType { get; set; }
        public string emailId { get; set; }
        public string idNumber { get; set; }
        public string idexpiry { get; set; }

        public string dateofbirth { get; set; }

        public string idType { get; set; }
        public string mobileNumber { get; set; }
        public string nationality { get; set; }
        public string noOfCars { get; set; }
        public string password { get; set; }
        public string poBox { get; set; }
        public string processFlag { get; set; }
        public string requestNumber { get; set; }
        public string serviceProviderId { get; set; }
        public string tradelicenceauthoritycode { get; set; }
        public string tradelicenceauthorityname { get; set; }
        public string trafficFileNumber { get; set; }
        public string userCreateFlag { get; set; }
        public string userId { get; set; }
    }


}
