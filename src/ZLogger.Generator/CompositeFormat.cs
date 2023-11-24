#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text
{
    /// <summary>Represents a parsed composite format string.</summary>
    [DebuggerDisplay("{Format}")]
    public sealed class CompositeFormat
    {
        /// <summary>The parsed segments that make up the composite format string.</summary>
        /// <remarks>
        /// Every segment represents either a literal or a format hole, based on whether Literal
        /// is non-null or ArgIndex is non-negative.
        /// </remarks>
        internal readonly (string? Literal, int ArgIndex, int Alignment, string? Format)[] _segments;
        /// <summary>The sum of the lengths of all of the literals in <see cref="_segments"/>.</summary>
        internal readonly int _literalLength;
        /// <summary>The number of segments in <see cref="_segments"/> that represent format holes.</summary>
        internal readonly int _formattedCount;
        /// <summary>The number of args required to satisfy the format holes.</summary>
        /// <remarks>This is equal to one more than the largest index required by any format hole.</remarks>
        internal readonly int _argsRequired;

        /// <summary>Initializes the instance.</summary>
        /// <param name="format">The composite format string that was parsed.</param>
        /// <param name="segments">The parsed segments.</param>
        private CompositeFormat(string format, (string? Literal, int ArgIndex, int Alignment, string? Format)[] segments)
        {
            // Store the format.
            Debug.Assert(format is not null);
            Format = format!;

            // Store the segments.
            Debug.Assert(segments is not null);
            _segments = segments!;

            // Compute derivative information from the segments.
            int literalLength = 0, formattedCount = 0, argsRequired = 0;
            foreach ((string? Literal, int ArgIndex, int Alignment, string? Format) segment in segments!)
            {
                Debug.Assert((segment.Literal is not null) ^ (segment.ArgIndex >= 0), "The segment should represent a literal or a format hole, but not both.");

                if (segment.Literal is string literal)
                {
                    literalLength += literal.Length; // no concern about overflow as these were parsed out of a single string
                }
                else if (segment.ArgIndex >= 0)
                {
                    formattedCount++;
                    argsRequired = Math.Max(argsRequired, segment.ArgIndex + 1);
                }
            }

            // Store the derivative information.
            Debug.Assert(literalLength >= 0);
            Debug.Assert(formattedCount >= 0);
            Debug.Assert(formattedCount == 0 || argsRequired > 0);
            _literalLength = literalLength;
            _formattedCount = formattedCount;
            _argsRequired = argsRequired;
        }

        /// <summary>Parse the composite format string <paramref name="format"/>.</summary>
        /// <param name="format">The string to parse.</param>
        /// <returns>The parsed <see cref="CompositeFormat"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null.</exception>
        /// <exception cref="FormatException">A format item in <paramref name="format"/> is invalid.</exception>
        public static CompositeFormat Parse(string format)
        {
            // ArgumentNullException.ThrowIfNull(format);

            var segments = new List<(string? Literal, int ArgIndex, int Alignment, string? Format)>();
            int failureOffset = default;
            ExceptionResource failureReason = default;
            if (!TryParseLiterals(format.AsSpan(), segments, ref failureOffset, ref failureReason))
            {
                throw new FormatException((failureOffset, failureReason).ToString());
            }

            return new CompositeFormat(format, segments.ToArray());
        }

        /// <summary>Gets the original composite format string used to create this <see cref="CompositeFormat"/> instance.</summary>
        public string Format { get; }

        /// <summary>Gets the minimum number of arguments that must be passed to a formatting operation using this <see cref="CompositeFormat"/>.</summary>
        /// <remarks>It's permissible to supply more arguments than this value, but it's an error to pass fewer.</remarks>
        public int MinimumArgumentCount => _argsRequired;

        /// <summary>Throws an exception if the specified number of arguments is fewer than the number required.</summary>
        /// <param name="numArgs">The number of arguments provided by the caller.</param>
        /// <exception cref="FormatException">An insufficient number of arguments were provided.</exception>
        internal void ValidateNumberOfArgs(int numArgs)
        {
            if (numArgs < _argsRequired)
            {
                throw new FormatException("SR.Format_IndexOutOfRange");
            }
        }

        /// <summary>Parse the composite format string into segments.</summary>
        /// <param name="format">The format string.</param>
        /// <param name="segments">The list into which to store the segments.</param>
        /// <param name="failureOffset">The offset at which a parsing error occured if <see langword="false"/> is returned.</param>
        /// <param name="failureReason">The reason for a parsing failure if <see langword="false"/> is returned.</param>
        /// <returns>true if the format string can be parsed successfully; otherwise, false.</returns>
        private static bool TryParseLiterals(ReadOnlySpan<char> format, List<(string? Literal, int ArgIndex, int Alignment, string? Format)> segments, ref int failureOffset, ref ExceptionResource failureReason)
        {
            // This parsing logic is copied from string.Format.  It's the same code modified to not format
            // as part of parsing and instead store the parsed literals and argument specifiers (alignment
            // and format) for later use.

            // Rather than parsing directly into the segments list, literals are parsed into a reusable builder.
            // Due to the nature of the parsing logic copied from string.Format, and our desire not to veer from
            // it significantly in order to maintain compatibility and accidental regression, multiple literals
            // next to each other might be parsed separately due to braces in between them.  This builder then
            // allows us to merge those segments back together easily prior to their being appended to the list.
            var vsb = new ValueStringBuilder(stackalloc char[256]);

            // Repeatedly find the next hole and process it.
            int pos = 0;
            char ch;
            while (true)
            {
                // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
                // Along the way we need to also unescape escaped closing braces.
                while (true)
                {
                    // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                    ReadOnlySpan<char> remainder = format.Slice(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        vsb.Append(remainder);
                        segments.Add((vsb.ToString(), -1, 0, null));
                        return true;
                    }

                    // Append the text until the brace.
                    vsb.Append(remainder.Slice(0, countUntilNextBrace));
                    pos += countUntilNextBrace;

                    // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                    // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                    char brace = format[pos];
                    if (!TryMoveNext(format, ref pos, out ch))
                    {
                        goto FailureUnclosedFormatItem;
                    }
                    if (brace == ch)
                    {
                        vsb.Append(ch);
                        pos++;
                        continue;
                    }

                    // This wasn't an escape, so it must be an opening brace.
                    if (brace != '{')
                    {
                        goto FailureUnexpectedClosingBrace;
                    }

                    // Proceed to parse the hole.
                    segments.Add((vsb.ToString(), -1, 0, null));
                    vsb.Length = 0;
                    break;
                }

                // We're now positioned just after the opening brace of an argument hole, which consists of
                // an opening brace, an index, an optional width preceded by a comma, and an optional format
                // preceded by a colon, with arbitrary amounts of spaces throughout.
                int width = 0;
                string? itemFormat = null; // used if itemFormat is null

                // First up is the index parameter, which is of the form:
                //     at least on digit
                //     optional any number of spaces
                // We've already read the first digit into ch.
                Debug.Assert(format[pos - 1] == '{');
                Debug.Assert(ch != '{');
                int index = ch - '0';
                if ((uint)index >= 10u)
                {
                    goto FailureExpectedAsciiDigit;
                }

                // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
                // proceed to finish parsing the full hole format.
                if (!TryMoveNext(format, ref pos, out ch))
                {
                    goto FailureUnclosedFormatItem;
                }
                if (ch != '}')
                {
                    // Continue consuming optional additional digits.
                    while (IsAsciiDigit(ch))
                    {
                        index = index * 10 + ch - '0';
                        if (!TryMoveNext(format, ref pos, out ch))
                        {
                            goto FailureUnclosedFormatItem;
                        }
                    }

                    // Consume optional whitespace.
                    while (ch == ' ')
                    {
                        if (!TryMoveNext(format, ref pos, out ch))
                        {
                            goto FailureUnclosedFormatItem;
                        }
                    }

                    // Parse the optional alignment, which is of the form:
                    //     comma
                    //     optional any number of spaces
                    //     optional -
                    //     at least one digit
                    //     optional any number of spaces
                    if (ch == ',')
                    {
                        // Consume optional whitespace.
                        do
                        {
                            if (!TryMoveNext(format, ref pos, out ch))
                            {
                                goto FailureUnclosedFormatItem;
                            }
                        }
                        while (ch == ' ');

                        // Consume an optional minus sign indicating left alignment.
                        int leftJustify = 1;
                        if (ch == '-')
                        {
                            leftJustify = -1;
                            if (!TryMoveNext(format, ref pos, out ch))
                            {
                                goto FailureUnclosedFormatItem;
                            }
                        }

                        // Parse alignment digits. The read character must be a digit.
                        width = ch - '0';
                        if ((uint)width >= 10u)
                        {
                            goto FailureExpectedAsciiDigit;
                        }
                        if (!TryMoveNext(format, ref pos, out ch))
                        {
                            goto FailureUnclosedFormatItem;
                        }
                        while (IsAsciiDigit(ch))
                        {
                            width = width * 10 + ch - '0';
                            if (!TryMoveNext(format, ref pos, out ch))
                            {
                                goto FailureUnclosedFormatItem;
                            }
                        }
                        width *= leftJustify;

                        // Consume optional whitespace
                        while (ch == ' ')
                        {
                            if (!TryMoveNext(format, ref pos, out ch))
                            {
                                goto FailureUnclosedFormatItem;
                            }
                        }
                    }

                    // The next character needs to either be a closing brace for the end of the hole,
                    // or a colon indicating the start of the format.
                    if (ch != '}')
                    {
                        if (ch != ':')
                        {
                            // Unexpected character
                            goto FailureUnclosedFormatItem;
                        }

                        // Search for the closing brace; everything in between is the format,
                        // but opening braces aren't allowed.
                        int startingPos = pos;
                        while (true)
                        {
                            if (!TryMoveNext(format, ref pos, out ch))
                            {
                                goto FailureUnclosedFormatItem;
                            }

                            if (ch == '}')
                            {
                                // Argument hole closed
                                break;
                            }

                            if (ch == '{')
                            {
                                // Braces inside the argument hole are not supported
                                goto FailureUnclosedFormatItem;
                            }
                        }

                        startingPos++;
                        itemFormat = format.Slice(startingPos, pos - startingPos).ToString();
                    }
                }

                Debug.Assert(format[pos] == '}');
                pos++;

                segments.Add((null, index, width, itemFormat));

                // Continue parsing the rest of the format string.
            }

        FailureUnexpectedClosingBrace:
            failureReason = ExceptionResource.Format_UnexpectedClosingBrace;
            failureOffset = pos;
            return false;

        FailureUnclosedFormatItem:
            failureReason = ExceptionResource.Format_UnclosedFormatItem;
            failureOffset = pos;
            return false;

        FailureExpectedAsciiDigit:
            failureReason = ExceptionResource.Format_ExpectedAsciiDigit;
            failureOffset = pos;
            return false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool TryMoveNext(ReadOnlySpan<char> format, ref int pos, out char nextChar)
            {
                pos++;
                if ((uint)pos >= (uint)format.Length)
                {
                    nextChar = '\0';
                    return false;
                }

                nextChar = format[pos];
                return true;
            }
        }

        // char shims

        static bool IsAsciiDigit(char c) => IsBetween(c, '0', '9');

        static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
          (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);

        internal enum ExceptionResource
        {
            Format_UnexpectedClosingBrace,
            Format_UnclosedFormatItem,
            Format_ExpectedAsciiDigit,
        }
    }
}
 
