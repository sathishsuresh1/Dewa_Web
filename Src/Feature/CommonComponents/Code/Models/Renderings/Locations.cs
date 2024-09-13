using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateId = "{781A439A-4B24-4627-833C-6D11873BDEC6}", AutoMap = true)]
    public class Location
    {
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<CustomerServiceSet> CustomerService { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<PaymentLocationset> PaymentLocation { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<WaterSupplyset> WaterSupply { get; set; }

        [SitecoreChildren(InferType = true)]

        //[SitecoreQuery("./*[@@templateid='4437E628-E5EB-4A99-8E4E-8E0DEAFA30A3']", IsRelative = true)]
        public virtual IEnumerable<FastEVChargeset> FastEVCharge { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<NormalEVChargeset> NormalEVCharge { get; set; }
    }

    [SitecoreType(TemplateId = "{9CEC0ECF-BD07-445A-B1FA-96EB9A112896}", AutoMap = true)]
    public class GenericLocations : ContentBase
    {
        
        [SitecoreField("latitude")]
        public virtual string latitude { get; set; }

        [SitecoreField("longitude")]
        public virtual string longitude { get; set; }

        [SitecoreField("name")]
        public virtual string name { get; set; }

        [SitecoreField("city")]
        public virtual string city { get; set; }

        [SitecoreField("whrs")]
        public virtual string whrs { get; set; }

        [SitecoreField("pimage")]
        public virtual Image pimage { get; set; }

        [SitecoreField("iimage")]
        public virtual Image iimage { get; set; }

        [SitecoreField("phone")]
        public virtual string phone { get; set; }

        [SitecoreField("address")]
        public virtual string address { get; set; }

    }

    [SitecoreType(TemplateId = "{967E171B-803B-4A05-AFCF-56B96A08CAC2}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class CustomerServiceSet
    {
        public CustomerServiceSet()
        {
            Children = new List<CustomerService>();
        }

        [SitecoreChildren]
        public virtual IEnumerable<CustomerService> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{F10E5806-B613-4ECF-BBC8-18A8CF839490}", AutoMap = true)]
    public class CustomerService : GenericLocations
    {

        public virtual string code { get; set; }
        public virtual string addressdetails { get; set; }
        public virtual string addressline1 { get; set; }
        public virtual string landmark { get; set; }
        public virtual string countrycode { get; set; }
        public virtual string zipcode { get; set; }
        public virtual string callcenternumber { get; set; }
        public virtual string emergencynumber { get; set; }
        public virtual string email { get; set; }
        public virtual string makaninumber { get; set; }
        public virtual string contacttext { get; set; }
        public virtual string businesscardtext { get; set; }
        public virtual Link businesscardlink { get; set; }

        [SitecoreField(FieldName = "services", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<DataSourceValueTemplate> services { get; set; }
        public virtual Link website { get; set; }

    }

    [SitecoreType(TemplateId = "{EF372E97-B3FC-4FB5-B21C-3749E70BADAA}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class PaymentLocationset
    {
        public PaymentLocationset()
        {
            Children = new List<PaymentLocation>();
        }

        [SitecoreChildren]
        public virtual IEnumerable<PaymentLocation> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{5EF39143-B418-47E4-8BBC-2639588D8B68}", AutoMap = true)]
    public class PaymentLocation : GenericLocations
    {
        public virtual string ccode { get; set; }
        public virtual Image fimage { get; set; }
        public virtual Link web { get; set; }
        public virtual string wname { get; set; }

        [SitecoreField(FieldName = "catg", Setting = SitecoreFieldSettings.InferType)]
        public virtual DataSourceValueTemplate catg { get; set; }
    }

    [SitecoreType(TemplateId = "{A0F2C587-E83B-4DFE-AED1-417CB601BEE0}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class WaterSupplyset
    {
        public WaterSupplyset()
        {
            Children = new List<WaterSupply>();
        }

        [SitecoreChildren]
        public virtual IEnumerable<WaterSupply> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{4F98626B-43C3-485E-873D-9E9254C504E6}", AutoMap = true)]
    public class WaterSupply : GenericLocations
    {
        public virtual string ccode { get; set; }
        public virtual Image fimage { get; set; }
        public virtual Link web { get; set; }
        public virtual string wname { get; set; }

        [SitecoreField(FieldName = "catg", Setting = SitecoreFieldSettings.InferType)]
        public virtual DataSourceValueTemplate catg { get; set; }
    }

    [SitecoreType(TemplateId = "{4437E628-E5EB-4A99-8E4E-8E0DEAFA30A3}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class FastEVChargeset
    {

        [SitecoreChildren]
        public virtual IEnumerable<FastEVCharge> Children { get; set; }
    }


    [SitecoreType(TemplateId = "{FA334C61-7D88-42EB-BF84-083A08911594}", AutoMap = true)]
    public class FastEVCharge : GenericLocations
    {
        [SitecoreField("ccode")]
        public virtual string ccode { get; set; }
        [SitecoreField("fimage")]
        public virtual Image fimage { get; set; }
        [SitecoreField("web")]
        public virtual Link web { get; set; }
        [SitecoreField("subname")]
        public virtual string subname { get; set; }
        [SitecoreField("code")]
        public virtual string code { get; set; }
    }

    [SitecoreType(TemplateId = "{F660C8F8-BD51-44A9-9879-86009C89D805}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class NormalEVChargeset
    {
        [SitecoreChildren]
        public virtual IEnumerable<NormalEVCharge> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{1F3F39FF-B5FF-4555-9018-4846CF654A79}", AutoMap = true)]
    public class NormalEVCharge : GenericLocations
    {
        public virtual string ccode { get; set; }
        public virtual Image fimage { get; set; }
        public virtual Link web { get; set; }
        public virtual string code { get; set; }
    }

    [SitecoreType(TemplateId = "{7A96389F-353B-40DA-BA04-800EF24DAADA}", AutoMap = true)]
    public class DataSourceValueTemplate
    {
        [SitecoreField("Value")]
        public virtual string serviceValue { get; set; }
    }

    public class LocationData
    {
        public JsonLocation Locations { get; set; }
    }

    public class JsonLocation
    {
        //public virtual List<CustomerServiceJsonModal> CustomerService { get; set; }

        public virtual CustomerServiceList CustomerService { get; set; }

        public virtual PaymentWaterLocationList PaymentLocations { get; set; }

        public virtual PaymentWaterLocationList WaterSupply { get; set; }

        public virtual FastEVChargeLocationList FastEVCharge { get; set; }

        public virtual NormalEVChargeLocationList NormalEVCharge { get; set; }
        //public List<PaymentWaterLocationJsonModal> PaymentLocations { get; set; }

        //public List<PaymentWaterLocationJsonModal> WaterSupply { get; set; }

        //public List<FastEVChargeJsonModal> FastEVCharge { get; set; }

        //public List<NormalEVChargeJsonModal> NormalEVCharge { get; set; }
    }
    //Inside JSON Item
    public class CustomerServiceList
    {
       public virtual List<CustomerServiceJsonModal> item { get; set; }
    }
    public class PaymentWaterLocationList
    {
       public virtual List<PaymentWaterLocationJsonModal> item { get; set; }
    }
    public class FastEVChargeLocationList
    {
       public virtual List<FastEVChargeJsonModal> item { get; set; }
    }
    public class NormalEVChargeLocationList
    {
       public virtual List<NormalEVChargeJsonModal> item { get; set; }
    }

    public class GenericLocationsJsonModal
    {
        public virtual string id { get; set; }
        public virtual string latitude { get; set; }

        public virtual string longitude { get; set; }

        public virtual string name { get; set; }

        public virtual string city { get; set; }

        public virtual string whrs { get; set; }
        public virtual string pimage { get; set; }

        public virtual string iimage { get; set; }

        public virtual string phone { get; set; }

        public virtual string address { get; set; }

    }

    public class CustomerServiceJsonModal : GenericLocationsJsonModal
    {
        public virtual string code { get; set; }
        public virtual string addressdetails { get; set; }
        public virtual string addressline1 { get; set; }
        public virtual string landmark { get; set; }
        public virtual string countrycode { get; set; }
        public virtual string zipcode { get; set; }
        public virtual string callcenternumber { get; set; }
        public virtual string emergencynumber { get; set; }
        public virtual string email { get; set; }
        public virtual string makaninumber { get; set; }
        public virtual string contacttext { get; set; }
        public virtual string businesscardtext { get; set; }
        public virtual string businesscardlink { get; set; }
        public ServicesList services { get; set; }
        public virtual string website { get; set; }

    }

    public class PaymentWaterLocationJsonModal : GenericLocationsJsonModal
    {
        public virtual string ccode { get; set; }
        public virtual string fimage { get; set; }
        public virtual string web { get; set; }
        public virtual string wname { get; set; }
        public virtual string catg { get; set; }
    }
    public class FastEVChargeJsonModal : GenericLocationsJsonModal
    {
        public virtual string code { get; set; }
        public virtual string ccode { get; set; }
        public virtual string fimage { get; set; }
        public virtual string web { get; set; }
        public virtual string subname { get; set; }

    }

    public class NormalEVChargeJsonModal : GenericLocationsJsonModal
    {
        public virtual string code { get; set; }
        public virtual string ccode { get; set; }
        public virtual string fimage { get; set; }
        public virtual string web { get; set; }

    }

    public class ServicesList
    {
       public virtual List<string> P { get; set; }
    }
}