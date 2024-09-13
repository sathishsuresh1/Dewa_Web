using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.LectureBookingSvc;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface ILectureBookingClient
    {
        ServiceResponse<PutLectureBookingAwardSubmissionResponse> PutLectureBookingAwardSubmission(
            PutLectureBookingAwardSubmission request,
            string userId,
             SupportedLanguage language = SupportedLanguage.English,
             RequestSegment segment = RequestSegment.Desktop);
    }
}
