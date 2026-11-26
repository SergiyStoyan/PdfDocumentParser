//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using PDFtoImage;
using SkiaSharp;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// PDF routines
    /// </summary>
    public static partial class Pdf
    {
        public static System.Drawing.Size GetPageSize(PdfDocument pdfDocument, int pageI)
        {
            var r = pdfDocument.GetPage(pageI);
            return new System.Drawing.Size((int)r.Width, (int)r.Height);
        }

        static public System.Drawing.Bitmap RenderBitmap(string pdfFile, int pageI, int resolution, bool byFile = false)
        {
            var options = new RenderOptions(Dpi: resolution)
            {
                BackgroundColor = SKColors.White, // Ensures transparent pages render on white instead of black
                WithAnnotations = true
            };
            using var pdfStream = File.OpenRead(pdfFile);
            using SKBitmap skBitmap = Conversion.ToImage(pdfStream, page: pageI - 1/*0-based*/, options: options);
            using var imageStream = new MemoryStream();
            skBitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
            imageStream.Position = 0;
            using var tempBitmap = new Bitmap(imageStream);
            return new Bitmap(tempBitmap);
        }

        static public List<CharBox> GetCharBoxsFromPage(PdfDocument pdfDocument, int pageI, bool removeDuplicates)
        {
            var p = GetPageSize(pdfDocument, pageI);
            List<CharBox> cbs = new List<CharBox>();
            var page = pdfDocument.GetPage(pageI);
            foreach (Letter c in page.Letters)
            {
                var r = c.BoundingBox;
                cbs.Add(new CharBox
                {
                    Char = c.Value,
                    R = new System.Drawing.RectangleF
                    {
                        X = (float)r.Left,
                        Y = (float)(p.Height - r.Top),
                        Width = (float)r.Width,
                        Height = (float)r.Height,
                    },
                    Letter = c
                });
            }
            if (removeDuplicates)
                cbs = RemoveDuplicates(cbs);
            return cbs;
        }

        public static List<string> GetTextLinesSurroundedByRectangle(IEnumerable<CharBox> cbs, System.Drawing.RectangleF r, TextAutoInsertSpace textAutoInsertSpace)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            return Page.GetLines(cbs, textAutoInsertSpace, null).Select(a => a.GetString()).ToList();
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(IEnumerable<CharBox> cbs, System.Drawing.RectangleF r, bool excludeInvisibleCharacters = false)
        {
            //cbs = RemoveDuplicates(cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)));
            cbs = cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/));
            if (excludeInvisibleCharacters)
                return cbs.Where(a => !InvisibleCharacters.Contains(a.Char)).ToList();
            return cbs.ToList();
        }

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
            public Letter Letter;
        }
    }
}