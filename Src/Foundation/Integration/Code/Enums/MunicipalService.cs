using System;

namespace DEWAXP.Foundation.Integration.Enums
{
    public enum MunicipalService
    {
        Unknown = -1,
        Electricity = 0,
        Water = 1,
        EV = 2
    }

	public static class MunicipalServiceExtensions
	{
		public static string Code(this MunicipalService service)
		{
			switch (service)
			{
				case MunicipalService.Electricity:
					return "D8";
				case MunicipalService.Water:
					return "D9";
				default:
					throw new ArgumentOutOfRangeException("service");
			}
		}
	}
}