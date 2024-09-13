using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
	public interface IEServicesClient
	{
		/// <summary>
		/// Links a MyID user to DEWA using their DEWA Business Partner details
		/// Relates to SAP WS17
		/// </summary>
		/// <param name="params">Requesting user's MyID contact information</param>
		/// <param name="language">Desired language of the response message</param>
		/// <returns>A response containing the user's Emirates ID and session token</returns>
		ServiceResponse<string> LinkBusinessPartnerToMyId(LinkBusinessPartnerToMyId @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

		/// <summary>
		/// Retrieves the clearance certificate related to a particular contract account
		/// Relates to SAP WS18
		/// </summary>
		/// <param name="userId">The unique identifier of the requesting user</param>
		/// <param name="sessionId">The requesting user's active session identifier</param>
		/// <param name="contractAccountNumber">The account to be queried</param>
		/// <param name="language">Desired language of the response message</param>
		/// <returns></returns>
		//ServiceResponse<ClearanceCertificateDetails> GetClearanceCertificate(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
	}
}
