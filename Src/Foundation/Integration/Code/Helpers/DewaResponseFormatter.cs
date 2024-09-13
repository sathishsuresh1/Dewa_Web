namespace DEWAXP.Foundation.Integration.Helpers
{
	public static class DewaResponseFormatter 
	{
		public static string Trimmer(string sValue)
		{
		    if (string.IsNullOrEmpty(sValue))
		    {
		        return string.Empty;
		    }
            return sValue.TrimStart('0'); 
		}

	}
}