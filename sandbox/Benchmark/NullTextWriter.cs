using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmark
{
    public class NullTextWriter : TextWriter
    {
		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}

		public override void Close() { }
		public override void Flush() { }
		public override void Write(char value) { }
		public override void Write(char[] buffer) { }
		public override void Write(char[] buffer, int index, int count) { }
		public override void Write(bool value) { }
		public override void Write(int value) { }
		public override void Write(uint value) { }
		public override void Write(long value) { }
		public override void Write(ulong value) { }
		public override void Write(float value) { }
		public override void Write(double value) { }
		public override void Write(decimal value) { }
		public override void Write(string value) { }
		public override void Write(object value) { }
		public override void Write(string format, object arg0) { }
		public override void Write(string format, object arg0, object arg1) { }
		public override void Write(string format, object arg0, object arg1, object arg2) { }
		public override void Write(string format, object[] arg) { }
		public override void WriteLine() { }
		public override void WriteLine(char value) { }
		public override void WriteLine(char[] buffer) { }
		public override void WriteLine(char[] buffer, int index, int count) { }
		public override void WriteLine(bool value) { }
		public override void WriteLine(int value) { }
		public override void WriteLine(uint value) { }
		public override void WriteLine(long value) { }
		public override void WriteLine(ulong value) { }
		public override void WriteLine(float value) { }
		public override void WriteLine(double value) { }
		public override void WriteLine(decimal value) { }
		public override void WriteLine(string value) { }
		public override void WriteLine(object value) { }
		public override void WriteLine(string format, object arg0) { }
		public override void WriteLine(string format, object arg0, object arg1) { }
		public override void WriteLine(string format, object arg0, object arg1, object arg2) { }
		public override void WriteLine(string format, object[] arg) { }

		protected override void Dispose(bool disposing){}
		public override ValueTask DisposeAsync()
		{
			return default;
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		public override Task FlushAsync()
		{
			return Task.CompletedTask;
		}

		public override void Write(ReadOnlySpan<char> buffer)
		{
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override void Write(StringBuilder value)
		{
			
		}
		public override Task WriteAsync(char value)
		{
			return Task.CompletedTask;
		}

		public override Task WriteAsync(char[] buffer, int index, int count)
		{
			return Task.CompletedTask;
		}

		public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}

		public override Task WriteAsync(string value)
		{
			return Task.CompletedTask;
		}

		public override Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}

		public override void WriteLine(ReadOnlySpan<char> buffer)
		{
			
		}

		public override void WriteLine(StringBuilder value)
		{
			
		}

		public override Task WriteLineAsync()
		{
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(char value)
		{
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(char[] buffer, int index, int count)
		{
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(string value)
		{
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}
	}
}
