namespace DEWAXP.Foundation.Content.Models.Bills
{
    public class FindBillModel
    {
        [DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter an account or premise number")]
        [DataAnnotations.MaxLength(10, ValidationMessageKey = "Please enter an account or premise number")]
        public string SearchCriteria { get; set; }
    }
}