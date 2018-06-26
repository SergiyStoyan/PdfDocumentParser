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
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.parser;
using System.Windows.Forms;
using System.Drawing;

namespace Cliver.InvoiceParser
{
    public class Page : IDisposable
    {
        public Page(PageCollection pageCollection, int pageI)
        {
            this.pageCollection = pageCollection;
            this.pageI = pageI;
        }
        int pageI;
        PageCollection pageCollection;

        ~Page()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                    _bitmap = null;
                }
                if (BitmapPreparedForTemplate != null)
                {
                    BitmapPreparedForTemplate.Dispose();
                    _bitmapPreparedForTemplate = null;
                }
                if (imageData != null)
                {
                    //imageData.Dispose();
                    _imageData = null;
                }
                if (charBoxLists != null)
                {
                    //charBoxLists.Dispose();
                    _charBoxLists = null;
                }
            }
        }

        Bitmap bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = Pdf.RenderBitmap(pageCollection.PdfFile, pageI, Settings.General.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        public Settings.Template ActiveTemplate
        {
            set
            {
                if (_activeTemplate == value)
                    return;
                if (_activeTemplate.PagesRotation != value.PagesRotation || _activeTemplate.AutoDeskew != value.AutoDeskew)
                {
                    if (BitmapPreparedForTemplate != null)
                    {
                        BitmapPreparedForTemplate.Dispose();
                        _bitmapPreparedForTemplate = null;
                    }
                }
                floatingAnchorIds2point0.Clear();
                _activeTemplate = value;
            }
            get
            {
                return _activeTemplate;
            }
        }
        Settings.Template _activeTemplate;
        Dictionary<int, System.Drawing.PointF?> floatingAnchorIds2point0 = new Dictionary<int, System.Drawing.PointF?>();

        public System.Drawing.Bitmap GetRectangeFromBitmapPreparedForTemplate(float x, float y, float w, float h)
        {
            return BitmapPreparedForTemplate.Clone(new RectangleF(x, y, w, h), System.Drawing.Imaging.PixelFormat.Undefined);
        }
        public System.Drawing.Bitmap BitmapPreparedForTemplate
        {
            get
            {
                if (_bitmapPreparedForTemplate == null)
                {
                    Bitmap b;
                    if (ActiveTemplate.PagesRotation == Settings.Template.PageRotations.NONE && !ActiveTemplate.AutoDeskew)
                        b = bitmap;
                    else
                    {
                        b = _bitmap.Clone(new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height), System.Drawing.Imaging.PixelFormat.Undefined);
                        //b = ImageRoutines.GetCopy(b);
                        switch (ActiveTemplate.PagesRotation)
                        {
                            case Settings.Template.PageRotations.NONE:
                                break;
                            case Settings.Template.PageRotations.Clockwise90:
                                b.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                                break;
                            case Settings.Template.PageRotations.Clockwise180:
                                b.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                                break;
                            case Settings.Template.PageRotations.Clockwise270:
                                b.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
                                break;
                            default:
                                throw new Exception("Unknown option: " + ActiveTemplate.PagesRotation);
                        }
                        if (ActiveTemplate.AutoDeskew)
                        {
                            using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                            {
                                //image.Density = new PointD(600, 600);
                                //image.AutoLevel();
                                //image.Negate();
                                //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                                //image.Negate();
                                image.Deskew(new ImageMagick.Percentage(10));
                                //image.AutoThreshold(AutoThresholdMethod.OTSU);
                                //image.Despeckle();
                                //image.WhiteThreshold(new Percentage(20));
                                //image.Trim();
                                b = image.ToBitmap();
                            }
                        }
                    }
                    _bitmapPreparedForTemplate = b;
                }
                return _bitmapPreparedForTemplate;
            }
        }
        System.Drawing.Bitmap _bitmapPreparedForTemplate = null;

        public System.Drawing.PointF? GetFloatingAnchorPoint0(int floatingAnchorId)
        {
            System.Drawing.PointF? p;
            if (!floatingAnchorIds2point0.TryGetValue(floatingAnchorId, out p))
            {
                List < System.Drawing.RectangleF > rs = FindFloatingAnchor(ActiveTemplate.FloatingAnchors[floatingAnchorId]);
                if (rs == null || rs.Count < 1)
                    p = null;
                else
                    p = new PointF(rs[0].X, rs[0].Y);
                floatingAnchorIds2point0[floatingAnchorId] = p;
            }
            return p;
        }

        public List<System.Drawing.RectangleF> FindFloatingAnchor(Settings.Template.FloatingAnchor fa)
        {
            if (fa == null)
                return null;

            switch (fa.ValueType)
            {
                case Settings.Template.ValueTypes.PdfText:
                    List<Settings.Template.FloatingAnchor.PdfTextElement.CharBox> ses = ((Settings.Template.FloatingAnchor.PdfTextElement)fa.Get()).CharBoxs;
                    List<Pdf.BoxText> bts = new List<Pdf.BoxText>();
                    foreach (Pdf.BoxText bt0 in charBoxLists.Where(a => a.Text == ses[0].Char))
                    {
                        bts.Clear();
                        bts.Add(bt0);
                        for (int i = 1; i < ses.Count; i++)
                        {
                            float x = bt0.R.X + ses[i].Rectangle.X - ses[0].Rectangle.X;
                            float y = bt0.R.Y + ses[i].Rectangle.Y - ses[0].Rectangle.Y;
                            foreach (Pdf.BoxText bt in charBoxLists.Where(a => a.Text == ses[i].Char))
                            {
                                if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                                    continue;
                                if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                                    continue;
                                if (bts.Contains(bt))
                                    continue;
                                bts.Add(bt);
                            }
                        }
                        if (bts.Count == ses.Count)
                        {
                            //System.Drawing.PointF point0 = new System.Drawing.PointF(bts[0].R.X, bts[0].R.Y);
                            //if (findFloatingAnchorSecondaryElements(point0, fa))
                            //    return point0;
                            return bts.Select(x => x.R).ToList();
                        }
                    }
                    return null;
                case Settings.Template.ValueTypes.OcrText:
                    return null;
                case Settings.Template.ValueTypes.ImageData:
                    ImageData id = (ImageData)fa.Get();
                        System.Drawing.PointF? p0 = id.FindWithinImage(imageData);
                        if (p0 == null)
                            return null;
                        System.Drawing.PointF point0 = (PointF)p0;
                        //if (findFloatingAnchorSecondaryElements((PointF)point0, fa))
                        //    return point0;
                        return new List<RectangleF> { new RectangleF(point0.X, point0.Y, imageData.Width, imageData.Height) };
                default:
                    throw new Exception("Unknown option: " + fa.ValueType);
            }
        }

        //bool findFloatingAnchorSecondaryElements(System.Drawing.PointF point0, Settings.Template.FloatingAnchor fa)
        //{
        //    for (int i = 1; i < fa.Elements.Count; i++)
        //    {
        //        if (!findFloatingAnchorElement(point0, fa.Elements[i]))
        //            return false;
        //    }
        //    return true;
        //}
        //bool findFloatingAnchorElement(System.Drawing.PointF point0, Settings.Template.FloatingAnchor.Element e)
        //{
        //    switch (e.ElementType)
        //    {
        //        case Settings.Template.ValueTypes.PdfText:
        //            {
        //                List<Settings.Template.FloatingAnchor.PdfTextElement.CharBox> ses = ((Settings.Template.FloatingAnchor.PdfTextElement)e.Get()).CharBoxs;
        //                List<Pdf.BoxText> bts = new List<Pdf.BoxText>();

        //                bts.Clear();
        //                for (int i = 0; i < ses.Count; i++)
        //                {
        //                    float x = point0.X + ses[i].Rectangle.X - ses[0].Rectangle.X;
        //                    float y = point0.Y + ses[i].Rectangle.Y - ses[0].Rectangle.Y;
        //                    foreach (Pdf.BoxText bt in charBoxLists.Where(a => a.Text == ses[i].Char))
        //                    {
        //                        if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
        //                            continue;
        //                        if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
        //                            continue;
        //                        if (bts.Contains(bt))
        //                            continue;
        //                        bts.Add(bt);
        //                    }
        //                }
        //                return bts.Count == ses.Count;
        //            }
        //        case Settings.Template.ValueTypes.OcrText:
        //            {
        //                return true;
        //            }
        //        case Settings.Template.ValueTypes.ImageData:
        //            {
        //                ImageData id = (ImageData)e.Get();
        //                return id.ImageIsSimilar(new ImageData(GetRectangeFromBitmapPreparedForTemplate(new Settings.Template.RectangleF(point0.X, point0.Y, id.Width, id.Height))));
        //            }
        //        default:
        //            throw new Exception("Unknown option: " + e.ElementType);
        //    }
        //}

        ImageData imageData
        {
            get
            {
                if (_imageData == null)
                    _imageData = new ImageData(bitmap);
                return _imageData;
            }
        }
        ImageData _imageData;

        List<Pdf.BoxText> charBoxLists
        {
            get
            {
                if (_charBoxLists == null)
                {
                    var bts = pageCollection.PdfReader.GetCharacterTextChunks(pageI).Select(x => new Pdf.BoxText
                    {
                        R = new System.Drawing.RectangleF
                        {
                            X = x.StartLocation[Vector.I1],
                            Y = pageCollection.PdfReader.GetPageSize(pageI).Height - x.EndLocation[Vector.I2],
                            Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
                            Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
                        },
                        Text = x.Text
                    });
                    _charBoxLists = bts.ToList();
                }
                return _charBoxLists;
            }
        }
        List<Pdf.BoxText> _charBoxLists;

        public bool IsInvoiceFirstPage()
        {
            string error;
            return IsInvoiceFirstPage(out error);
        }

        public bool IsInvoiceFirstPage(out string error)
        {
            foreach (Settings.Template.Mark m in ActiveTemplate.InvoiceFirstPageRecognitionMarks)
            {
                System.Drawing.PointF point0 = new System.Drawing.PointF(0, 0);
                if (m.FloatingAnchorId >= 0)
                {
                    System.Drawing.PointF? p0;
                    p0 = GetFloatingAnchorPoint0(m.FloatingAnchorId);
                    if (p0 != null)
                        point0 = (PointF)p0;
                    else
                    {
                        error = "FloatingAnchor[" + m.FloatingAnchorId + "] not found.";
                        return false;
                    }
                }
                Settings.Template.RectangleF r = new Settings.Template.RectangleF(m.Rectangle.X + point0.X, m.Rectangle.Height + point0.Y, m.Rectangle.Width, m.Rectangle.Height);

                switch (m.ValueType)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        {
                            //string t = pr.ExtractText(pageI, r.X, Height - r.Y - r.Height, r.Width, r.Height);
                            string t2 = Pdf.GetTextByTopLeftCoordinates(charBoxLists, r.X, r.Y, r.Width, r.Height);
                            t2 = FieldPreparation.Normalize(t2);
                            string t1 = FieldPreparation.Normalize(m.Value);
                            if (t1 == t2)
                                break;
                                error = "InvoiceFirstPageRecognitionMark[" + ActiveTemplate.InvoiceFirstPageRecognitionMarks.IndexOf(m) + "]:\r\n" + t2 + "\r\n <> \r\n" + t1;
                                return false;
                        }
                    case Settings.Template.ValueTypes.OcrText:
                        {
                            string t2 = TesseractW.This.GetText(BitmapPreparedForTemplate, r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);
                            t2 = FieldPreparation.Normalize(t2);
                            string t1 = FieldPreparation.Normalize(m.Value);
                            if (t1 == t2)
                                break;
                            error = "InvoiceFirstPageRecognitionMark[" + ActiveTemplate.InvoiceFirstPageRecognitionMarks.IndexOf(m) + "]:\r\n" + t2 + "\r\n <> \r\n" + t1;
                            return false;
                        }
                    case Settings.Template.ValueTypes.ImageData:
                        {
                            System.Drawing.Bitmap b = GetRectangeFromBitmapPreparedForTemplate(r.X, r.Y, r.Width, r.Height);
                            ImageData id = ImageData.Deserialize(m.Value);
                            if (id.ImageIsSimilar(new ImageData(b)))
                                break;
                            error = "InvoiceFirstPageRecognitionMark[" + ActiveTemplate.InvoiceFirstPageRecognitionMarks.IndexOf(m) + "]: image is not similar.";
                            return false;
                        }
                    default:
                        throw new Exception("Unknown option: " + m.ValueType);
                }
            }
            error = null;
            return true;
        }

        public string GetFieldValue(Settings.Template.Field f)
        {
            string t = null;

            System.Drawing.PointF point0 = new System.Drawing.PointF(0, 0);
            if (f.FloatingAnchorId >= 0)
            {
                System.Drawing.PointF? p0;
                p0 = GetFloatingAnchorPoint0(f.FloatingAnchorId);
                if (p0 != null)
                    point0 = (PointF)p0;
                //else
                //    Log.Main.Warning("");
            }
            Settings.Template.RectangleF r = new Settings.Template.RectangleF(f.Rectangle.X + point0.X, f.Rectangle.Height + point0.Y, f.Rectangle.Width, f.Rectangle.Height);

            switch (f.ValueType)
            {
                case Settings.Template.ValueTypes.PdfText:
                    //string t = pr.ExtractText(pageI, r.X, Height - r.Y - r.Height, r.Width, r.Height);
                    t = Pdf.GetTextByTopLeftCoordinates(charBoxLists, r.X, r.Y, r.Width, r.Height);
                    break;
                case Settings.Template.ValueTypes.OcrText:
                    t = TesseractW.This.GetText(BitmapPreparedForTemplate, r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);
                    break;
                case Settings.Template.ValueTypes.ImageData:
                    ImageData id = new ImageData(GetRectangeFromBitmapPreparedForTemplate(r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio));
                    return id.GetAsString();
                default:
                    throw new Exception("Unknown option: " + f.ValueType);
            }
            return prepareField(FieldPreparation.Normalize(t));
        }

        Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();

        public float Height;

        static string prepareField(string f)
        {
            return Regex.Replace(f, @"\-", "");
        }
    }
}