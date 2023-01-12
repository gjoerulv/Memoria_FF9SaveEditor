using System;

namespace Memoria.BaseOperations
{
    /// <summary>
    /// Static class that do basic string operation.
    /// </summary>
    public static class Strings
    {

        /// <summary>
        /// Trims all the folder names away from a filename, including root.
        /// </summary>
        /// <param name="s">The string to trim.</param>
        /// <returns>The filename without the absolute path.</returns>
        public static string TrimFolders(string s)
        {
            int i = 0;
            /*while (s.Contains("\\"))
            {
                i = s.IndexOf("\\") + 1;
                s = s.Remove(0, i);
            }
            while (s.Contains("/"))
            {
                i = s.IndexOf("/") + 1;
                s = s.Remove(0, i);
            }*/
            while (s.Contains(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                i = s.IndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
                s = s.Remove(0, i);
            }
            if (System.IO.Path.AltDirectorySeparatorChar != System.IO.Path.DirectorySeparatorChar)
            {
                while (s.Contains(System.IO.Path.AltDirectorySeparatorChar.ToString()))
                {
                    i = s.IndexOf(System.IO.Path.AltDirectorySeparatorChar) + 1;
                    s = s.Remove(0, i);
                }
            }
            return s;
        }

        /// <summary>
        /// Gets a seperated string from a string with a char seperator (i.e a semicolon).
        /// </summary>
        /// <param name="s">The string to retrieve from.</param>
        /// <param name="nr">Zero based index.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>The seperated string.</returns>
        public static string GetSeprStr(string s, byte nr, char seperator)
        {
            string[] ss = s.Split(new char[] { seperator });
            if (ss.Length < nr)
                throw new Exception("BaseOperations::Strings::GetSeprStr reports: The desired index is out of range.");
            return ss[nr];
        }

        public static string AdjustString(string timeUnit, byte length, char addChar)
        {
            while (timeUnit.Length < length)
                timeUnit = addChar + timeUnit;
            return timeUnit;
        }


        public static char[] GetSlotGameIDnr(byte nrProductIDs)
        {
            if(nrProductIDs > 14)
                throw new Exception("BaseOperations::Strings::GetSlotGameIDnr reports: The desired index is out of range.");
            string s = nrProductIDs.ToString();
            if (s.Length < 2)
                s = "0" + s;
            return s.ToCharArray();
        }

        public static string GetCardString(Card card)
        {
            return ((card.Attack / 16).ToString("x") +
                card.AttackType +
                (card.PhysicalDefence / 16).ToString("x") +
                (card.MagicDefence / 16).ToString("x")).ToUpper();
        }
    }
}