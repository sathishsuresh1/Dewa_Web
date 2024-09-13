using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.Responses
{

	//<?xml version="1.0" encoding="UTF-8" standalone="no"?>
	//<GetRoadWorkNotif>
	//<ContractAccount/>
	//<DateTimeStamp>20151119172814</DateTimeStamp>
	//<ResponseCode>399</ResponseCode>
	//<Description>There is no ongoing Roadwork.</Description>
	//</GetRoadWorkNotif>

	//<?xml version="1.0" encoding="UTF-8" standalone="no"?>
	//<GetRoadWorkNotif>
	//	<DateTimeStamp>20151127174545</DateTimeStamp>
	//	<ResponseCode>000</ResponseCode>
	//	<Description>Sucess</Description>
	//	<Item>
	//		<CITY_NAME>118-NAIF</CITY_NAME>
	//		<Street>AL BURJ STREET</Street>
	//		<DISRUPTION>ACCESS TO PROPERTY</DISRUPTION>
	//		<START_DATE>23.11.2015</START_DATE>
	//		<END_DATE>31.01.2016</END_DATE>
	//		<X_COORDINATE>25.270207127356304</X_COORDINATE>
	//		<Y_COORDINATE>55.306943842535148</Y_COORDINATE>
	//	</Item>
	//	<Item>
	//		<CITY_NAME>122-AL BARAHA</CITY_NAME>
	//		<Street>STREET</Street>
	//		<DISRUPTION>ACCESS TO PROPERTY</DISRUPTION>
	//		<START_DATE>23.11.2015</START_DATE>
	//		<END_DATE>30.12.2015</END_DATE>
	//		<X_COORDINATE>25.279562125494042</X_COORDINATE>
	//		<Y_COORDINATE>55.319899913324214</Y_COORDINATE>
	//	</Item>
	//</GetRoadWorkNotif>

	[Serializable]
	[XmlRoot(ElementName = "GetRoadWorkNotif")]
	public class RoadWorksListResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "DateTimeStamp")]
		public string DateTimeStamp { get; set; }
		[XmlElement("Item")]
        public List<RoadWork> RoadWorks { get; set; }
	}

	[Serializable]
    [XmlRoot(ElementName = "Item")]
    public class RoadWork
    {
		[XmlElement(ElementName = "CITY_NAME")]
		public string CITY_NAME { get; set; }
		[XmlElement(ElementName = "Street")]
		public string Street { get; set; }
		[XmlElement(ElementName = "DISRUPTION")]
		public string DISRUPTION { get; set; }
		[XmlElement(ElementName = "START_DATE")]
		public string START_DATE { get; set; }
		[XmlElement(ElementName = "END_DATE")]
		public string END_DATE { get; set; }
		[XmlElement(ElementName = "X_COORDINATE")]
		public string X_COORDINATE { get; set; }
		[XmlElement(ElementName = "Y_COORDINATE")]
		public string Y_COORDINATE { get; set; }
	}
}
