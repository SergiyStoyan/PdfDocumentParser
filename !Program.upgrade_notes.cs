//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using System.Drawing.Imaging;
using System.Threading.Tasks;
using Sdcb.SimdPaddleOCR;
using Sdcb.SimdPaddleOCR.Models.ChineseV6Medium;
using PdfiumViewer;
using System.Collections.Generic;

/*
 * using PdfPig; - for extracting native pdf characters
 * 
 */

namespace Cliver.PdfDocumentParser
{
    public class Program
    {
        public static void Main()//trigger Program()
        {
            try
            {


                string pdfPath = @"C:\temp\_failed_invoices\I  111252.pdf";

                var p = OcrHelper.GetBitmapsFromPdf(pdfPath, 500)[0];

                string t = OcrHelper.RecognizeImage(p);
            }
            catch (Exception e)
            {

            }
        }
    }

    public class OcrHelper
    {
        public static List<Bitmap> GetBitmapsFromPdf(string pdfPath, int dpi = 300)
        {
            var bitmaps = new List<Bitmap>();

            using (var document = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    // Renders directly to a System.Drawing.Image at the requested DPI
                    using (Image pageImage = document.Render(i, dpi, dpi, PdfRenderFlags.CorrectFromDpi))
                    {
                        bitmaps.Add(new Bitmap(pageImage));
                    }
                }
            }

            return bitmaps;
        }
        public static string RecognizeImage(Bitmap bitmap)
        {
            // Load embedded models once (no temp files extracted to disk)
            using (PaddleOcrAll ocr = PaddleOcrAll.Load(ChineseV6MediumModels.Default))
            {
                // Ensure bitmap is 24-bit BGR format (SimdPaddleOCR's native default)
                using (Bitmap bgrBitmap = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                    ? (Bitmap)bitmap.Clone()
                    : bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb))
                {
                    var rect = new Rectangle(0, 0, bgrBitmap.Width, bgrBitmap.Height);
                    BitmapData bmpData = bgrBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    try
                    {
                        // Copy locked pixel buffer into a managed byte array
                        int byteCount = Math.Abs(bmpData.Stride) * bgrBitmap.Height;
                        byte[] pixels = new byte[byteCount];
                        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);

                        // Run OCR passing width, height, and row stride
                        PaddleOcrResult result = ocr.Run(
                            pixels,
                            bgrBitmap.Width,
                            bgrBitmap.Height,
                            bmpData.Stride,
                            ImagePixelFormat.Bgr24);

                        return result.Text;
                    }
                    finally
                    {
                        bgrBitmap.UnlockBits(bmpData);
                    }
                }
            }
        }
    }
}