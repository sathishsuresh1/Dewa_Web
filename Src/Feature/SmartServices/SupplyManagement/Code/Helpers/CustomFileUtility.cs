//using DEWAXP.Feature.SupplyManagement.Helpers.ConsumptionComplaint;
using DEWAXP.Foundation.Helpers;
using System.Collections.Generic;
using System.IO;

namespace DEWAXP.Feature.SupplyManagement.Helpers
{
    public class CustomFileUtility
    {
        public static T LoadJson<T>(string filePath, bool isVirtualPath = false)
        {
            T items;
            if (isVirtualPath)
            {
                filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
            }

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = CustomJsonConvertor.DeserializeObject<T>(json);
            }
            return items;
        }

        public static Models.SmartResponseModel.JsonMasterModel LoadSmartDubaiModelJson(string filePath, bool isVirtualPath = false)
        {
            Models.SmartResponseModel.JsonMasterModel items;

            if (string.IsNullOrWhiteSpace(SmartResponse.SmartResponseSessionHelper.SmartResponseJsonText))
            {
                if (isVirtualPath)
                {
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                }
                using (StreamReader r = new StreamReader(filePath))
                {
                    SmartResponse.SmartResponseSessionHelper.SmartResponseJsonText = r.ReadToEnd();
                    items = Models.SmartResponseModel.JsonMasterModel.FromJson(SmartResponse.SmartResponseSessionHelper.SmartResponseJsonText);
                }
            }
            else
            {
                items = Models.SmartResponseModel.JsonMasterModel.FromJson(SmartResponse.SmartResponseSessionHelper.SmartResponseJsonText);
            }
            return items;
        }


        public static Models.ConsumptionComplaint.JsonMasterModel LoadConsumptionComplaintJson(string filePath, bool isVirtualPath = false)
        {
            Models.ConsumptionComplaint.JsonMasterModel items;

            if (string.IsNullOrWhiteSpace(ConsumptionComplaint.ConsumptionSessionHelper.CC_JsonText))
            {
                if (isVirtualPath)
                {
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                }
                using (StreamReader r = new StreamReader(filePath))
                {
                    ConsumptionComplaint.ConsumptionSessionHelper.CC_JsonText = r.ReadToEnd();
                    items = Models.ConsumptionComplaint.JsonMasterModel.FromJson(ConsumptionComplaint.ConsumptionSessionHelper.CC_JsonText);
                }
            }
            else
            {
                items = Models.ConsumptionComplaint.JsonMasterModel.FromJson(ConsumptionComplaint.ConsumptionSessionHelper.CC_JsonText);
            }
            return items;
        }
    }
}