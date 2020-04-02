using System;
using System.Collections.Generic;
using System.Text;

namespace ZLogger
{
    public interface IAsyncLogProcessor : IAsyncDisposable
    {
        void Post(IZLoggerEntry log);
    }
}
