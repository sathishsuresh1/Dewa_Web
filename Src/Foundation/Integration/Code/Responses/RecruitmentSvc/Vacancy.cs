using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.RecruitmentSvc;

namespace DEWAXP.Foundation.Integration.Responses.RecruitmentSvc
{
	[Serializable]
	public class Vacancy
	{
		public string Identifier { get; set; }

		public string Reference { get; set; }

		public DateTime ListingDate { get; set; }

		public DateTime OpeningDate { get; set; }

		public DateTime ClosingDate { get; set; }

		public string Job { get; set; }

		public string RequireDescription { get; set; }

		public string Description { get; set; }

		internal static Vacancy From(DT_JobSearch_RespItem item)
		{
			return new Vacancy
			{
				Identifier = item.ObjectID,
				Job = item.ObjectText,
				Reference = item.ReferenceCode,
				RequireDescription = item.RequireDescription,
				Description = item.TaskDescription,
				OpeningDate = item.StartDate,
				ClosingDate = item.EndDate,
				ListingDate = item.PublishDate
			};
		}
	}
}
