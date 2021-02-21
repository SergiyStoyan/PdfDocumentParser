////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        http://www.cliversoft.com
////********************************************************************************************
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing;
//using Tesseract;

//namespace Cliver.PdfDocumentParser
//{
//    partial class Ocr
//    {
//        internal class Page
//        {
//            public void Dispose()
//            {
//                lock (this)
//                {
//                    if (_engine != null)
//                    {
//                        _engine.Dispose();
//                        _engine = null;
//                    }
//                }
//            }

//            public int DetectOrientationAngle(Bitmap b, out float confidence)
//            {
//                using (var page = engine.Process(b, PageSegMode.OsdOnly))
//                {
//                    page.DetectBestOrientation(out int o, out confidence);
//                    return o;
//                }
//            }

//            public string GetTextSurroundedByRectangle(Bitmap b, RectangleF r)
//            {
//                r = new RectangleF(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
//                r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
//                if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
//                    return null;
//                using (var page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), PageSegMode.SingleBlock))
//                {
//                    return page.GetText();
//                }
//            }

//            public string GetHtml(Bitmap b)
//            {
//                using (var page = engine.Process(b, PageSegMode.SingleBlock))
//                {
//                    return page.GetHOCRText(0, false);
//                }
//            }

//            public List<CharBox> GetCharBoxs(Bitmap b)
//            {
//                List<CharBox> cbs = new List<CharBox>();
//                using (var page = engine.Process(b, PageSegMode.SingleBlock))
//                {
//                    //string t = page.GetHOCRText(1, true);
//                    //var dfg = page.GetThresholdedImage();                        
//                    //Tesseract.Orientation o;
//                    //float c;
//                    // page.DetectBestOrientation(out o, out c);
//                    //  var l = page.AnalyseLayout();
//                    //var ti =   l.GetBinaryImage(Tesseract.PageIteratorLevel.Para);
//                    //Tesseract.Rect r;
//                    // l.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
//                    using (var i = page.GetIterator())
//                    {
//                        //int j = 0;
//                        //i.Begin();
//                        //do
//                        //{
//                        //    bool g = i.IsAtBeginningOf(Tesseract.PageIteratorLevel.Block);
//                        //    bool v = i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
//                        //    var bt = i.BlockType;
//                        //    //if (Regex.IsMatch(bt.ToString(), @"image", RegexOptions.IgnoreCase))
//                        //    //{
//                        //    //    //i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block,out r);
//                        //    //    Tesseract.Pix p = i.GetBinaryImage(Tesseract.PageIteratorLevel.Block);
//                        //    //    Bitmap b = Tesseract.PixConverter.ToBitmap(p);
//                        //    //    b.Save(Log.AppDir + "\\test" + (j++) + ".png", System.Drawing.Imaging.ImageFormat.Png);
//                        //    //}
//                        //} while (i.Next(Tesseract.PageIteratorLevel.Block));
//                        //do
//                        //{
//                        //    do
//                        //    {
//                        //        do
//                        //        {
//                        //            do
//                        //        {
//                        do
//                        {
//                            //if (i.IsAtBeginningOf(PageIteratorLevel.Block))
//                            //{
//                            //}
//                            //if (i.IsAtBeginningOf(PageIteratorLevel.Para))
//                            //{
//                            //}
//                            //if (i.IsAtBeginningOf(PageIteratorLevel.TextLine))
//                            //{
//                            //}

//                            Rect r;
//                            if (i.TryGetBoundingBox(PageIteratorLevel.Symbol, out r))
//                            {
//                                if (i.IsAtBeginningOf(PageIteratorLevel.Word))
//                                {
//                                    //if (i.IsAtBeginningOf(PageIteratorLevel.Para))
//                                    //{
//                                    //    cbs.Add(new CharBox
//                                    //    {
//                                    //        Char = "\r\n",
//                                    //        AutoInserted = true,
//                                    //        R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
//                                    //    });
//                                    //}//seems to work not well

//                                    //cbs.Add(new CharBox//worked well before autoinsert was moved
//                                    //{
//                                    //    Char = " ",
//                                    //    AutoInserted = true,
//                                    //    R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
//                                    //});
//                                }
//                                cbs.Add(new CharBox
//                                {
//                                    Char = i.GetText(PageIteratorLevel.Symbol),
//                                    R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
//                                });
//                            }
//                        } while (i.Next(PageIteratorLevel.Symbol));
//                        //            } while (i.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));
//                        //        } while (i.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
//                        //    } while (i.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
//                        //} while (i.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
//                    }
//                }
//                return cbs;
//            }
//        }
//    }
//}
