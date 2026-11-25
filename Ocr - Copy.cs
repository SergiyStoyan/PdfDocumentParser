//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Threading.Tasks;
//using Sdcb.SimdPaddleOCR;
//using Sdcb.SimdPaddleOCR.ModelProvider;
//using Sdcb.SimdPaddleOCR.Models.ChineseV6Medium;

//public class PageOrientationService : IDisposable
//{
//    private readonly PaddleOcrDetector _detector;
//    private readonly PaddleOcrAll _ocrNoFlip;

//    private PageOrientationService(PaddleOcrDetector detector, PaddleOcrAll ocrNoFlip)
//    {
//        _detector = detector;
//        _ocrNoFlip = ocrNoFlip;
//    }

//    public static async Task<PageOrientationService> CreateAsync()
//    {
//        var model = ChineseV6MediumModels.Default;
//        var detector = new PaddleOcrDetector(model.Detection.OpenRead());

//        // Omit the Classification model so upside-down (180°) text is not auto-flipped
//        // and scores noticeably lower than upright text
//        var noFlipBundle = new PaddleOcrModelBundle(
//            model.Name,
//            model.LanguageCode,
//            model.Detection,
//            model.Recognition,
//            model.Dictionary
//            );
//        var ocrNoFlip = PaddleOcrAll.Load(noFlipBundle);

//        return new PageOrientationService(detector, ocrNoFlip);
//    }

//    /// <summary>
//    /// Detects clockwise page rotation (0, 90, 180, or 270 degrees) and rotates the Bitmap upright.
//    /// </summary>
//    public int DetectAndFixOrientation(Bitmap bitmap)
//    {
//        // 1. Detect boxes to check if the page is horizontal (0°/180°) or sideways (90°/270°)
//        byte[] pixels = ExtractBgraPixels(bitmap, out int stride);
//        PaddleOcrDetectionResult detection = _detector.Detect(
//            pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);

//        var boxes = detection?.Boxes;
//        if (boxes == null || boxes.Length == 0)
//            return 0;

//        int baseAngle = 0;
//        int verticalCount = boxes.Count(b => b.Size.Height > b.Size.Width * 1.2f);
//        if (verticalCount > boxes.Length / 2)
//        {
//            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
//            baseAngle = 90;
//            pixels = ExtractBgraPixels(bitmap, out stride);
//        }

//        // 2. Score at baseAngle (0° or 90°) using RecognitionScore
//        PaddleOcrResult firstPass = _ocrNoFlip.Run(
//            pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);
//        float firstScore = firstPass.Lines.Length > 0
//            ? firstPass.Lines.Average(l => l.RecognitionScore)
//            : 0f;

//        // 3. Rotate 180° and score again to check if the page was upside-down
//        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
//        pixels = ExtractBgraPixels(bitmap, out stride);
//        PaddleOcrResult secondPass = _ocrNoFlip.Run(
//            pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);
//        float secondScore = secondPass.Lines.Length > 0
//            ? secondPass.Lines.Average(l => l.RecognitionScore)
//            : 0f;

//        if (secondScore > firstScore)
//        {
//            // Keep the 180°-rotated bitmap; total rotation was (baseAngle + 180) % 360
//            return (baseAngle + 180) % 360;
//        }

//        // First angle was better — rotate back 180°
//        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
//        return baseAngle;
//    }

//    private static byte[] ExtractBgraPixels(Bitmap bitmap, out int stride)
//    {
//        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
//        BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//        try
//        {
//            stride = data.Stride;
//            byte[] buffer = new byte[stride * bitmap.Height];
//            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
//            return buffer;
//        }
//        finally
//        {
//            bitmap.UnlockBits(data);
//        }
//    }

//    public void Dispose()
//    {
//        _detector?.Dispose();
//        _ocrNoFlip?.Dispose();
//    }
//}
