using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.meterreading
{
    public class MeterreadingResponse
    {
       
        public MeterReadingReplyMessage ReplyMessage { get; set; }
        public float electricity_Reading { get; set; }
        public string enddate { get; set; }
        public string startdate { get; set; }
        public string errorCodeE { get; set; }
        public string errorCodeW { get; set; }
        public string errorMessageE { get; set; }
        public string errorMessageW { get; set; }
        public float water_Reading { get; set; }

        public DateTime? end_date
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(enddate))
                {
                    return Convert.ToDateTime(enddate);
                }
                return null;
            }
        }
        public DateTime? start_date
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(startdate))
                {
                    return Convert.ToDateTime(startdate);
                }
                return null;
            }
        }
    }

    public class MeterReadingReplyMessage
    {
        public MeterReading_RM_Header Header { get; set; }

        public MeterReading_RM_Reply Reply { get; set; }


    }



    public class MeterReading_RM_Reply
    {
        public string replyCode { get; set; }
        public string replyText { get; set; }
    }

    public class MeterReading_RM_Header
    {
        public string verb { get; set; }
        public string noun { get; set; }
        public string revision { get; set; }
        public string dateTime { get; set; }
    }
}
