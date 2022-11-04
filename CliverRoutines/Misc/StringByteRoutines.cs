//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliver
{
    public static class StringByteRoutines
    {
        public static string Compare2FilesAsByteArrays(string f1, string f2, int start_p1, int start_p2, int length1 = -1, bool hexadecimal_output = true)
        {
            string s = null;

            string output_format;
            if (hexadecimal_output)
                output_format = "X";
            else
                output_format = "N";

            byte[] bs1 = File.ReadAllBytes(f1);
            byte[] bs2 = File.ReadAllBytes(f2);

            if (length1 < 0)
                length1 = bs1.Length - start_p1;
            int end_p1 = start_p1 + length1;
            int end_p2 = start_p2 + length1;

            if (bs1.Length < end_p1)
                throw new Exception("bs1.Length < end_p1");
            if (bs2.Length < end_p2)
                throw new Exception("bs2.Length < end_p2");

            int shift2 = start_p2 - start_p1;
            int prev_i = -1;
            for (int i = start_p1; i < end_p1; i++)
                if (bs1[i] != bs2[i + shift2])
                {
                    if (prev_i + 1 != i)
                        s += "\r\n";
                    prev_i = i;
                    s += i.ToString() + ":" + bs1[i].ToString(output_format) + "[" + Convert.ToChar(bs1[i]) + "]" + " != " + bs2[i + shift2].ToString(output_format) + "[" + Convert.ToChar(bs2[i]) + "]";
                }

            return s;
        }

        public static string GetHexadecimalReadableRepresentation(byte[] bs, int startPosition = 0, int? length = null)
        {
            if (bs == null)
                return null;
            if (length == null)
                length = bs.Length;
            string s = BitConverter.ToString(bs, startPosition, (int)length);
            s = Regex.Replace(s, @"\s+", "");
            return s;
        }

        public static string GetHexadecimalPrefixedRepresentation(byte[] bs, int startPosition = 0, int? length = null, string prefix = "0x")
        {
            if (bs == null)
                return null;
            if (length == null)
                length = bs.Length;
            string s = BitConverter.ToString(bs, startPosition, (int)length);
            s = Regex.Replace(s, @"\s+", "");
            s = Regex.Replace(s, @"\-", "");
            return prefix + s;
        }

        //static string normalize_hexadecimal_representation(string s)
        //{
        //    const string between_8_and_9 = "    ";
        //    s = Regex.Replace(s, between_8_and_9, "", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //    s = Regex.Replace(s, @"((?:[\da-z]{2}[^\da-z]*){7}[\da-z]{2})(?:[^\da-z]*)", "$1" + between_8_and_9, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //    s = Regex.Replace(s, @"(.*?" + between_8_and_9 + ".*?)" + between_8_and_9, "$1\r\n", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //    return s;
        //}

        public static byte[] GetByteArrayFromHexadecimalRepresentation(string hexadecimalRepresentation)
        {
            if (hexadecimalRepresentation == null)
                return null;
            hexadecimalRepresentation = Regex.Replace(hexadecimalRepresentation.Trim(), @"^0x", "");
            hexadecimalRepresentation = Regex.Replace(hexadecimalRepresentation, @"[^\da-f]+", "");
            int cn = hexadecimalRepresentation.Length / 2;
            byte[] bytes = new byte[cn];
            StringReader sr = new StringReader(hexadecimalRepresentation);
            for (int i = 0; i < cn; i++)
                bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            return bytes;
        }

        public static string ToBitString(this BitArray bits)
        {
            char[] cs = new char[bits.Count];
            for (int i = 0; i < bits.Count; i++)
                cs[i] = bits[i] ? '1' : '0';
            return new string(cs);
        }
    }
}

