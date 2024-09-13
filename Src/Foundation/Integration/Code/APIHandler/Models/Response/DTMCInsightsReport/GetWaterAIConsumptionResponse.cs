using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCInsightsReport
{
    public class GetWaterAIConsumptionResponse
    {
        public GWAICR_ReplyMessage ReplyMessage { get; set; }
    }

    public class GWAICR_ReplyMessage
    {
        public GWAICR_RM_Header Header { get; set; }

        public GWAICR_RM_Reply Reply { get; set; }

        public GWAICR_RM_Payload Payload { get; set; }
    }

    #region [Reply Message]

    /// <summary>
    /// Get Water AI Consumption Response> ReplyMessage> Header
    /// </summary>
    public class GWAICR_RM_Header
    {
        public string verb { get; set; }
        public string noun { get; set; }
        public string revision { get; set; }
        public string dateTime { get; set; }
    }
    /// <summary>
    /// Get Water AI Consumption Response> ReplyMessage> Reply
    /// </summary>
    public class GWAICR_RM_Reply
    {
        public string replyCode { get; set; }
        public string replyText { get; set; }
    }
    /// <summary>
    /// Get Water AI Consumption Response> ReplyMessage> Payload
    /// </summary>
    public class GWAICR_RM_Payload
    {
        public GWAICR_RM_P_MeterReading MeterReading { get; set; }
    }
    #region [Playload]
    /// <summary>
    /// Get Water AI Consumption Response> ReplyMessage> Payload >MeterReading
    /// </summary>
    public class GWAICR_RM_P_MeterReading
    {
        public GWAICR_RM_P_MR_ServicePoint ServicePoint { get; set; }
        public GWAICR_RM_P_MR_IntervalBlock IntervalBlock { get; set; }
    }
    #region [MeterReading]

    /// <summary>
    ///  Get Water AI Consumption Response> ReplyMessage> Payload> MeterReading> ServicePoint
    /// </summary>
    public class GWAICR_RM_P_MR_ServicePoint
    {
        public string mRID { get; set; }
    }
    /// <summary>
    ///  Get Water AI Consumption Response> ReplyMessage> Payload> MeterReading> IntervalBlock
    /// </summary>
    public class GWAICR_RM_P_MR_IntervalBlock
    {
        public GWAICR_RM_P_MR_IB_ReadingType ReadingType { get; set; }
        public List<GWAICR_RM_P_MR_IB_IReading> IReading { get; set; }
    }

    #region [IntervalBlock]
    /// <summary>
    ///  Get Water AI Consumption Response> ReplyMessage> Payload> MeterReading> IntervalBlock> ReadingType
    /// </summary>
    public class GWAICR_RM_P_MR_IB_ReadingType
    {
        public string mRID { get; set; }
        public string measurementType { get; set; }
        public string unit { get; set; }
        public string touCode { get; set; }
        public string channelNumber { get; set; }
    }
    /// <summary>
    ///  Get Water AI Consumption Response> ReplyMessage> Payload> MeterReading> IntervalBlock> IReading
    /// </summary>
    public class GWAICR_RM_P_MR_IB_IReading
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string value { get; set; }
        public string demandPeakTime { get; set; }
    }
    #endregion

    #endregion

    #endregion

    #endregion





}
