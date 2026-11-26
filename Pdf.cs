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
                    string bufferFileDir = System.IO.Path.GetTempPath() + Log.ProgramName;
                    Directory.CreateDirectory(bufferFileDir);
                    string bufferFile = bufferFileDir + "\\buffer.png";
                    p.StartInfo.Arguments = "-dNOPROMPT -r" + resolution + " -dDownScaleFactor=" + dDownScaleFactor + " -dBATCH -dFirstPage=" + pageI + " -dLastPage=" + pageI + " -sDEVICE=png16m -dNOPAUSE -sOutputFile=\"" + bufferFile + "\" -q \"" + pdfFile + "\"";
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.UseShellExecute = false;
                    p.Start();
                    p.WaitForExit();
                    string stderr = p.StandardError.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(stderr))
                        Log.Error(stderr);
                    if (!File.Exists(bufferFile))
                        throw new Exception("Could not create bufferFile " + bufferFile);
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

        static public List<CharBox> GetCharBoxsFromPage(PdfDocument pdfDocument, int pageI, bool removeDuplicates)
        {
            //var r = GetPageSize( pdfDocument,pageI);
            //CharBoxExtractionStrategy s = new CharBoxExtractionStrategy(new System.Drawing.RectangleF(0, 0, r.Width, r.Height));
            //PdfTextExtractor.GetTextFromPage(pdfReader, pageI, s);

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
                        Y = (float)r.Bottom,
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
          public  Letter Letter;
        }
    }
}