namespace System.Text
{
    internal ref partial struct ValueStringBuilder
    {
        private char[]? _arrayToReturnToPool;
        private Span<char> _chars;
        private int _pos;

        public ValueStringBuilder(Span<char> initialBuffer)
        {
            _arrayToReturnToPool = null;
            _chars = initialBuffer;
            _pos = 0;
        }

        public ValueStringBuilder(int initialCapacity)
        {
            _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
            _chars = _arrayToReturnToPool;
            _pos = 0;
        }

        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _chars.Length);
                _pos = value;
            }
        }

        public int Capacity => _chars.Length;

        public void EnsureCapacity(int capacity)
        {
            // This is not expected to be called this with negative capacity
            Debug.Assert(capacity >= 0);

            // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
            if ((uint)capacity > (uint)_chars.Length)
                Grow(capacity - _pos);
        }

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// Does not ensure there is a null char after <see cref="Length"/>
        /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
        /// the explicit method call, and write eg "fixed (char* c = builder)"
        /// </summary>
        public ref char GetPinnableReference()
        {
            return ref MemoryMarshal.GetReference(_chars);
        }

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ref char GetPinnableReference(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                _chars[Length] = '\0';
            }
            return ref MemoryMarshal.GetReference(_chars);
        }

        public ref char this[int index]
        {
            get
            {
                Debug.Assert(index < _pos);
                return ref _chars[index];
            }
        }

        public override string ToString()
        {
            string s = _chars.Slice(0, _pos).ToString();
            Dispose();
            return s;
        }

        /// <summary>Returns the underlying storage of the builder.</summary>
        public Span<char> RawChars => _chars;

        /// <summary>
        /// Returns a span around the contents of the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ReadOnlySpan<char> AsSpan(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                _chars[Length] = '\0';
            }
            return _chars.Slice(0, _pos);
        }

        public ReadOnlySpan<char> AsSpan() => _chars.Slice(0, _pos);
        public ReadOnlySpan<char> AsSpan(int start) => _chars.Slice(start, _pos - start);
        public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

        public bool TryCopyTo(Span<char> destination, out int charsWritten)
        {
            if (_chars.Slice(0, _pos).TryCopyTo(destination))
            {
                charsWritten = _pos;
                Dispose();
                return true;
            }
            else
            {
                charsWritten = 0;
                Dispose();
                return false;
            }
        }

        public void Insert(int index, char value, int count)
        {
            if (_pos > _chars.Length - count)
            {
                Grow(count);
            }

            int remaining = _pos - index;
            _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
            _chars.Slice(index, count).Fill(value);
            _pos += count;
        }

        public void Insert(int index, string? s)
        {
            if (s == null)
            {
                return;
            }

            int count = s.Length;

            if (_pos > (_chars.Length - count))
            {
                Grow(count);
            }

            int remaining = _pos - index;
            _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
            s
#if !NETCOREAPP
                .AsSpan()
#endif
                .CopyTo(_chars.Slice(index));
            _pos += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            int pos = _pos;
            Span<char> chars = _chars;
            if ((uint)pos < (uint)chars.Length)
            {
                chars[pos] = c;
                _pos = pos + 1;
            }
            else
            {
                GrowAndAppend(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string? s)
        {
            if (s == null)
            {
                return;
            }

            int pos = _pos;
            if (s.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            {
                _chars[pos] = s[0];
                _pos = pos + 1;
            }
            else
            {
                AppendSlow(s);
            }
        }

        private void AppendSlow(string s)
        {
            int pos = _pos;
            if (pos > _chars.Length - s.Length)
            {
                Grow(s.Length);
            }

            s
#if !NETCOREAPP
                .AsSpan()
#endif
                .CopyTo(_chars.Slice(pos));
            _pos += s.Length;
        }

        public void Append(char c, int count)
        {
            if (_pos > _chars.Length - count)
            {
                Grow(count);
            }

            Span<char> dst = _chars.Slice(_pos, count);
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = c;
            }
            _pos += count;
        }

        public unsafe void Append(char* value, int length)
        {
            int pos = _pos;
            if (pos > _chars.Length - length)
            {
                Grow(length);
            }

            Span<char> dst = _chars.Slice(_pos, length);
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = *value++;
            }
            _pos += length;
        }

        public void Append(scoped ReadOnlySpan<char> value)
        {
            int pos = _pos;
            if (pos > _chars.Length - value.Length)
            {
                Grow(value.Length);
            }

            value.CopyTo(_chars.Slice(_pos));
            _pos += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<char> AppendSpan(int length)
        {
            int origPos = _pos;
            if (origPos > _chars.Length - length)
            {
                Grow(length);
            }

            _pos = origPos + length;
            return _chars.Slice(origPos, length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowAndAppend(char c)
        {
            Grow(1);
            Append(c);
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="_pos"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

            // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
            // to double the size if possible, bounding the doubling to not go beyond the max array length.
            int newCapacity = (int)Math.Max(
                (uint)(_pos + additionalCapacityBeyondPos),
                Math.Min((uint)_chars.Length * 2, ArrayMaxLength));

            // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
            // This could also go negative if the actual required length wraps around.
            char[] poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

            _chars.Slice(0, _pos).CopyTo(poolArray);

            char[]? toReturn = _arrayToReturnToPool;
            _chars = _arrayToReturnToPool = poolArray;
            if (toReturn != null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            char[]? toReturn = _arrayToReturnToPool;
            this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
            if (toReturn != null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }
    }
}