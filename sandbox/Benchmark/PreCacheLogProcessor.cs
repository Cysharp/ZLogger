using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZLogger;

namespace Benchmark
{
    internal class PreCacheLogProcessor : IAsyncLogProcessor
    {
        List<IZLoggerEntry> entries = new();

        public void Post(IZLoggerEntry log)
        {
            lock (entries)
            {
                entries.Add(log);
            }
        }

        public ValueTask DisposeAsync()
        {
            lock (entries)
            {
                foreach (var item in CollectionsMarshal.AsSpan(entries))
                {
                    item.Return();
                }
                entries.Clear();
            }
            return default;
        }
    }
}
