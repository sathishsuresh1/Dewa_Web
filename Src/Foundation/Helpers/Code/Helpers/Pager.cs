// <copyright file="Pager.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Pager" />
    /// </summary>
    public static class Pager
    {
        /// <summary>
        /// The CalculateTotalPages
        /// </summary>
        /// <param name="numItems">The numItems<see cref="int"/></param>
        /// <param name="pageSize">The pageSize<see cref="int"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int CalculateTotalPages(int numItems, int pageSize)
        {
            return numItems % pageSize == 0 ? numItems / pageSize : numItems / pageSize + 1;
        }

        /// <summary>
        /// The GetPaginationRange
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <param name="totalPages">The totalPages<see cref="int"/></param>
        /// <returns>The <see cref="IEnumerable{int}"/></returns>
        public static IEnumerable<int> GetPaginationRange(int currentPage, int totalPages)
        {
            const int desiredCount = 5;
            List<int> returnint = new List<int>();
            int start = currentPage - 1;
            int projectedEnd = start + desiredCount;
            if (projectedEnd > totalPages)
            {
                start = start - (projectedEnd - totalPages);
                projectedEnd = totalPages;
            }
            int p = start;
            while (p++ < projectedEnd)
            {
                if (p > 0)
                {
                    returnint.Add(p);
                }
            }
            return returnint;
        }
    }
}
