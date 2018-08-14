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
        public static async Task<Task<TResult>[]> PartitionerExecuteAsync<TItem, TResult>(IEnumerable<TItem> items,
            Func<TItem, int, Task<TResult>> func, int? partitionCount = null)
        {
            var ps = GetPartitions(items, partitionCount);
            return await PartitionerExecuteAsync(items, func, ps);
        }


        private static async Task<Task<TResult>[]> PartitionerExecuteAsync<TItem, TResult>(IEnumerable<TItem> items,
            Func<TItem, int, Task<TResult>> func, IList<IEnumerator<TItem>> ps)
        {
            var tasks = new Task<Task<TResult>[]>[ps.Count];
            var psItemCount = items.Count() / ps.Count;
            //Console.WriteLine($"GetPartitions Count {ps.Count}");
            for (var k = 0; k < ps.Count; k++)
            {
                var psfies = ps[k];
                tasks[k] = ExecuteAsync(func, psfies, k, psItemCount);
            }
            var s = (await Task.WhenAll(tasks));
            return s.SelectMany(x => x).ToArray();
        }

        private static async Task<Task<TResult>[]> ExecuteAsync<TItem, TResult>(Func<TItem, int, Task<TResult>> func,
            IEnumerator<TItem> psfies, int index, int? psItemCount)
        {
            var tasks = psItemCount.HasValue ? new List<Task<TResult>>(psItemCount.Value) : new List<Task<TResult>>();
            using (psfies)
            {
                while (psfies.MoveNext())
                {
                    var f = psfies.Current;
                    var t = func.Invoke(f, index);
                    tasks.Add(t);
                    try
                    {
                        await t;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return tasks.ToArray();
        }


        public static IList<IEnumerator<TItem>> GetPartitions<TItem>(IEnumerable<TItem> items,
            int? partitionCount = null)
        {
            var ps = Partitioner.Create(items).GetPartitions(partitionCount ?? System.Environment.ProcessorCount);
            return ps;
        }

    }
}