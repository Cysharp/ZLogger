#if NETSTANDARD2_0
using System.Text;

namespace ZLogger;

internal static partial class Shims
{
    public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, ReadOnlySpan<byte> bytes)
    {
        unsafe
        {
            fixed (char* charsPtr = &chars[0])
            fixed (byte* bytesPtr = &bytes[0])
            {
                return encoding.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
            }
        }
    }

    public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        unsafe
        {
            fixed (byte* bytesPtr = &bytes[0])
            {
                return encoding.GetString(bytesPtr, bytes.Length);
            }
        }
    }

    public static bool StartsWith(this string str, char value)
    {
        return str.Length != 0 && str[0] == value;
    }
}

#endif