using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ZLogger.Providers
{
    internal class RollingFileStream : Stream
    {
        static readonly Regex NumberRegex = new Regex("(\\d)+$", RegexOptions.Compiled);

        readonly object streamLock = new object();
        readonly Func<DateTimeOffset, DateTimeOffset> timestampPattern;
        readonly Func<DateTimeOffset, int, string> fileNameSelector;
        readonly long rollSizeInBytes;
        readonly ZLoggerOptions options;

#pragma warning disable CS8618

        bool disposed = false;
        int writtenLength;
        string fileName;
        Stream innerStream;
        DateTimeOffset currentTimestampPattern;

        public RollingFileStream(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, ZLoggerOptions options)
        {
            this.timestampPattern = timestampPattern;
            this.fileNameSelector = fileNameSelector;
            this.rollSizeInBytes = rollSizeKB * 1024;
            this.options = options;

            ValidateFileNameSelector();
            TryChangeNewRollingFile();
        }

#pragma warning restore CS8618

        void ValidateFileNameSelector()
        {
            var now = DateTimeOffset.UtcNow;
            var fileName1 = Path.GetFileNameWithoutExtension(fileNameSelector(now, 0));
            var fileName2 = Path.GetFileNameWithoutExtension(fileNameSelector(now, 1));

            if (!NumberRegex.IsMatch(fileName1) || !NumberRegex.IsMatch(fileName2))
            {
                throw new ArgumentException("fileNameSelector is invalid format, must be int(sequence no) is last.");
            }

            var seqStr1 = NumberRegex.Match(fileName1).Groups[0].Value;
            var seqStr2 = NumberRegex.Match(fileName2).Groups[0].Value;

            int seq1;
            int seq2;
            if (!int.TryParse(seqStr1, out seq1) || !int.TryParse(seqStr2, out seq2))
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
            var now = DateTimeOffset.UtcNow;
            DateTimeOffset ts;
            try
            {
                ts = timestampPattern(now);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("timestampPattern convert failed.", ex);
            }

            // needs to create next file
            if (innerStream == null || ts != currentTimestampPattern || writtenLength >= rollSizeInBytes)
            {
                int sequenceNo = 0;
                if (innerStream != null && ts == currentTimestampPattern)
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

                lock (streamLock)
                {
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
                        currentTimestampPattern = ts;
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
                        innerStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Can't create FileStream", ex);
                    }
                }
            }
        }

        static int ExtractCurrentSequence(string fileName)
        {
            int extensionDotIndex = fileName.LastIndexOf('.');

            fileName = Path.GetFileNameWithoutExtension(fileName);

            var sequenceString = NumberRegex.Match(fileName).Groups[0].Value;
            int seq;
            if (int.TryParse(sequenceString, out seq))
            {
                return seq;
            }
            else
            {
                return 0;
            }
        }

        #region override

        public override bool CanRead => innerStream.CanRead;

        public override bool CanSeek => innerStream.CanSeek;

        public override bool CanWrite => innerStream.CanWrite;

        public override long Length => innerStream.Length;

        public override long Position { get => innerStream.Position; set => innerStream.Position = value; }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            TryChangeNewRollingFile();
            innerStream.Write(buffer, offset, count);
            writtenLength += count;
        }

        protected override void Dispose(bool disposing)
        {
            lock (streamLock)
            {
                innerStream.Dispose();
                disposed = true;
            }
        }

        #endregion
    }
}
