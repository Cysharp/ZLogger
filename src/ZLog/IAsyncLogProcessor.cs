using System;
using System.Collections.Generic;
using System.Text;

namespace ZLog
{
    public interface IAsyncLogProcessor : IAsyncDisposable
    {
        void Post(IZLogEntry log);
    }
}
