using System;
using System.Globalization;

namespace Memoria.BaseOperations
{
    /// <summary>
    /// Class to convert and encode binary offsets. Can only handle unsigned values.
    /// </summary>
    static class OffsetManager
    {
        public static string Swap_Bytes_ToString(byte[] b, int startOffset, ushort bytesToSwap)
        {
            if (startOffset > b.Length - 1)
                throw new Exception("Start offset is longer than buffer length.");
            if (bytesToSwap <= 0)
                throw new Exception("Bytes to swap must be more than 0.");
            string s = "";
            string hexValue = "";
            for (int i = startOffset; i > (startOffset - bytesToSwap); i--)
            {
                s = b[i].ToString("x");
                if (s.Length < 2)
                    s = "0" + s;
                hexValue += s;
            }
            while (hexValue.StartsWith("0"))
            {
                hexValue = hexValue.TrimStart('0');
            }
            if (hexValue == "")
                return "0";
            return hexValue;
        }
        public static uint Swap_Bytes(byte[] b, int startOffset, ushort bytesToSwap)
        {
            return UInt32.Parse(Swap_Bytes_ToString(b, startOffset, bytesToSwap), NumberStyles.HexNumber);
        }
        /// <summary>
        /// Makes a string value into a hex value, modded % 2 to add zeros on uneven byte values.
        /// </summary>
        /// <param name="s">The value to convert.</param>
        /// <returns>The value in hex.</returns>
        public static string Make_Into_Offset(string s)
        {

            s = Convert.ToInt32(s).ToString("x");
            if (s.Length % 2 != 0)
                s = "0" + s;
            return s;
        }
        /// <summary>
        /// Makes an integer value into a memory/pointer value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="bytes">Desired offset byte length. Include all 00h bytes in the pointer.</param>
        /// <returns>The array containig each byte in the pointer.</returns>
        public static byte[] Flap_Values(uint value, ushort bytes)
        {
            string s = Make_Into_Offset(value.ToString()).ToUpper();
            while (s.Length < bytes * 2)
            {
                s = "0" + s;
            }
            string o = s;
            int stringIndex;
            byte[] b = new byte[s.Length / 2];
            for (int i = 0; i < b.Length; i++)
            {
                stringIndex = (b.Length * 2 - 1) - (i * 2) - 1;
                o = s.Substring(stringIndex);
                if (o.Length > 2)
                    o = o.Remove(2);
                b[i] = Byte.Parse(o, NumberStyles.HexNumber);
            }
            return b;
        }
    }
}
