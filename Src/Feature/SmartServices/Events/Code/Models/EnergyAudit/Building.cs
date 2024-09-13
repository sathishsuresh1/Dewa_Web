using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.DataAnnotations;
using Sitecore.Globalization;

namespace DEWAXP.Feature.Events.Models.EnergyAudit
{
	public class Building
	{
		[Required(AllowEmptyStrings = false, ValidationMessageKey = DictionaryKeys.BuildingAudit.ContractAccountNumberValidationMessage)]
		public string ContractAccountNumber { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
		public string BuildingName { get; set; }

		public int? FloorArea { get; set; }

		public int NumberOfBuildingsToBeAudited { get; set; }

		public string Detail
		{
			get { return Translate.Text("J85.ElectWater"); }
		}
	}
}