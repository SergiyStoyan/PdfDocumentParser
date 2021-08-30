//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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

            /// <summary>
            /// !!!Used for debugging tips only
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "{" + Char + "}" + ", " + R.ToString();
            }
        }

        public static List<Line<CharBoxT>> GetLines<CharBoxT>(IEnumerable<CharBoxT> cbs, TextAutoInsertSpace textAutoInsertSpace, CharFilter charFilter) where CharBoxT : CharBox, new()
        {
            if (textAutoInsertSpace?.IgnoreSourceSpaces == true)
                cbs = cbs.Where(a => a.Char != " ");
            if (charFilter != null)//to filter out wrong OCR chars like borders etc which brakes lines
            {
                //SizeF s=new SizeF(ignoreCharsBiggerThan.Width*Settings.Constants.Pdf2ImageResolutionRatio)
                float maxWidth = charFilter.MaxWidth <= 0 ? float.MaxValue : charFilter.MaxWidth;
                float maxHeight = charFilter.MaxHeight <= 0 ? float.MaxValue : charFilter.MaxHeight;
                cbs = cbs.Where(a => a.R.Width >= charFilter.MinWidth && a.R.Width <= maxWidth && a.R.Height >= charFilter.MinHeight && a.R.Height <= maxHeight);
            }
            List<Line<CharBoxT>> lines = new List<Line<CharBoxT>>();
            foreach (CharBoxT cb in cbs)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    float mY = cb.R.Bottom - cb.R.Height / 2;
                    if (mY < lines[i].Top)
                    {
                        Line<CharBoxT> l = new Line<CharBoxT> { Top = cb.R.Top, Bottom = cb.R.Bottom };
                        l.CharBoxs.Add(cb);
                        lines.Insert(i, l);
                        goto NEXT_CHAR;
                    }
                    if (mY <= lines[i].Bottom)//the char's center is in the line
                    {
                        lines[i].CharBoxs.Add(cb);
                        if (lines[i].Top > cb.R.Top)
                            lines[i].Top = cb.R.Top;
                        if (lines[i].Bottom < cb.R.Bottom)
                            lines[i].Bottom = cb.R.Bottom;
                        goto NEXT_CHAR;
                    }
                }
                {
                    Line<CharBoxT> l = new Line<CharBoxT> { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxs.Add(cb);
                    lines.Add(l);
                }
            NEXT_CHAR:;
            }

            for (int i = 1; i < lines.Count; i++)
            {
                float intersectionH2 = (lines[i - 1].Bottom - lines[i].Top) * 2;
                if (intersectionH2 > lines[i - 1].Height || intersectionH2 > lines[i].Height)
                {
                    lines[i - 1].CharBoxs.AddRange(lines[i].CharBoxs);
                    if (lines[i - 1].Top > lines[i].Top)
                        lines[i - 1].Top = lines[i].Top;
                    if (lines[i - 1].Bottom < lines[i].Bottom)
                        lines[i - 1].Bottom = lines[i].Bottom;
                    lines.RemoveAt(i);
                    i--;
                }
            }

            lines.ForEach(a => a.CharBoxs = a.CharBoxs.OrderBy(b => b.R.X).ToList());

            if (textAutoInsertSpace?.Threshold > 0)
                foreach (Line<CharBoxT> l in lines)
                    for (int i = 1; i < l.CharBoxs.Count; i++)
                    {
                        CharBox cb0 = l.CharBoxs[i - 1];
                        CharBox cb = l.CharBoxs[i];
                        if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (/*cb0.R.Width*/0.8 / cb0.R.Height + /*cb.R.Width*/0.8 / cb.R.Height) * textAutoInsertSpace.Threshold)
                        {
                            float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                            int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                            for (int j = 0; j < spaceNumber; j++)
                                l.CharBoxs.Insert(i, new CharBoxT { Char = textAutoInsertSpace.Representative, R = new RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
                            i += spaceNumber;
                        }
                    }

            return lines;
        }

        /// <summary>
        /// Splits the chars onto non-intesecting lines.
        /// </summary>
        /// <typeparam name="CharBoxT"></typeparam>
        /// <param name="cbs"></param>
        /// <param name="textAutoInsertSpace"></param>
        /// <returns></returns>
        public static List<Line<CharBoxT>> GetLines2<CharBoxT>(IEnumerable<CharBoxT> cbs, TextAutoInsertSpace textAutoInsertSpace) where CharBoxT : CharBox, new()
        {//!!!no line must intersect with an other!!!
            bool spaceAutoInsert = textAutoInsertSpace?.Threshold > 0;
            if (textAutoInsertSpace?.IgnoreSourceSpaces == true)
                cbs = cbs.Where(a => a.Char != " ");

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
                        goto NEXT_CHAR;
                    }
                    if (cb.R.Top <= lines[i].Bottom)//the char is on the line
                    {
                        if (i + 1 < lines.Count && cb.R.Bottom >= lines[i + 1].Top)//the char is also on the next line
                        {
                            lines[i].CharBoxs.AddRange(lines[i + 1].CharBoxs);
                            if (lines[i].Top > lines[i + 1].Top)
                                lines[i].Top = lines[i + 1].Top;
                            if (lines[i].Bottom < lines[i + 1].Bottom)
                                lines[i].Bottom = lines[i + 1].Bottom;
                            lines.RemoveAt(i + 1);
                        }
                        lines[i].CharBoxs.Add(cb);
                        if (lines[i].Top > cb.R.Top)
                            lines[i].Top = cb.R.Top;
                        if (lines[i].Bottom < cb.R.Bottom)
                            lines[i].Bottom = cb.R.Bottom;
                        goto NEXT_CHAR;
                    }
                }
                {
                    Line<CharBoxT> l = new Line<CharBoxT> { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxs.Add(cb);
                    lines.Add(l);
                }
            NEXT_CHAR:;
            }

            for (int i = 1; i < lines.Count; i++)
            {
                float intersetionH2 = (lines[i - 1].Bottom - lines[i].Top) * 2;
                if (intersetionH2 > lines[i - 1].Height || intersetionH2 > lines[i].Height)
                {
                    lines[i - 1].CharBoxs.AddRange(lines[i].CharBoxs);
                    if (lines[i - 1].Top > lines[i].Top)
                        lines[i - 1].Top = lines[i].Top;
                    if (lines[i - 1].Bottom < lines[i].Bottom)
                        lines[i - 1].Bottom = lines[i].Bottom;
                    lines.RemoveAt(i);
                    i--;
                }
            }

            lines.ForEach(a => a.CharBoxs = a.CharBoxs.OrderBy(b => b.R.X).ToList());

            if (spaceAutoInsert)
                foreach (Line<CharBoxT> l in lines)
                    for (int i = 1; i < l.CharBoxs.Count; i++)
                    {
                        CharBox cb0 = l.CharBoxs[i - 1];
                        CharBox cb = l.CharBoxs[i];
                        if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (/*cb0.R.Width*/0.8 / cb0.R.Height + /*cb.R.Width*/0.8 / cb.R.Height) * textAutoInsertSpace.Threshold)
                        {
                            float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                            int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                            for (int j = 0; j < spaceNumber; j++)
                                l.CharBoxs.Insert(i, new CharBoxT { Char = textAutoInsertSpace.Representative, R = new RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
                            i += spaceNumber;
                        }
                    }
            return lines;
        }

        public class Line<T> where T : CharBox
        {
            public float Left { get { return CharBoxs.Count > 0 ? CharBoxs[0].R.Left : -1; } }
            public float Right { get { return CharBoxs.Count > 0 ? CharBoxs[CharBoxs.Count - 1].R.Right : -1; } }
            public float Top;
            public float Bottom;
            public float Height { get { return Bottom - Top; } }
            public List<T> CharBoxs = new List<T>();

            /// <summary>
            /// to avoid cropping text
            /// </summary>
            internal void Pad(int linePaddingY)
            {
                Top = CharBoxs.Min(a => a.R.Top) - linePaddingY;
                Bottom = CharBoxs.Max(a => a.R.Bottom) + linePaddingY;
                //Height = Bottom - Top;
            }

            /// <summary>
            /// !!!Used for debugging tips only
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in CharBoxs)
                    sb.Append(cb.Char);
                return "{" + sb.ToString() + "}" + ", {Left=" + Left + ",Right=" + Right + ",Top=" + Top + ",Bottom=" + Bottom + ",Height=" + Height + "}";
            }

            public string GetString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in CharBoxs)
                    sb.Append(cb.Char);
                return sb.ToString();
            }
        }

        public static List<string> GetTextLines<CharBoxT>(IEnumerable<CharBoxT> cbs, TextAutoInsertSpace textAutoInsertSpace, CharFilter charFilter) where CharBoxT : CharBox, new()
        {
            return GetLines(cbs, textAutoInsertSpace, charFilter).Select(a => a.GetString()).ToList();
        }

        public static string GetText(IEnumerable<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace, CharFilter charFilter)
        {
            return string.Join("\r\n", GetTextLines(cbs, textAutoInsertSpace, charFilter));
        }

        internal static void AdjustBorders<CharBoxT>(List<Line<CharBoxT>> ls, RectangleF ar) where CharBoxT : CharBox, new()
        {
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
                    if (l.Bottom >= ls[i + 1].Top)
                        continue;
                    l.Bottom = (ls[i + 1].Top + l.Bottom) / 2;
                    ls[i + 1].Top = l.Bottom;
                }
                else
                    l.Bottom = (l.Bottom - l.Top) < (ar.Bottom - l.Bottom) ? l.Bottom : ar.Bottom;
            }
        }

        public static void PadLines<CharBoxT>(List<Line<CharBoxT>> ls, int linePaddingY) where CharBoxT : CharBox, new()
        {
            if (linePaddingY > 0)
                ls.ForEach(a => a.Pad(linePaddingY));
        }

        /// <summary>
        /// Auxiliary method which can be applied to a string during post-processing.
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

        /// <summary>
        /// Auxiliary method which can be applied to a string collection during post-processing.
        /// </summary>
        /// <param name="values"></param>
        public static void NormalizeText(List<string> values)
        {
            for (int i = 0; i < values.Count; i++)
                values[i] = NormalizeText(values[i]);
        }
    }
}