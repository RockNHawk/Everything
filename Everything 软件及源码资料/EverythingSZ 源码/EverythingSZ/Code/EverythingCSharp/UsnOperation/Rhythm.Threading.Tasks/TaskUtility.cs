using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rhythm.Threading.Tasks
{
    public class TaskUtility
    {
        public static List<TResult> PartitionerExecuteSync<TItem, TResult>(IEnumerable<TItem> items,
         Func<TItem, int, TResult> func, int? partitionCount = null)
        {
            var ps = GetPartitions(items, partitionCount);
            var psResults = PartitionerExecuteSync(items, func, ps);
            var count = 0;
            for (int i = 0; i < psResults.Length; i++)
            {
                var rs = psResults[i];
                count += rs == null ? 0 : rs.Length;
            }
            var results = new List<TResult>(count);
            for (int i = 0; i < psResults.Length; i++)
            {
                var rs = psResults[i];
                if (rs != null)
                {
                    results.AddRange(rs);
                }
            }
            return results;
        }

        public static TResult[][] PartitionerExecuteSync<TItem, TResult>(IEnumerable<TItem> items,
            Func<TItem, int, TResult> func, IList<IEnumerator<TItem>> ps)
        {
            var results = new TResult[ps.Count][];
            var psItemCount = items.Count() / ps.Count;
            //Console.WriteLine($"GetPartitions Count {ps.Count}");
            Parallel.For(0, ps.Count, k =>
            {
                var psfies = ps[k];
                results[k] = ExecuteSync(func, psfies, k, psItemCount);
            });
            //for (var k = 0; k < ps.Count; k++)
            //{
            //    var psfies = ps[k];
            //    results[k] = ExecuteSync(func, psfies, k, psItemCount);
            //}
            return results;
        }

        private static TResult[] ExecuteSync<TItem, TResult>(Func<TItem, int, TResult> func,
            IEnumerator<TItem> psfies, int index, int? psItemCount)
        {
            var results = psItemCount.HasValue ? new List<TResult>(psItemCount.Value) : new List<TResult>();
            using (psfies)
            {
                while (psfies.MoveNext())
                {
                    var f = psfies.Current;
                    // TODO:Exception handling
                    var t = func.Invoke(f, index);
                    results.Add(t);
                }
            }
            return results.ToArray();
        }


        public static IList<IEnumerator<TItem>> GetPartitions<TItem>(IEnumerable<TItem> items,
      int? partitionCount = null)
        {
            var ps = Partitioner.Create(items).GetPartitions(partitionCount ?? System.Environment.ProcessorCount);
            return ps;
        }

    }
}