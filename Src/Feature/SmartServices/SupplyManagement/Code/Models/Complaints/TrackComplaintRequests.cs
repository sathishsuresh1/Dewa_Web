using DEWAXP.Foundation.Content.Models.ContactDetails;
using System.Collections.Generic;
using System.Linq;
namespace DEWAXP.Feature.SupplyManagement.Models.Complaints
{
    public class TrackComplaintRequests
    {
        public TrackComplaintRequests(List<ComplaintRequest> requests, ContactDetails contactDetails)
        {
            ContactDetails = contactDetails;

            Requests = requests;

            if (contactDetails != null && requests != null && requests.Count > 0)
            {
                Requests = Requests.Select(c => { c.AccountNo = contactDetails.AccountNumber; return c; })?.ToList();
            }
        }

        public List<ComplaintRequest> Requests { get; private set; }
        public ContactDetails ContactDetails { get; private set; }
    }
}