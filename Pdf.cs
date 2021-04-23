//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Diagnostics;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Geom;
using iText.Kernel.Font;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// PDF routines
    /// </summary>
    public static partial class Pdf
    {
        //static public PdfReader CreatePdfReader(string pdfFile)
        //{
        //    PdfReader.unethicalreading = true;
        //    return new PdfReader(pdfFile);
        //}

        //public static System.Drawing.SizeF GetPageSizeF(this PdfReader pr, int pageI)
        //{
        //    iTextSharp.text.Rectangle r = pr.GetPageSize(pageI);
        //    return new System.Drawing.SizeF(r.Width, r.Height);
        //}

        public static System.Drawing.Size GetPageSize(PdfDocument pdfDocument, int pageI)
        {
            Rectangle r = pdfDocument.GetPage(pageI).GetPageSize();
            return new System.Drawing.Size((int)r.GetWidth(), (int)r.GetHeight());
        }

        //static public RenderFilter[] CreateFilters(this PdfReader pr, float x, float y, float w, float h)
        //{
        //    return new RenderFilter[] { new RegionTextRenderFilter(new System.util.RectangleJ(x, y, w, h)) };
        //}

        //static public Dictionary<string, string> ExtractTexts(this PdfReader pr, int pageI, Dictionary<string, RenderFilter[]> fieldNames2filters)
        //{
        //    Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();
        //    foreach (string fn in fieldNames2filters.Keys)
        //        fieldNames2texts[fn] = pr.ExtractText(pageI, fieldNames2filters[fn]);
        //    return fieldNames2texts;
        //}

        //public static string ExtractText(this PdfReader pr, int pageI, float x, float y, float w, float h)
        //{
        //    RenderFilter[] rf = new RenderFilter[] { new RegionTextRenderFilter(new System.util.RectangleJ(x, y, w, h)) };
        //    return ExtractText(pr, pageI, rf);
        //}

        //public static string ExtractText(this PdfReader pr, int pageI, RenderFilter[] f)
        //{
        //    ITextExtractionStrategy s = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), f);
        //    return PdfTextExtractor.GetTextFromPage(pr, pageI, new LimitedTextStrategy2(s));
        //    //return PdfTextExtractor.GetTextFromPage(pr, pageI, new LimitedTextStrategy(new LocationTextExtractionStrategy(), f));
        //}
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
        //public class LimitedTextStrategy2 : iTextSharp.text.pdf.parser.ITextExtractionStrategy
        //{
        //    public readonly ITextExtractionStrategy textextractionstrategy;

        //    public LimitedTextStrategy2(ITextExtractionStrategy strategy)
        //    {
        //        this.textextractionstrategy = strategy;
        //    }
        //    public void RenderText(TextRenderInfo renderInfo)
        //    {
        //        foreach (TextRenderInfo info in renderInfo.GetCharacterRenderInfos())
        //        {
        //            this.textextractionstrategy.RenderText(info);
        //        }
        //    }
        //    public string GetResultantText()
        //    {
        //        return this.textextractionstrategy.GetResultantText();
        //    }

        //    public void BeginTextBlock()
        //    {
        //        this.textextractionstrategy.BeginTextBlock();

        //    }
        //    public void EndTextBlock()
        //    {
        //        this.textextractionstrategy.EndTextBlock();

        //    }
        //    public void RenderImage(ImageRenderInfo renderInfo)
        //    {
        //        this.textextractionstrategy.RenderImage(renderInfo);
        //    }
        //}

        static public System.Drawing.Bitmap RenderBitmap(string pdfFile, int pageI, int resolution, bool byFile = false)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = Log.AppDir + "\\gswin32c.exe";
                int dDownScaleFactor = 1;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.UseShellExecute = false;

                if (!byFile)
                {
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.Arguments = "-dNOPROMPT -r" + resolution + " -dDownScaleFactor=" + dDownScaleFactor + " -dBATCH -dFirstPage=" + pageI + " -dLastPage=" + pageI + " -sDEVICE=png16m -dNOPAUSE -sOutputFile=%stdout -q \"" + pdfFile + "\"";
                    p.Start();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        p.StandardOutput.BaseStream.CopyTo(ms);
                        p.WaitForExit();
                        try
                        {
                            return new System.Drawing.Bitmap(ms);
                        }
                        catch (Exception e)
                        {
                            return RenderBitmap(pdfFile, pageI, resolution, true);
                        }
                    }
                }
                else//some pdf's require this because gs puts errors to stdout
                {
                    string bufferFileDir = System.IO.Path.GetTempPath() + Log.ProcessName;
                    Directory.CreateDirectory(bufferFileDir);
                    string bufferFile = bufferFileDir + "\\buffer.png";
                    p.StartInfo.Arguments = "-dNOPROMPT -r" + resolution + " -dDownScaleFactor=" + dDownScaleFactor + " -dBATCH -dFirstPage=" + pageI + " -dLastPage=" + pageI + " -sDEVICE=png16m -dNOPAUSE -sOutputFile=\"" + bufferFile + "\" -q \"" + pdfFile + "\"";
                    p.Start();
                    p.WaitForExit();
                    System.Drawing.Bitmap b;// = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(bufferFile);
                    using (var bt = new System.Drawing.Bitmap(bufferFile))//to free the file
                    {
                        b = new System.Drawing.Bitmap(bt);//!!!sets 96dpi
                        b.SetResolution(bt.HorizontalResolution, bt.VerticalResolution);
                    }
                    Directory.Delete(bufferFileDir, true);
                    return b;
                }
            }
        }

        //static public List<LocationTextExtractionStrategy.TextChunk> GetTextChunks(this PdfReader pr, int pageI)
        //{
        //    ChunkLocationTextExtractionStrategy2 s = new ChunkLocationTextExtractionStrategy2();
        //    PdfTextExtractor.GetTextFromPage(pr, pageI, s);
        //    return s.TextChunks;
        //}
        //public class ChunkLocationTextExtractionStrategy2 : LocationTextExtractionStrategy
        //{
        //    public List<TextChunk> TextChunks = new List<TextChunk>();

        //    public override void RenderText(TextRenderInfo renderInfo)
        //    {
        //        base.RenderText(renderInfo);

        //        //Vector bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
        //        Vector bottomLeft = renderInfo.GetBaseline().GetStartPoint();
        //        Vector topRight = renderInfo.GetAscentLine().GetEndPoint();
        //        TextChunk tc = new TextChunk(renderInfo.GetText(), bottomLeft, topRight, renderInfo.GetSingleSpaceWidth());
        //        TextChunks.Add(tc);
        //    }
        //}

        static public List<CharBox> GetCharBoxsFromPage(PdfDocument pdfDocument, int pageI, bool removeDuplicates)
        {
            PdfPage p = pdfDocument.GetPage(pageI);
            Rectangle r = p.GetPageSize();
            CharBoxExtractionStrategy s = new CharBoxExtractionStrategy(new System.Drawing.RectangleF(r.GetX(), r.GetY(), r.GetWidth(), r.GetHeight()));
            PdfTextExtractor.GetTextFromPage(p, s);
            if (removeDuplicates)
                s.CharBoxs = RemoveDuplicates(s.CharBoxs);
            return s.CharBoxs;
        }
        public class CharBoxExtractionStrategy : LocationTextExtractionStrategy
        {
            public List<CharBox> CharBoxs = new List<CharBox>();

            public CharBoxExtractionStrategy(System.Drawing.RectangleF pageSize) : base()
            {
                this.pageSize = pageSize;
            }
            System.Drawing.RectangleF pageSize;
            Dictionary<string, PdfFont> fontNames2font = new Dictionary<string, PdfFont>();

            public override void EventOccurred(IEventData data, EventType type)
            {
                base.EventOccurred(data, type);

                TextRenderInfo tri = data as TextRenderInfo;
                //if (type != EventType.RENDER_TEXT)
                if (tri == null)
                    return;

                PdfFont f = tri.GetFont();
                string fn = f.ToString();
                if (!fontNames2font.TryGetValue(fn, out PdfFont font))
                {
                    font = f;
                    fontNames2font[fn] = font;
                }
                float fontSize = Math.Abs/*sometimes it is negative in iText7*/(tri.GetFontSize());
                //if (font.GetAscent("I", fontSize) == 0)
                //    fontSize = 0;

                List<CharBox> cbs = new List<CharBox>();
                IList<TextRenderInfo> cris = tri.GetCharacterRenderInfos();
                foreach (TextRenderInfo cri in cris)
                {
                    Vector baseLeft = cri.GetBaseline().GetStartPoint();
                    Vector topRight = cri.GetAscentLine().GetEndPoint();
                    //float rise = cri.GetRise();
                    //if (rise != 0)
                    //    rise = rise;
                    float fontHeightFromBaseLine = topRight.Get(Vector.I2) - baseLeft.Get(Vector.I2);
                    //if (fontSize > 0)
                    //{
                    //    float fontHeightFromBaseLine2 = fontSize * 0.75f/*(?)convertion from PX to PT*/;//!!!this calculation is heuristic and coincides with cri.GetAscentLine().GetEndPoint().Get(Vector.I2) - bottomLeft.Get(Vector.I2) in iText5 
                    //    if (fontHeightFromBaseLine > fontHeightFromBaseLine2)
                    //        fontHeightFromBaseLine = fontHeightFromBaseLine2;
                    //}
                    float x = baseLeft.Get(Vector.I1);
                    //float y = topRight.Get(Vector.I2);
                    CharBox cb = new CharBox
                    {
                        Char = cri.GetText(),
                        R = new System.Drawing.RectangleF
                        {
                            X = x - pageSize.X,
                            Y = pageSize.Height + pageSize.Y - baseLeft.Get(Vector.I2) - fontHeightFromBaseLine,
                            Width = topRight.Get(Vector.I1) - x,
                            //Height = y - bottomLeft.Get(Vector.I2)// + d,
                            Height = fontHeightFromBaseLine
                        },
                        Font = font,
                        FontSize = fontSize
                    };
                    cbs.Add(cb);
                }
                CharBoxs.AddRange(cbs);
            }
        }

        public static List<string> GetTextLinesSurroundedByRectangle(IEnumerable<CharBox> cbs, System.Drawing.RectangleF r, TextAutoInsertSpace textAutoInsertSpace)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            List<string> ls = new List<string>();
            foreach (Page.Line<CharBox> l in Page.GetLines(cbs, textAutoInsertSpace))
                ls.Add(l.ToString());
            return ls;
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(IEnumerable<CharBox> cbs, System.Drawing.RectangleF r, bool excludeInvisibleCharacters = false)
        {
            //cbs = RemoveDuplicates(cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)));
            cbs = cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/));
            if (excludeInvisibleCharacters)
                cbs = cbs.Where(a => !InvisibleCharacters.Contains(a.Char)).ToList();
            return cbs.ToList();
        }

        //public static List<CharBox> GetCharBoxsFromPage(PdfReader pdfReader, int pageI, bool removeDuplicates)
        //{
        //    var cbs = pdfReader.GetCharacterTextChunks(pageI).Select(x => new Pdf.CharBox
        //    {
        //        R = new System.Drawing.RectangleF
        //        {
        //            X = x.StartLocation[Vector.I1],
        //            Y = pdfReader.GetPageSize(pageI).Height - x.EndLocation[Vector.I2],
        //            Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
        //            Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
        //        },
        //        Char = x.Text
        //    });
        //    if (removeDuplicates)
        //        cbs = RemoveDuplicates(cbs);
        //    return cbs.ToList();
        //}

        //public static string ExtractText(this PdfPage page, Rectangle rect)
        //{
        //    var filter = new IEventFilter[1];
        //    filter[0] = new TextRegionEventFilter(rect);
        //    var filteredTextEventListener = new FilteredTextEventListener(new LocationTextExtractionStrategy(), filter);
        //    var str = PdfTextExtractor.GetTextFromPage(page, filteredTextEventListener);
        //    return str;
        //}
        //public static string GetResultantText(this LocationTextExtractionStrategy strategy, Rectangle rect)
        //{
        //    IList<TextChunk> locationalResult = (IList<TextChunk>)locationalResultField.GetValue(strategy);
        //    List<TextChunk> nonMatching = new List<TextChunk>();
        //    foreach (TextChunk chunk in locationalResult)
        //    {
        //        ITextChunkLocation location = chunk.GetLocation();
        //        Vector start = location.GetStartLocation();
        //        Vector end = location.GetEndLocation();
        //        if (!rect.IntersectsLine(start.Get(Vector.I1), start.Get(Vector.I2), end.Get(Vector.I1), end.Get(Vector.I2)))
        //        {
        //            nonMatching.Add(chunk);
        //        }
        //    }
        //    nonMatching.ForEach(c => locationalResult.Remove(c));
        //    try
        //    {
        //        return strategy.GetResultantText();
        //    }
        //    finally
        //    {
        //        nonMatching.ForEach(c => locationalResult.Add(c));
        //    }
        //}

        //static FieldInfo locationalResultField = typeof(LocationTextExtractionStrategy).GetField("locationalResult", BindingFlags.NonPublic | BindingFlags.Instance);


        public static List<CharBox> RemoveDuplicates(IEnumerable<CharBox> cbs)
        {
            List<CharBox> bs = cbs.Where(a => a.R.Width >= 0 && a.R.Height >= 0).ToList();//some symbols are duplicated with negative width and height
            for (int i = 0; i < bs.Count; i++)
                for (int j = bs.Count - 1; j > i; j--)
                {
                    if (Math.Abs(bs[i].R.X - bs[j].R.X) > Settings.Constants.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (Math.Abs(bs[i].R.Y - bs[j].R.Y) > Settings.Constants.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (bs[i].Char != bs[j].Char)
                        continue;
                    bs.RemoveAt(j);
                }
            return bs;
        }

        public static IEnumerable<CharBox> RemoveInvisibles(IEnumerable<CharBox> cbs)
        {
            return cbs.Where(x => !InvisibleCharacters.Contains(x.Char));
        }
        public static readonly string InvisibleCharacters = " \t";

        public class CharBox : Page.CharBox
        {
            //public bool AutoInserted = false;

            public PdfFont Font;
            public float FontSize;
        }
    }
}