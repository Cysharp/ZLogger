using System.Text.RegularExpressions;

namespace ZLogger.Providers;

internal partial class RollingFileStream : Stream
{
#if NET7_0_OR_GREATER    
     [GeneratedRegex("(\\d)+$", RegexOptions.Compiled)]
     private static partial Regex NumberRegexCompiled();
     static readonly Regex NumberRegex = NumberRegexCompiled();
#else    
    static readonly Regex NumberRegex = new("(\\d)+$", RegexOptions.Compiled);
#endif
    

    readonly Func<DateTimeOffset, int, string> fileNameSelector;
    readonly RollingInterval rollInterval;
    readonly long rollSizeInBytes;
    readonly TimeProvider? timeProvider;
    readonly bool fileShared;

#pragma warning disable CS8618

    bool disposed;
    int writtenLength;
    string fileName;
    Stream? innerStream;
    DateTimeOffset? nextCheckpoint;

    public RollingFileStream(Func<DateTimeOffset, int, string> fileNameSelector, RollingInterval rollInterval, int rollSizeKB, TimeProvider? timeProvider, bool fileShared)
    {
        this.fileNameSelector = fileNameSelector;
        this.rollInterval = rollInterval;
        this.rollSizeInBytes = (long)rollSizeKB * 1024;
        this.timeProvider = timeProvider;
        this.fileShared = fileShared;

        ValidateFileNameSelector();
        TryChangeNewRollingFile();
    }

#pragma warning restore CS8618

    void ValidateFileNameSelector()
    {
        var now = timeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
        var fileName1 = Path.GetFileNameWithoutExtension(fileNameSelector(now, 0));
        var fileName2 = Path.GetFileNameWithoutExtension(fileNameSelector(now, 1));

        if (!NumberRegex.IsMatch(fileName1) || !NumberRegex.IsMatch(fileName2))
        {
            throw new ArgumentException("fileNameSelector is invalid format, must be int(sequence no) is last.");
        }

        var seqStr1 = NumberRegex.Match(fileName1).Groups[0].Value;
        var seqStr2 = NumberRegex.Match(fileName2).Groups[0].Value;

        if (!int.TryParse(seqStr1, out var seq1) || !int.TryParse(seqStr2, out var seq2))
        {
            throw new ArgumentException("fileNameSelector is invalid format, must be int(sequence no) is last.");
        }

        if (seq1 == seq2)
        {
            throw new ArgumentException("fileNameSelector is invalid format, must be int(sequence no) is incremental.");
        }
    }

    void TryChangeNewRollingFile()
    {
        var now = timeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
        var currentCheckpoint = GetCurrentCheckpoint(now);

        // needs to create next file
        if (innerStream == null || currentCheckpoint >= nextCheckpoint || writtenLength >= rollSizeInBytes)
        {
            var sequenceNo = 0;
            if (innerStream != null && currentCheckpoint < nextCheckpoint)
            {
                sequenceNo = ExtractCurrentSequence(fileName) + 1;
            }

            string? fn = null;
            while (true)
            {
                try
                {
                    var newFn = fileNameSelector(now, sequenceNo);
                    if (fn == newFn)
                    {
                        throw new InvalidOperationException("fileNameSelector indicate same filname");
                    }
                    fn = newFn;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("fileNameSelector convert failed", ex);
                }

                var fi = new FileInfo(fn);
                if (fi.Exists)
                {
                    if (fi.Length >= rollSizeInBytes)
                    {
                        sequenceNo++;
                        continue;
                    }
                }
                break;
            }

            if (disposed) return;
            try
            {
                if (innerStream != null)
                {
                    innerStream.Flush();
                    innerStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't dispose fileStream", ex);
            }

            try
            {
                fileName = fn!;
                nextCheckpoint = GetNextCheckpoint(now);
                if (File.Exists(fileName))
                {
                    writtenLength = (int)new FileInfo(fileName).Length;
                }
                else
                {
                    writtenLength = 0;
                }

                var di = new FileInfo(fileName).Directory;
                if (!di!.Exists)
                {
                    di.Create();
                }
                // useAsync:false, use sync(in thread) processor, don't use FileStream buffer(use buffer size = 1).
                innerStream = fileShared
                    ? new SharedFileStream(fileName)
                    : new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't create FileStream", ex);
            }
        }
    }
    
    DateTimeOffset? GetCurrentCheckpoint(DateTimeOffset instant) => rollInterval switch
    {
        RollingInterval.Infinite => null,
        RollingInterval.Year => new DateTimeOffset(instant.Year, 1, 1, 0, 0, 0, instant.Offset),
        RollingInterval.Month => new DateTimeOffset(instant.Year, instant.Month, 1, 0, 0, 0, instant.Offset),
        RollingInterval.Day => new DateTimeOffset(instant.Year, instant.Month, instant.Day, 0, 0, 0, instant.Offset),
        RollingInterval.Hour => new DateTimeOffset(instant.Year, instant.Month, instant.Day, instant.Hour, 0, 0, instant.Offset),
        RollingInterval.Minute => new DateTimeOffset(instant.Year, instant.Month, instant.Day, instant.Hour, instant.Minute, 0, instant.Offset),
        _ => throw new ArgumentOutOfRangeException()
    };

    DateTimeOffset? GetNextCheckpoint(DateTimeOffset instant)
    {
        var current = GetCurrentCheckpoint(instant);
        if (current == null)
            return null;

        return rollInterval switch
        {
            RollingInterval.Year => current.Value.AddYears(1),
            RollingInterval.Month => current.Value.AddMonths(1),
            RollingInterval.Day => current.Value.AddDays(1),
            RollingInterval.Hour => current.Value.AddHours(1),
            RollingInterval.Minute => current.Value.AddMinutes(1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static int ExtractCurrentSequence(string fileName)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);

        var sequenceString = NumberRegex.Match(fileName).Groups[0].Value;
        return int.TryParse(sequenceString, out var seq) ? seq : 0;
    }

    #region override

    public override bool CanRead => innerStream!.CanRead;

    public override bool CanSeek => innerStream!.CanSeek;

    public override bool CanWrite => innerStream!.CanWrite;

    public override long Length => innerStream!.Length;

    public override long Position { get => innerStream!.Position; set => innerStream!.Position = value; }

    public override void Flush()
    {
        innerStream!.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return innerStream!.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return innerStream!.Seek(offset, origin);
    }
    public override void SetLength(long value)
    {
        innerStream!.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        TryChangeNewRollingFile();
        innerStream!.Write(buffer, offset, count);
        writtenLength += count;
    }

    protected override void Dispose(bool disposing)
    {
        innerStream!.Dispose();
        disposed = true;
    }
    #endregion
}