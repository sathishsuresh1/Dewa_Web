using DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Helpers.SmartResponse
{
    public class SmartResponseSessionHelper
    {
        public static JsonMasterModel ElectricityComplaintJsonSetting
        {
            get
            {

                var d = (JsonMasterModel)HttpContext.Current.Session["ElectricityComplaintJsonSetting"];
                    if (d == null)
                {
                    d = SmartResponseHelper.LoadUpdatedSmartDubaiModelJson();
                }

                return d;
            }
            set
            {
                HttpContext.Current.Session["ElectricityComplaintJsonSetting"] = value;

            }

        }
        public static JsonMasterModel WaterComplaintJsonSetting
        {
            get
            {
                return (JsonMasterModel)HttpContext.Current.Session["WaterComplaintJsonSetting"];
            }
            set
            {
                HttpContext.Current.Session["WaterComplaintJsonSetting"] = value;

            }

        }


        public static Dictionary<int, CommonRender> CurrentUserAnswer
        {
            get
            {
                return (Dictionary<int, CommonRender>)HttpContext.Current.Session["CurrentUserAnswer"];
            }
            set
            {
                HttpContext.Current.Session["CurrentUserAnswer"] = value;

            }

        }


        public static Dictionary<SM_Id, string> UserSelectedIdAnswer
        {
            get
            {
                return (Dictionary<SM_Id, string>)HttpContext.Current.Session["UserSelectedIdAnswer"];
            }
            set
            {
                HttpContext.Current.Session["UserSelectedIdAnswer"] = value;

            }

        }

        public static string SmartResponseJsonText
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["SmartResponseJsonText"]);
            }
            set
            {
                HttpContext.Current.Session["SmartResponseJsonText"] = value;

            }

        }
    }
}