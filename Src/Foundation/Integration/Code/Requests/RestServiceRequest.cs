using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class RestServiceRequest
    {
        public string SqlQuery { get; set; }
        public string DB_Username { get; set; }
        public string DB_Password { get; set; }

        public List<RestServiceRequestParam> SqlParamters { get; set; }
    }
    public class RestServiceRequestParam
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
