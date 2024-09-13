using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.Builder.Models.LocalPurchase
{
	public class Table
	{
		public virtual IEnumerable<TableRow> Rows { get; set; }
	}

	public class TableRow
	{
		public virtual int SlNo { get; set; }
		public virtual string No { get; set; }
		public virtual string Description { get; set; }
		public DateTime FloatingDate { get; set; }
		public DateTime ClosingDate { get; set; }
		public virtual string DownloadUrl { get; set; }
	}
}