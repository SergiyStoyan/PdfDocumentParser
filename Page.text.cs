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
            public RectangleF R;
        }

        public static List<Line> GetLines(IEnumerable<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            bool spaceAutoInsert = textAutoInsertSpace != null && textAutoInsertSpace.Threshold > 0;
            cbs = cbs.OrderBy(a => a.R.X).ToList();
            List<Line> lines = new List<Line>();
            foreach (CharBox cb in cbs)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (cb.R.Bottom < lines[i].Top)
                    {
                        Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                        l.CharBoxs.Add(cb);
                        lines.Insert(i, l);
                        goto CONTINUE;
                    }
                    if (cb.R.Bottom - cb.R.Height / 2 <= lines[i].Bottom)
                    {
                        if (spaceAutoInsert && /*cb.Char != " " &&*/ lines[i].CharBoxs.Count > 0)
                        {
                            CharBox cb0 = lines[i].CharBoxs[lines[i].CharBoxs.Count - 1];
                            if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (cb0.R.Width / cb0.R.Height + cb.R.Width / cb.R.Height) * textAutoInsertSpace.Threshold)
                            {
                                float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                                int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                                for (int j = 0; j < spaceNumber; j++)
                                    lines[i].CharBoxs.Add(new CharBox { Char = textAutoInsertSpace.Representative, R = new RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
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
                    Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxs.Add(cb);
                    lines.Add(l);
                }
            CONTINUE:;
            }
            return lines;
        }

        public class Line
        {
            public float Left { get { return CharBoxs[0].R.Left; } }
            public float Right { get { return CharBoxs[CharBoxs.Count - 1].R.Right; } }
            public float Top;
            public float Bottom;
            public List<CharBox> CharBoxs = new List<CharBox>();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in CharBoxs)
                    sb.Append(cb.Char);
                return sb.ToString();
            }
        }

        public static List<string> GetTextLines(IEnumerable<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs, textAutoInsertSpace))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return ls;
        }

        internal static List<Line> GetLinesWithAdjacentBorders(IEnumerable<CharBox> cbs, RectangleF ar)
        {
            List<Line> ls = GetLines(cbs, null);
            for (int i = 0; i < ls.Count; i++)
            {
                Line l = ls[i];
                if (ar.Top > l.Top)
                    continue;
                if (ar.Bottom < l.Bottom)
                    continue;
                if (i == 0)
                    l.Top -= (l.Bottom - l.Top) < (l.Top - ar.Top) ? (l.Bottom - l.Top) / 2 : (l.Top - ar.Top) / 2;
                if (i < ls.Count - 1)
                {
                    l.Bottom += (l.Bottom - l.Top) < (ar.Bottom - l.Bottom) ? (l.Bottom - l.Top) / 2 : (ar.Bottom - l.Bottom) / 2;
                    ls[i + 1].Top = l.Bottom;
                }
                else
                    l.Bottom = (l.Bottom + ar.Bottom) / 2;
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