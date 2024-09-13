using System;

namespace DEWAXP.Foundation.Helpers
{
	public static class Sizer
	{
		public static string FullSize(long size)
		{
			// Via http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
			var order = 0;
			var bytes = size;
			string[] sizes = {"B", "KB", "MB", "GB"};
			while (bytes >= 1024 && order + 1 < sizes.Length)
			{
				order++;
				bytes = bytes/1024;
			}

			// Adjust the format string to your preferences. For example "{0:0.#}{1}" would
			// show a single decimal place, and no space.
			var result = String.Format("{0:0.##} {1}", bytes, sizes[order]);
			return result;
		}
	}
}