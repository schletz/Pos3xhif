using OpenCvSharp;
using OpenCvSharp.XPhoto;
using OpenCvSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenCvDemo.Application.Services
{
    public class ImageProcessingService
    {
        private void EnsureFileExists(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new ArgumentException($"File {filename} dosen't exist.");
            }
        }

        /// <summary>
        /// Erkennt einzelne Fotos von einem Scan oder einer Buchseite.
        /// </summary>
        /// <param name="filename">Einzulesende Bilddatei.</param>
        /// <param name="sharpen">Schärfung der Bilder. Ist für Fotos gut, für Buch- oder Magazinseiten wegen des Druckrasters aber schlecht.</param>
        /// <returns></returns>
        public int DetectPhotos(string filename, bool sharpen = false)
        {
            EnsureFileExists(filename);

            // Weißabgleich der erkannten Bilder.
            using var wb = SimpleWB.Create();
            Mat kernel = new Mat<double>(3, 3, new double[] { 0, -1, 0, -1, 5, -1, 0, -1, 0 });

            int found = 0;
            foreach (var image in ExtractImages(filename, true))
            {
                found++;
                var filteredImage = new Mat();
                // Weißabgleich. Gerade bei analogen Fotos erforderlich, da es Tageslicht- und Kunstlichtfilme gab.
                wb.BalanceWhite(image, filteredImage);
                // Schärfen des Bildes
                // https://learnopencv.com/image-filtering-using-convolution-in-opencv/
                if (sharpen)
                    filteredImage = filteredImage.Filter2D(-1, kernel, new Point(-1, -1), 0, BorderTypes.Default);
                filteredImage.ImWrite($"cropped{found}.jpg");
            }

            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
            return found;
        }

        /// <summary>
        /// Erkennt den Text mit Hilfe von Tesseract OCR. Dafür muss folgendes gegeben sein:
        /// -) Die Datei deu.traineddata liegt im Projektverzeichnis und wird mit "copy if newer" ins Ausgabeverzeichnis kopiert.
        ///    https://tesseract-ocr.github.io/tessdoc/Data-Files.html und https://github.com/tesseract-ocr/tessdata_fast
        ///    bieten trainierte Dateien an.
        /// -) Die CPU unterstützt AVX (mit HWDIAG zu ermitteln)
        /// </summary>
        public string GetText(string filename)
        {
            EnsureFileExists(filename);
            using var ocr = OCRTesseract.Create(".", "deu", "", 1);
            using var mat = new Mat(filename)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(180, 255, ThresholdTypes.Binary);

            ocr.Run(mat, out var text, out var componentRects, out var componentTexts, out var componentConfidences, ComponentLevels.TextLine);
            return text;
        }

        public void EdgeDetection(string filename)
        {
            EnsureFileExists(filename);
            using var src = new Mat(filename, ImreadModes.Grayscale);
            using var dst = new Mat();

            Cv2.Canny(src, dst, 50, 200);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }
        }

        /// <summary>
        /// Extrahiert ein Rechteck aus dem Originalbild und dreht es gerade.
        /// </summary>
        private Mat ExtractImage(Mat mat, RotatedRect rect, int crop = 0)
        {
            Rect boundingRect = rect.BoundingRect();
            var center = new Point2f(boundingRect.Width / 2, boundingRect.Height / 2);
            // Ist das Foto am Bildrand, kann das BoundingRect über die Grenze des Bildes gehen.
            boundingRect.Width = Math.Min(boundingRect.Width, mat.Cols - boundingRect.X);
            boundingRect.Height = Math.Min(boundingRect.Height, mat.Rows - boundingRect.Y);
            var newImage = mat[boundingRect].Clone();
            // Der Winkel ist immer entgegen dem Uhrzeigersinn. Wir wollen es zur nächstgelegenen
            // Achse (x oder y) drehen.
            float angle = rect.Angle > 45 ? rect.Angle - 90 : rect.Angle;
            var excess = rect.Angle > 45 && rect.Angle < 90
                ? (width: boundingRect.Width - (int)rect.Size.Height, height: boundingRect.Height - (int)rect.Size.Width)
                : (width: boundingRect.Width - (int)rect.Size.Width, height: boundingRect.Height - (int)rect.Size.Height);
            if (excess.height - 2 * crop < 0 || excess.width - 2 * crop < 0)
                crop = 0;
            // Generiert die Matrix in der Form
            // +---------------------+
            // | cos(x)   -sin(x)  0 |
            // | sin(x)    cos(x)  0 |
            // +---------------------+
            var rot = Cv2.GetRotationMatrix2D(center, angle, 1);
            // Die Verschiebung in die 3. Spalte schreiben.
            rot.At<double>(0, 2) -= excess.width / 2 + crop;
            rot.At<double>(1, 2) -= excess.height / 2 + crop;
            Cv2.WarpAffine(newImage, newImage, rot, boundingRect.Size);
            return newImage[0, newImage.Rows - excess.height - 2 * crop, 0, newImage.Cols - excess.width - 2 * crop];
        }

        /// <summary>
        /// Extrahiert die ausgerichtete Matrix der Einzelbilder ohne Bildverarbeitung.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="showImages">Zeigt die Bilder und die erkannten Bereiche an.</param>
        /// <returns></returns>
        private IEnumerable<Mat> ExtractImages(string filename, bool showImages = false)
        {
            using var src = new Mat(filename);
            using var displayImage = showImages ? src.Clone() : new Mat();
            // Minimale Größe eines extrahierten Bildes. Verhindert die Erkennung von weißen Stellen im Foto.
            int minSize = (int)(src.Rows * src.Cols * 0.05);
            // maximale Größe eines extrahierten Bildes. Verhindert die Erkennung der gescannten Seite als Gesamtes.
            int maxSize = (int)(src.Rows * src.Cols * 0.95);
            double scale = 1000 / (double)src.Rows;

            var gray = src.Channels() == 3 ? src.CvtColor(ColorConversionCodes.BGR2GRAY) : src.Clone();
            var threshold = gray.Threshold(210, 255, ThresholdTypes.BinaryInv);

            threshold.FindContours(
                contours: out var contours,
                hierarchy: out var hierarchyIndexes,
                mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);

            foreach (var c in contours)
            {
                var rect = Cv2.MinAreaRect(c);
                float size = rect.Size.Width * rect.Size.Height;
                if (size > minSize && size < maxSize)
                {
                    var boundingRect = rect.BoundingRect();
                    var points = Enumerable.Repeat(rect.Points().Select(p => new Point(p.X, p.Y)), 1);
                    if (showImages)
                    {
                        displayImage.Polylines(points, true, Scalar.Red, 20);
                        displayImage.Rectangle(boundingRect, Scalar.Blue, 10);
                    }
                    yield return ExtractImage(src, rect, 20);
                }
            }
            if (showImages)
            {
                new Window("Schwellenwert", threshold.Resize(Size.Zero, scale, scale), WindowFlags.AutoSize);
                new Window("Erkannte Bilder", displayImage.Resize(Size.Zero, scale, scale), WindowFlags.AutoSize);
            }
        }
    }
}