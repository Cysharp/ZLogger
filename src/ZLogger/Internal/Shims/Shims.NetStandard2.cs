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
    
     public static unsafe Span<T> CreateSpan<T>(ref T value, int length) where T : unmanaged
     {
         fixed(T* pointer = &value)
         {
             return new Span<T>(pointer, length);
         }
     }
     
     public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(ref T value, int length) where T : unmanaged
     {
         fixed(T* pointer = &value)
         {
             return new ReadOnlySpan<T>(pointer, length);
         }
     }
}
#endif
