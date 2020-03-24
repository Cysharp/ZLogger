using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ZLog
{
    public class ZLogOptions
    {
        public Action<IBufferWriter<byte>, LogLevel, EventId, string>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogLevel, EventId, string>? PostfixFormatter { get; set; }
    }
}
