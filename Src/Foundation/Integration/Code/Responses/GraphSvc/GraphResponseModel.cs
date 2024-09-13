using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.GraphSvc
{  
        public class Header
        {
            public string verb { get; set; }
            public string noun { get; set; }
            public int revision { get; set; }
            public DateTime dateTime { get; set; }
        }

        public class Reply
        {
            public int replyCode { get; set; }
            public string replyText { get; set; }
        }

        public class ServicePoint
        {
            public string mRID { get; set; }
        }

        public class ReadingType
        {
            public string mRID { get; set; }
            public string measurementType { get; set; }
            public string unit { get; set; }
            public int channelNumber { get; set; }
            public int intervalLength { get; set; }
        }

        public class Quality
        {
            public DateTime versionTime { get; set; }
            public string validationStatus { get; set; }
            public bool noData { get; set; }
        }

        public class IReading
        {
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
            public int intervalLength { get; set; }
            public double? value { get; set; }
            public string measurementSource { get; set; }
            public Quality Quality { get; set; }
            public int flags { get; set; }
            public string lastUpdatedBy { get; set; }
        }

        public class IntervalBlock
        {
            public ReadingType ReadingType { get; set; }
            public List<IReading> IReading { get; set; }
        }

        public class MeterReading
        {
            public ServicePoint ServicePoint { get; set; }
            public IntervalBlock IntervalBlock { get; set; }
        }

        public class Payload
        {
            public MeterReading MeterReading { get; set; }
        }

        public class ReplyMessage
        {
            public Header Header { get; set; }
            public Reply Reply { get; set; }
            public Payload Payload { get; set; }
        }

        public class RootObject
        {
            public ReplyMessage ReplyMessage { get; set; }
        }
    
}
