using System;

namespace DEWAXP.Foundation.Integration.Enums
{
	public enum ComplaintPriority
	{
		High,
		Low,
		Nil
	}

	public static class ComplaintPriorityExtensions
	{
		public static string Code(this ComplaintPriority priority)
		{
			switch (priority)
			{
				case ComplaintPriority.High:
					return "D001";
				case ComplaintPriority.Low:
					return "D002";
				case ComplaintPriority.Nil:
					return "D003";
				default:
					throw new ArgumentOutOfRangeException("priority");
			}
		}
	}
}
