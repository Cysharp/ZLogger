using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZLog
{
    public class ZLogOptions
    {
        public Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter { get; set; }
        public Action<Exception>? ErrorLogger { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
