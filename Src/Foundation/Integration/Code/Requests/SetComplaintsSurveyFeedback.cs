using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
	public class ComplaintsSurveyFeedback
	{

		public string[] QuestionNumber { get; set; }

		public string[] AnswerChoice { get; set; }

		public string Comment { get; set; }

		public string notificationkey { get; set; }


		public ComplaintsSurveyFeedback()
		{

		}
	}
}
