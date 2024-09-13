
using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class FindEstimateModel
    {
	    public FindEstimateModel()
	    {
		    History = new EstimatePaymentHistoryModel();
	    }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "validation message")]
        [MinLength(9, ValidationMessageKey = "validation message")]
        [MaxLength(10, ValidationMessageKey = "validation message")]
        public string EstimateNumber { get; set; }

        public EstimatePaymentHistoryModel History { get; set; }
    }
}