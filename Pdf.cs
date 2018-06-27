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

namespace Cliver.InvoiceParser
{
    public static partial class Pdf
    {
        //public Pdf(string pdf)
        //{
        //    PdfReader.unethicalreading = true;
        //    pr = new PdfReader(pdf);

        //}
        //PdfReader pr;

        //~Pdf()
        //{
        //    if (pr != null)
        //    {
        //        pr.Close();
        //        pr = null;
        //    }
        //} 

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

        public static string GetTextByTopLeftCoordinates(List<BoxText> bts, float x, float y, float w, float h)
        {
            System.Drawing.RectangleF d = new System.Drawing.RectangleF { X = x, Y = y, Width = w, Height = h };
            bts = RemoveDuplicatesAndOrder(bts.Where(a => (d.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)));
            StringBuilder sb = new StringBuilder(bts.Count > 0 ? bts[0].Text : "");
            for (int i = 1; i < bts.Count; i++)
            {
                if (Math.Abs(bts[i - 1].R.Y - bts[i].R.Y) > bts[i - 1].R.Height / 2)
                    sb.Append("\r\n");
                else if (Math.Abs(bts[i - 1].R.Right - bts[i].R.X) > Math.Min(bts[i - 1].R.Width, bts[i].R.Width) / 2)
                    sb.Append(" ");
                sb.Append(bts[i].Text);
            }
            return sb.ToString();
        }

        public static List<BoxText> GetBoxTextsSurroundedByRectangle(List<BoxText> bts, System.Drawing.RectangleF r)
        {
            return bts.Where(a => /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R)).ToList();
        }

