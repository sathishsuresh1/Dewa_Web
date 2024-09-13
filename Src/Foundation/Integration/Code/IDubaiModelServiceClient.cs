using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
	public interface IDubaiModelServiceClient
	{
		ServiceResponse<ContractAccountClassificationResponse> GetAccountClassification(string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
	}
}
