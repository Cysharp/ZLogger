namespace ZLogger.Providers;

internal class SharedFileStream : Stream
{
    const int MutexWaitTimeout = 10000;
    
    readonly FileStream innerStream;
    readonly Mutex mutex;

    public SharedFileStream(string filePath)
    {
        // useAsync:false, use sync(in thread) processor, don't use FileStream buffer(use buffer size = 1).
        innerStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);

        var mutexName = Path.GetFullPath(filePath).Replace(Path.DirectorySeparatorChar, ':') + ".zlogger";
        mutex = new Mutex(false, mutexName);
    }

    public override bool CanRead => innerStream.CanRead;
    public override bool CanSeek => innerStream.CanSeek;
    public override bool CanWrite => innerStream.CanWrite;
    public override long Length => innerStream.Length;
    public override long Position 
    { 
        get => innerStream.Position;
        set => innerStream.Position = value;
    }

    public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);
    
    public override void Flush()
    {
        using var _ = AcquireMutex();
        innerStream.Flush();
    }
    
    public override void SetLength(long value)
    {
        using var _ = AcquireMutex();
        innerStream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        using var _ = AcquireMutex();
        return innerStream.Read(buffer, offset, count);
    } 

    public override void Write(byte[] buffer, int offset, int count)
    {
        using var _ = AcquireMutex();
        
        innerStream.Seek(0, SeekOrigin.End);
        innerStream.Write(buffer, offset, count);
        innerStream.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        mutex.Dispose();
    }

    MutexAcquired AcquireMutex() => new(mutex);

    readonly struct MutexAcquired : IDisposable
    {
        readonly Mutex mutex;

        public MutexAcquired(Mutex mutex)
        {
            this.mutex = mutex;
            // AbandonedMutexException is propagated upstream
            var mutexAcquired = mutex.WaitOne(MutexWaitTimeout);
            if (!mutexAcquired)
            {
                throw new InvalidOperationException($"Shared file mutex could not be acquired within {MutexWaitTimeout} ms");
            }
        }
        
        public void Dispose()
        {
            mutex.ReleaseMutex();
        }
    }
}
