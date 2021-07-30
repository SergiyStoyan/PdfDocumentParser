//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Cliver.PdfDocumentParser
{
    public partial class Page
    {
        public class CharBox
        {
            public string Char;
            /// <summary>
            /// (!)Y is the distance from the page's top to the char's top. But it is actually culculated from the char's baseline which is its bottom. It is because the baseline is considred most reliable position given by iText, while ascentLine or descentLine are not.
            /// </summary>
            public RectangleF R;
        }

        public static List<Line<CharBoxT>> GetLines<CharBoxT>(IEnumerable<CharBoxT> cbs, TextAutoInsertSpace textAutoInsertSpace) where CharBoxT : CharBox, new()
        {
            bool spaceAutoInsert = textAutoInsertSpace?.Threshold > 0;
            if (textAutoInsertSpace?.IgnoreSourceSpaces == true)
                cbs = cbs.Where(a => a.Char != " ");
            cbs = cbs.OrderBy(a => a.R.X).ToList();
            List<Line<CharBoxT>> lines = new List<Line<CharBoxT>>();
            foreach (CharBoxT cb in cbs)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (cb.R.Bottom < lines[i].Top)
                    {
                        Line<CharBoxT> l = new Line<CharBoxT> { Top = cb.R.Top, Bottom = cb.R.Bottom };
                        l.CharBoxs.Add(cb);
                        lines.Insert(i, l);
                        goto CONTINUE;
                    }
                    if (cb.R.Bottom - cb.R.Height / 2 <= lines[i].Bottom)
                    {
                        if (spaceAutoInsert && /*cb.Char != " " &&*/ lines[i].CharBoxs.Count > 0)
                        {
                            CharBox cb0 = lines[i].CharBoxs[lines[i].CharBoxs.Count - 1];
                            if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (/*cb0.R.Width*/0.8 / cb0.R.Height + /*cb.R.Width*/0.8 / cb.R.Height) * textAutoInsertSpace.Threshold)
                            {
                                float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                                int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                                for (int j = 0; j < spaceNumber; j++)
                                    lines[i].CharBoxs.Add(new CharBoxT { Char = textAutoInsertSpace.Representative, R = new RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
                            }
                        }
                        lines[i].CharBoxs.Add(cb);
                        if (lines[i].Top > cb.R.Top)
                            lines[i].Top = cb.R.Top;
                        if (lines[i].Bottom < cb.R.Bottom)
                            lines[i].Bottom = cb.R.Bottom;
                        goto CONTINUE;
                    }
                }
                {
                    Line<CharBoxT> l = new Line<CharBoxT> { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxs.Add(cb);
                    lines.Add(l);
                }
            CONTINUE:;
            }
            return lines;
        }

        public class Line<T> where T : CharBox
        {
            public float Left { get { return CharBoxs[0].R.Left; } }
            public float Right { get { return CharBoxs[CharBoxs.Count - 1].R.Right; } }
            public float Top;
            public float Bottom;
            public List<T> CharBoxs = new List<T>();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in CharBoxs)
                    sb.Append(cb.Char);
                return sb.ToString();
            }
        }

        public static List<string> GetTextLines<CharBoxT>(IEnumerable<CharBoxT> cbs, TextAutoInsertSpace textAutoInsertSpace) where CharBoxT : CharBox, new()
        {
            List<string> ls = new List<string>();
            foreach (Line<CharBoxT> l in GetLines(cbs, textAutoInsertSpace))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return ls;
        }

        public static string GetText(IEnumerable<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            return string.Join("\r\n", GetTextLines(cbs, textAutoInsertSpace));
        }

        internal static List<Line<CharBoxT>> GetLinesWithAdjacentBorders<CharBoxT>(IEnumerable<CharBoxT> cbs, RectangleF ar) where CharBoxT : CharBox, new()
        {
            List<Line<CharBoxT>> ls = GetLines(cbs, null);
            for (int i = 0; i < ls.Count; i++)
            {
                Line<CharBoxT> l = ls[i];
                if (ar.Top > l.Top)
                    continue;
                if (ar.Bottom < l.Bottom)
                    continue;
                if (i == 0)
                    l.Top = (l.Bottom - l.Top) < (l.Top - ar.Top) ? (l.Bottom + l.Top) / 2 : ar.Top;
                if (i < ls.Count - 1)
                {
                    l.Bottom = (ls[i + 1].Top + l.Bottom) / 2;
                    ls[i + 1].Top = l.Bottom;
                }
                else
                    l.Bottom = (l.Bottom - l.Top) < (ar.Bottom - l.Bottom) ? (l.Bottom + l.Top) / 2 : ar.Bottom;
            }
            return ls;
        }

        /// <summary>
        /// Auxiliary method which can be applied to a string during post-processing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NormalizeText(string value)
        {
            if (value == null)
                return null;
            value = FieldPreparation.ReplaceNonPrintableChars(value);
            value = Regex.Replace(value, @"\s+", " ");
            value = value.Trim();
            return value;
        }

        public static void NormalizeText(List<string> values)
        {
            for (int i = 0; i < values.Count; i++)
                values[i] = NormalizeText(values[i]);
        }
    }
}