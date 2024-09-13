using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Foundation.Helpers.Extensions
{
	public static class EnumerableExtensions
	{
		public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> items, int page, int pageSize)
		{
			items = items.Skip((page - 1)*pageSize).Take(pageSize);
			return items;
		}

		public static IList<T> ApplyPaging<T>(this IList<T> items, int page, int pageSize)
		{
			items = items.Skip((page - 1)*pageSize).Take(pageSize).ToList();
			return items;
		}

		public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int qtyPerBatch)
		{
		    var enumerable = items as T[] ?? items.ToArray();
		    var itemcount = enumerable.Count();
            if (itemcount <= qtyPerBatch)
		    {
                var retItems = new List<IEnumerable<T>>() ;
                retItems.Add(enumerable);
		        return retItems;
		    }
            var iterations = (double)(itemcount / qtyPerBatch);
			var roundedIterations = (int)Math.Ceiling(iterations);
			var batchItems = new List<IEnumerable<T>>();
			for (int i = 0; i < roundedIterations; i++)
			{
				batchItems.Add(enumerable.Skip(i * qtyPerBatch).Take(qtyPerBatch).ToArray());
			}
			return batchItems;
		}

		public static IEnumerable<T> ApplyPaging<T>(this IEnumerable<T> items, int page, int pageSize)
		{
			items = items.Skip((page - 1)*pageSize).Take(pageSize).ToList();
			return items;
		}

		public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> items, int page, int pageSize, out int numItems)
		{
			numItems = items.Count();
			items = items.Skip((page - 1)*pageSize).Take(pageSize);
			return items;
		}

		public static bool HasAny<T>(this IEnumerable<T> enumerable)
		{
			return enumerable != null && enumerable.Any();
		}
	}
}