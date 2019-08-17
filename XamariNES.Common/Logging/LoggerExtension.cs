using System.Text;
using NLog;

namespace XamariNES.Common.Logging
{
    public static class LoggerExtension
    {
        /// <summary>
        ///     Takes a Byte Array and logs it in a hex-editor like format for easy reading
        /// </summary>
        /// <param name="l"></param>
        /// <param name="arrayToLog"></param>
        public static void InfoHex(this Logger l, byte[] arrayToLog)
        {
            var output = new StringBuilder();

            //Print Header
            output.AppendLine(new string('-', 73));
            output.AppendLine($"{arrayToLog.Length} bytes, 0x0000 -> 0x{arrayToLog.GetUpperBound(0):X4}");
            output.AppendLine(new string('-', 73));
            output.Append("      ");
            for (var i = 0; i < 0x10; i++)
            {
                output.Append($" {i:X2}");
            }
            output.AppendLine();
            var hexString = new StringBuilder(47);
            var literalString = new StringBuilder(15);
            
            //Print Hex Values
            for (var i = 0; i < arrayToLog.Length; i++)
            {
                hexString.Append($" {arrayToLog[i]:X2}");
                literalString.Append(arrayToLog[i] < 32 ? ' ' : (char)arrayToLog[i]);
                
                //New Memory Page
                if ((i | 0x0F) == i)
                {
                    output.AppendLine($"{(i & ~0xF):X4} [{hexString} ] {literalString}");
                    hexString.Clear();
                    literalString.Clear();
                }
            }

            //Flush any data remaining in the buffer
            if (hexString.Length > 0)
            {
                output.AppendLine($"{(arrayToLog.Length & ~0xF):X4} [{hexString.ToString().PadRight(48)} ] {literalString}");
                hexString.Clear();
                literalString.Clear();
            }

            l.Info($"\r\n{output}");
        }
    }
}
