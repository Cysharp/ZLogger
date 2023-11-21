#if !NET6_0_OR_GREATER
namespace System.Numerics;

public class BitOperations
{
    public static int PopCount(uint value)
    {
        // if (Popcnt.IsSupported)
        // {
        //     return (int)Popcnt.PopCount(value);
        // }
        //
        // if (AdvSimd.Arm64.IsSupported)
        // {
        //     // PopCount works on vector so convert input value to vector first.
        //
        //     Vector64<uint> input = Vector64.CreateScalar(value);
        //     Vector64<byte> aggregated = AdvSimd.Arm64.AddAcross(AdvSimd.PopCount(input.AsByte()));
        //     return aggregated.ToScalar();
        // }
 
        return SoftwareFallback(value);
 
        static int SoftwareFallback(uint value)
        {
            const uint c1 = 0x_55555555u;
            const uint c2 = 0x_33333333u;
            const uint c3 = 0x_0F0F0F0Fu;
            const uint c4 = 0x_01010101u;
 
            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 24;
 
            return (int)value;
        }
    }    
}
#endif