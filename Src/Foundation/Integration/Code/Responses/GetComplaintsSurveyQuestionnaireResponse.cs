using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Responses
{
	public class ComplaintsSurveyQuestionnaireResponse
	{
		public string SurveyComments;

		public string Responsedesciption;

		public string NotificationKey;

		public string[] QuestionArabic;

		public string[] QuestionEnglish;

		public string[] QuestionNo;

		public string ResponseCode;

		public ComplaintsSurveyQuestionnaireResponse()
        {
          
        }
		
    }

  
}
