using System.Collections.Generic;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses.RecruitmentSvc;

namespace DEWAXP.Foundation.Integration
{
	public interface IRecruitmentServiceClient
	{
		IEnumerable<Vacancy> GetVacancies(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

		Vacancy GetVacancy(string identifier, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
	}
}