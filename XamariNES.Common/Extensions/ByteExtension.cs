using System.Runtime.CompilerServices;

namespace XamariNES.Common.Extensions
{
    /// <summary>
    ///     Extension Methods for typical functions performed on byte values in XamariNES
    /// </summary>
    public static class ByteExtension
    {
        /// <summary>
        ///     Helper Method just to see if Bit 7 is set denoting a negative value of a signed byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this byte b) => (b & (1 << 7)) != 0;

        /// <summary>
        ///     Returns if the specified bit was set
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitNumber"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(this byte b, int bitNumber) => (b & (1 << bitNumber)) != 0;

        /// <summary>
        ///     Returns if the specified flag is set in the byte
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFlagSet(this byte b, byte bitMask) => (b & bitMask) != 0;

        /// <summary>
        ///     Sets the specified bitmask to for the specified bits
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SetFlag(this byte b, byte bitMask) => (byte)(b | bitMask);

        /// <summary>
        ///     Sets the specified bitmask to 0 for the specified bits
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RemoveFlag(this byte b, byte bitMask) => (byte) (b & ~bitMask);
    }
}