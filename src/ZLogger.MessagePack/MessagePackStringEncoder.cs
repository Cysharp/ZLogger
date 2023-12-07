using System;
using System.Text;
using MessagePack;

namespace ZLogger.MessagePack;

static class MessagePackStringEncoder
{
    static readonly Encoding StringEncoding = new UTF8Encoding(false);
    
    public static byte[] Encode(string stringValue, MessagePackSerializerOptions? options = null)
    {
        var oldSpec = options?.OldSpec ?? false;
        var characterLength = stringValue.Length;
        var bufferSize = StringEncoding.GetMaxByteCount(characterLength) + 5;
        
        var useOffset = characterLength switch
        {
            <= MessagePackRange.MaxFixStringLength => 1,
            <= byte.MaxValue when !oldSpec => 2,
            <= ushort.MaxValue => 3,
            _ => 5
        };

        Span<byte> buffer = stackalloc byte[bufferSize];
        var byteCount = StringEncoding.GetBytes(stringValue, buffer[useOffset..]);
        // move body and write prefix
        if (byteCount <= MessagePackRange.MaxFixStringLength)
        {
            buffer[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
        }
        else if (byteCount <= byte.MaxValue && !oldSpec)
        {
            buffer[0] = MessagePackCode.Str8;
            buffer[1] = unchecked((byte)byteCount);
        }
        else if (byteCount <= ushort.MaxValue)
        {
            buffer[0] = MessagePackCode.Str16;
            // Write big-endian
            buffer[1] = (byte)(byteCount >> 8);
            buffer[2] = (byte)byteCount;
        }
        else
        {
            buffer[0] = MessagePackCode.Str32;
            buffer[1] = (byte)(byteCount >> 24);
            buffer[2] = (byte)(byteCount >> 16);
            buffer[3] = (byte)(byteCount >> 8);
            buffer[4] = (byte)byteCount;
        }

        return buffer[..(byteCount + useOffset)].ToArray();
    }
}