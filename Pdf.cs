//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.parser;
using System.Diagnostics;
using ImageMagick;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// Auxiliary pdf routines
    /// </summary>
    internal static partial class Pdf
    {
        static public PdfReader CreatePdfReader(string pdfFile)
        {
            PdfReader.unethicalreading = true;
            return new PdfReader(pdfFile);
        }

        public static System.Drawing.SizeF GetPageSize2(this PdfReader pr, int page_i)
        {
            iTextSharp.text.Rectangle r = pr.GetPageSize(page_i);
            return new System.Drawing.SizeF(r.Width, r.Height);
        }

        static public RenderFilter[] CreateFilters(this PdfReader pr, float x, float y, float w, float h)
        {
            return new RenderFilter[] { new RegionTextRenderFilter(new System.util.RectangleJ(x, y, w, h)) };
        }

        //static public Dictionary<string, string> ExtractTexts(this PdfReader pr, int page_i, Dictionary<string, RenderFilter[]> fieldNames2filters)
        //{
        //    Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();
        //    foreach (string fn in fieldNames2filters.Keys)
        //        fieldNames2texts[fn] = pr.ExtractText(page_i, fieldNames2filters[fn]);
        //    return fieldNames2texts;
        //}

        public static string ExtractText(this PdfReader pr, int page_i, float x, float y, float w, float h)
        {
            RenderFilter[] rf = new RenderFilter[] { new RegionTextRenderFilter(new System.util.RectangleJ(x, y, w, h)) };
            return ExtractText(pr, page_i, rf);
        }

        public static string ExtractText(this PdfReader pr, int page_i, RenderFilter[] f)
        {
            ITextExtractionStrategy s = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), f);
            return PdfTextExtractor.GetTextFromPage(pr, page_i, new LimitedTextStrategy2(s));
            //return PdfTextExtractor.GetTextFromPage(pr, page_i, new LimitedTextStrategy(new LocationTextExtractionStrategy(), f));
        }
        //public class LimitedTextStrategy : FilteredTextRenderListener
        //{
        //    public LimitedTextStrategy(ITextExtractionStrategy strategy, RenderFilter[] filters):base(strategy, filters)
        //    {
        //    }
        //    public void RenderText(TextRenderInfo renderInfo)
        //    {
        //        foreach (TextRenderInfo info in renderInfo.GetCharacterRenderInfos())
        //        {
        //            base.RenderText(info);
        //        }
        //    }
        //}
        public class LimitedTextStrategy2 : iTextSharp.text.pdf.parser.ITextExtractionStrategy
        {
            public readonly ITextExtractionStrategy textextractionstrategy;

            public LimitedTextStrategy2(ITextExtractionStrategy strategy)
            {
                this.textextractionstrategy = strategy;
            }
            public void RenderText(TextRenderInfo renderInfo)
            {
                foreach (TextRenderInfo info in renderInfo.GetCharacterRenderInfos())
                {
                    this.textextractionstrategy.RenderText(info);
                }
            }
            public string GetResultantText()
            {
                return this.textextractionstrategy.GetResultantText();
            }

            public void BeginTextBlock()
            {
                this.textextractionstrategy.BeginTextBlock();

            }
            public void EndTextBlock()
            {
                this.textextractionstrategy.EndTextBlock();

            }
            public void RenderImage(ImageRenderInfo renderInfo)
            {
                this.textextractionstrategy.RenderImage(renderInfo);
            }
        }

        static public System.Drawing.Bitmap RenderBitmap(string pdf_file, int page_i, int resolution, bool byFile = false)
        {
            Process p = new Process();
            p.StartInfo.FileName = Log.AppDir + "\\gswin32c.exe";
            int dDownScaleFactor = 1;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;

            if (!byFile)
            {
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.Arguments = "-dNOPROMPT -r" + resolution + " -dDownScaleFactor=" + dDownScaleFactor + " -dBATCH -dFirstPage=" + page_i + " -dLastPage=" + page_i + " -sDEVICE=png16m -dNOPAUSE -sOutputFile=%stdout -q \"" + pdf_file + "\"";
                p.Start();
                MemoryStream ms = new MemoryStream();
                p.StandardOutput.BaseStream.CopyTo(ms);
                p.WaitForExit();

                try
                {
                    return new System.Drawing.Bitmap(System.Drawing.Image.FromStream(ms));
                }
                catch (Exception e)
                {
                    return RenderBitmap(pdf_file, page_i, resolution, true);
                }
            }
            else//some pdf's require this because gs puts errors to stdout
            {
                string buffer_file = Log.AppCommonDataDir + "\\buffer.png";
                p.StartInfo.Arguments = "-dNOPROMPT -r" + resolution + " -dDownScaleFactor=" + dDownScaleFactor + " -dBATCH -dFirstPage=" + page_i + " -dLastPage=" + page_i + " -sDEVICE=png16m -dNOPAUSE -sOutputFile=\"" + buffer_file + "\" -q \"" + pdf_file + "\"";
                p.Start();
                p.WaitForExit();

                return new System.Drawing.Bitmap(System.Drawing.Image.FromFile(buffer_file));
            }
        }

        static public List<LocationTextExtractionStrategy.TextChunk> GetTextChunks(this PdfReader pr, int page_i)
        {
            ChunkLocationTextExtractionStrategy2 s = new ChunkLocationTextExtractionStrategy2();
            PdfTextExtractor.GetTextFromPage(pr, page_i, s);
            return s.TextChunks;
        }
        public class ChunkLocationTextExtractionStrategy2 : LocationTextExtractionStrategy
        {
            public List<TextChunk> TextChunks = new List<TextChunk>();

            public override void RenderText(TextRenderInfo renderInfo)
            {
                base.RenderText(renderInfo);

                //Vector bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
                Vector bottomLeft = renderInfo.GetBaseline().GetStartPoint();
                Vector topRight = renderInfo.GetAscentLine().GetEndPoint();
                TextChunk tc = new TextChunk(renderInfo.GetText(), bottomLeft, topRight, renderInfo.GetSingleSpaceWidth());
                TextChunks.Add(tc);
            }
        }

        static public List<LocationTextExtractionStrategy.TextChunk> GetCharacterTextChunks(this PdfReader pr, int page_i)
        {
            CharLocationTextExtractionStrategy s = new CharLocationTextExtractionStrategy();
            PdfTextExtractor.GetTextFromPage(pr, page_i, s);
            return s.TextChunks;
        }
        public class CharLocationTextExtractionStrategy : LocationTextExtractionStrategy
        {
            public List<TextChunk> TextChunks = new List<TextChunk>();

            public override void RenderText(TextRenderInfo renderInfo)
            {
                base.RenderText(renderInfo);

                List<TextChunk> tcs = new List<TextChunk>();
                IList<TextRenderInfo> cris = renderInfo.GetCharacterRenderInfos();
                foreach (TextRenderInfo cri in cris)
                {
                    //Vector bottomLeft = cri.GetDescentLine().GetStartPoint();
                    Vector bottomLeft = cri.GetBaseline().GetStartPoint();
                    Vector topRight = cri.GetAscentLine().GetEndPoint();
                    TextChunk tc = new TextChunk(cri.GetText(), bottomLeft, topRight, cri.GetSingleSpaceWidth());
                    //TextChunks.Add(new TextChunk2(rect, renderInfo.GetText()));
                    tcs.Add(tc);
                }
                TextChunks.AddRange(tcs);
            }
        }

        public static string GetTextByTopLeftCoordinates(List<CharBox> cbs, System.Drawing.RectangleF r)
        {
            cbs = removeDuplicates(cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)));
            cbs = cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)).ToList();
            List<string> ls = new List<string>();
            foreach (Line l in getLines(cbs, true))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxes)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return string.Join("\r\n", ls);
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(List<CharBox> bts, System.Drawing.RectangleF r, bool excludeInvisibleCharacters)
        {
            var bts_ = bts.Where(a => /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R));
            if (excludeInvisibleCharacters)
            {
                string ignoredCharacters = " \t";
                bts_ = bts_.Where(a => !ignoredCharacters.Contains(a.Char));
            }
            return bts_.ToList();
        }

        public static List<Line> RemoveDuplicatesAndGetLines(IEnumerable<CharBox> cbs, bool spaceAutoInsert)
        {
            return getLines(removeDuplicates(cbs), spaceAutoInsert);
        }

        static List<CharBox> removeDuplicates(IEnumerable<CharBox> cbs)
        {
            List<CharBox> bs = cbs.Where(a => a.R.Width >= 0 && a.R.Height >= 0).ToList();//some symbols are duplicated with negative width anf height
            for (int i = 0; i < bs.Count; i++)
                for (int j = bs.Count - 1; j > i; j--)
                {
                    if (Math.Abs(bs[i].R.X - bs[j].R.X) > Settings.ImageProcessing.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (Math.Abs(bs[i].R.Y - bs[j].R.Y) > Settings.ImageProcessing.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (bs[i].Char != bs[j].Char)
                        continue;
                    bs.RemoveAt(j);
                }
            return bs;
        }

        static List<Line> getLines(IEnumerable<CharBox> cbs, bool spaceAutoInsert)
        {
            cbs = cbs.OrderBy(a => a.R.X).ToList();
            List<Line> lines = new List<Line>();
            foreach (CharBox cb in cbs)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (cb.R.Bottom < lines[i].Top)
                    {
                        Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                        l.CharBoxes.Add(cb);
                        lines.Insert(i, l);
                        goto CONTINUE;
                    }
                    if (cb.R.Bottom - cb.R.Height / 2 <= lines[i].Bottom)
                    {
                        if (spaceAutoInsert && cb.Char != " " && lines[i].CharBoxes.Count > 0)
                        {
                            CharBox cb0 = lines[i].CharBoxes[lines[i].CharBoxes.Count - 1];
                            if (cb0.Char != " " && cb.R.Left - cb0.R.Right > (cb.R.Width + cb.R.Height) / 5)
                                lines[i].CharBoxes.Add(new CharBox { Char = " ", R = new System.Drawing.RectangleF((cb.R.Left + cb0.R.Right) / 2, 0, 0, 0) });
                        }
                        lines[i].CharBoxes.Add(cb);
                        if (lines[i].Top > cb.R.Top)
                            lines[i].Top = cb.R.Top;
                        if (lines[i].Bottom < cb.R.Bottom)
                            lines[i].Bottom = cb.R.Bottom;
                        goto CONTINUE;
                    }
                }
                {
                    Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxes.Add(cb);
                    lines.Add(l);
                }
                CONTINUE:;
            }
            return lines;
        }

        public class Line
        {
            public float Top;
            public float Bottom;
            public List<CharBox> CharBoxes = new List<CharBox>();
        }

        public static List<CharBox> GetCharBoxsFromPage(PdfReader pdfReader, int pageI)
        {
            var bts = pdfReader.GetCharacterTextChunks(pageI).Select(x => new Pdf.CharBox
            {
                R = new System.Drawing.RectangleF
                {
                    X = x.StartLocation[Vector.I1],
                    Y = pdfReader.GetPageSize(pageI).Height - x.EndLocation[Vector.I2],
                    Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
                    Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
                },
                Char = x.Text
            });
            return bts.ToList();
        }

        public class CharBox
        {
            public System.Drawing.RectangleF R;
            public string Char;
        }
    }
}