        public static List<BoxText> RemoveDuplicatesAndOrder(IEnumerable<BoxText> bts)
        {
            List<BoxText> bs = bts.Where(a => a.R.Width >= 0 && a.R.Height >= 0).ToList();//some symbols are duplicated with negative width anf height
            for (int i = 0; i < bs.Count; i++)
                for (int j = bs.Count - 1; j > i; j--)
                {
                    if (Math.Abs(bs[i].R.X - bs[j].R.X) > Settings.General.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (Math.Abs(bs[i].R.Y - bs[j].R.Y) > Settings.General.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (bs[i].Text != bs[j].Text)
                        continue;
                    bs.RemoveAt(j);
                }
            return bs.OrderBy(a => a.R.Y).OrderBy(a => a.R.X).ToList();
        }

        public static List<BoxText> GetCharBoxsFromPage(PdfReader pdfReader, int pageI)
        {
            var bts = pdfReader.GetCharacterTextChunks(pageI).Select(x => new Pdf.BoxText
            {
                R = new System.Drawing.RectangleF
                {
                    X = x.StartLocation[Vector.I1],
                    Y = pdfReader.GetPageSize(pageI).Height - x.EndLocation[Vector.I2],
                    Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
                    Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
                },
                Text = x.Text
            });
            return bts.ToList();
        }

        public class BoxText
        {
            public System.Drawing.RectangleF R;
            public string Text;
        }
    }

    //public class BitmapCollection : HandyDictionary<int, System.Drawing.Bitmap>
    //{
    //    public BitmapCollection(Func<int, System.Drawing.Bitmap> get_page_bitmap) : base(get_page_bitmap)
    //    {
    //    }

    //    public System.Drawing.Bitmap Get(int page_i, Settings.Template.RectangleF r = null)
    //    {
    //        lock (this)
    //        {
    //            System.Drawing.Bitmap b = base[page_i];
    //            if (r != null)
    //                b = b.Clone(r.GetSystemRectangle(), System.Drawing.Imaging.PixelFormat.Undefined);
    //            //b = ImageRoutines.GetCopy(b, r.GetSystem());
    //            return b;

    //            //switch (pages_rotation)
    //            //{
    //            //    case Settings.Template.PageRotations.NONE:
    //            //        r_ = r == null ? new System.Drawing.RectangleF(0, 0, b.Width, b.Height) : r.Convert();
    //            //        return b.Clone(r_, System.Drawing.Imaging.PixelFormat.Undefined);
    //            //    case Settings.Template.PageRotations.Clockwise90:
    //            //        r_ = r == null ? new System.Drawing.RectangleF(0, 0, b.Width, b.Height) : new System.Drawing.RectangleF(r.Y, b.Height - r.Width - r.X, r.Height, r.Width);
    //            //        //b = ImageRoutines.GetCopy(b, r);
    //            //        b = b.Clone(r_, System.Drawing.Imaging.PixelFormat.Undefined);
    //            //        //b.Save(Log.AppDir + "\\test.png", System.Drawing.Imaging.ImageFormat.Png);
    //            //        b.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
    //            //        //b.Save(Log.AppDir + "\\test1.png", System.Drawing.Imaging.ImageFormat.Png);
    //            //        //if(ReplaceWithConverted)
    //            //        //    base[]
    //            //        return b;
    //            //    case Settings.Template.PageRotations.Clockwise180:
    //            //        r_ = r == null ? new System.Drawing.RectangleF(0, 0, b.Width, b.Height) : new System.Drawing.RectangleF(b.Width - r.Width - r.X, b.Height - r.Height - r.Y, r.Width, r.Height);
    //            //        //b = ImageRoutines.GetCopy(b, r);
    //            //        b = b.Clone(r_, System.Drawing.Imaging.PixelFormat.Undefined);
    //            //        b.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
    //            //        return b;
    //            //    case Settings.Template.PageRotations.Clockwise270:
    //            //        r_ = r == null ? new System.Drawing.RectangleF(0, 0, b.Width, b.Height) : new System.Drawing.RectangleF(b.Width - r.Y - r.Height, r.X, r.Height, r.Width);
    //            //        //b = ImageRoutines.GetCopy(b, r);
    //            //        b = b.Clone(r_, System.Drawing.Imaging.PixelFormat.Undefined);
    //            //        b.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
    //            //        return b;
    //            //    default:
    //            //        throw new Exception("Unknown option: " + pages_rotation);
    //            //}
    //        }
    //    }
    //    static public System.Drawing.Bitmap GetPageBitmap(string pdfFile, int page_i, Settings.Template.PageRotations pagesRotation, bool autoDeskew)
    //    {
    //        System.Drawing.Bitmap b = Pdf.RenderBitmap(pdfFile, page_i, Settings.General.PdfPageImageResolution);
    //        if (pagesRotation != Settings.Template.PageRotations.NONE || autoDeskew)
    //        {
    //            b = b.Clone(new System.Drawing.Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.PixelFormat.Undefined);
    //            //b = ImageRoutines.GetCopy(b);
    //            b = BitmapCollection.GetProcessed(b, pagesRotation, autoDeskew);
    //        }
    //        return b;
    //    }
    //    static public System.Drawing.Bitmap GetProcessed(System.Drawing.Bitmap b, Settings.Template.PageRotations pages_rotation, bool autoDeskew)
    //    {
    //        switch (pages_rotation)
    //        {
    //            case Settings.Template.PageRotations.NONE:
    //                break;
    //            case Settings.Template.PageRotations.Clockwise90:
    //                b.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
    //                break;
    //            case Settings.Template.PageRotations.Clockwise180:
    //                b.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
    //                break;
    //            case Settings.Template.PageRotations.Clockwise270:
    //                b.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
    //                break;
    //            default:
    //                throw new Exception("Unknown option: " + pages_rotation);
    //        }

    //        if (autoDeskew)
    //        {
    //            using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
    //            {
    //                //image.Density = new PointD(600, 600);
    //                //image.AutoLevel();
    //                //image.Negate();
    //                //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
    //                //image.Negate();
    //                image.Deskew(new ImageMagick.Percentage(10));
    //                //image.AutoThreshold(AutoThresholdMethod.OTSU);
    //                //image.Despeckle();
    //                //image.WhiteThreshold(new Percentage(20));
    //                //image.Trim();
    //                b = image.ToBitmap();
    //            }
    //        }

    //        return b;
    //    }
    //}
}