using DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Helpers.ConsumptionComplaint
{
    public class ConsumptionSessionHelper
    {
        //#region [Smart Response]
        //public static JsonMasterModel ElectricityComplaintJsonSetting
        //{
        //    get
        //    {

        //        var d = (JsonMasterModel)HttpContext.Current.Session["ElectricityComplaintJsonSetting"];
        //        if (d == null)
        //        {
        //            d = CommonHelper.LoadUpdatedSmartDubaiModelJson();
        //        }

        //        return d;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["ElectricityComplaintJsonSetting"] = value;

        //    }

        //}
        //public static JsonMasterModel WaterComplaintJsonSetting
        //{
        //    get
        //    {
        //        return (JsonMasterModel)HttpContext.Current.Session["WaterComplaintJsonSetting"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["WaterComplaintJsonSetting"] = value;

        //    }

        //}


        //public static Dictionary<int, CommonRender> CurrentUserAnswer
        //{
        //    get
        //    {
        //        return (Dictionary<int, CommonRender>)HttpContext.Current.Session["CurrentUserAnswer"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CurrentUserAnswer"] = value;

        //    }

        //}


        //public static Dictionary<SM_Id, string> UserSelectedIdAnswer
        //{
        //    get
        //    {
        //        return (Dictionary<SM_Id, string>)HttpContext.Current.Session["UserSelectedIdAnswer"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["UserSelectedIdAnswer"] = value;

        //    }

        //}

        //public static string SmartResponseJsonText
        //{
        //    get
        //    {
        //        return Convert.ToString(HttpContext.Current.Session["SmartResponseJsonText"]);
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["SmartResponseJsonText"] = value;

        //    }

        //}
        //#endregion

        #region [ConsumptionComplaint]
        public static JsonMasterModel ConsumptionComplaintJsonSetting
        {
            get
            {

                var d = (JsonMasterModel)HttpContext.Current.Session["ConsumptionComplaintJsonSetting"];
                if (d == null)
                {
                    d = ConsumptionHelper.LoadUpdatedConsumptionComplaintModelJson();
                }

                return d;
            }
            set
            {
                HttpContext.Current.Session["ConsumptionComplaintJsonSetting"] = value;

            }

        }


        public static Dictionary<int, CommonRender> CC_CurrentUserAnswer
        {
            get
            {
                return (Dictionary<int, CommonRender>)HttpContext.Current.Session["CC_CurrentUserAnswer"];
            }
            set
            {
                HttpContext.Current.Session["CC_CurrentUserAnswer"] = value;

            }

        }


        public static Dictionary<SM_Id, string> CC_UserSelectedIdAnswer
        {
            get
            {
                return (Dictionary<SM_Id, string>)HttpContext.Current.Session["CC_UserSelectedIdAnswer"];
            }
            set
            {
                HttpContext.Current.Session["CC_UserSelectedIdAnswer"] = value;

            }

        }

        public static string CC_JsonText
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CC_JsonText"]);
            }
            set
            {
                HttpContext.Current.Session["CC_JsonText"] = value;

            }

        }


        public static DEWAXP.Foundation.Integration.DewaSvc.notificationInfoOut CC_GetConsumptionDetail
        {
            get
            {
                return (DEWAXP.Foundation.Integration.DewaSvc.notificationInfoOut)(HttpContext.Current.Session["CC_GetConsumptionDetail"]);
            }
            set
            {
                HttpContext.Current.Session["CC_GetConsumptionDetail"] = value;

            }
        }
        #endregion
    }
}