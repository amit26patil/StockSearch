using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search.Services
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> CreateBatches<T>(this IEnumerable<T> items, int batchSize = 100)
        {
            var noOfBatches = (int)Math.Ceiling((decimal)items.Count() / batchSize);
            return Enumerable
                    .Range(0, noOfBatches)
                    .Select(a => items
                                    .Skip(a * batchSize)
                                    .Take(batchSize)
                           );
        }
    }
}
