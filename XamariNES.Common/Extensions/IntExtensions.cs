using System.Runtime.CompilerServices;

namespace XamariNES.Common.Extensions
{
    /// <summary>
    ///     Extension Methods for typical functions performed on int values in XamariNES
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        ///     Returns if the specified bit was set
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFlagSet(this int b, int bitMask) => (b & bitMask) != 0;
    }
}
