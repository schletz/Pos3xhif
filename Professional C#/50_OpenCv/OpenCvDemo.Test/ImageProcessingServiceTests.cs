using OpenCvDemo.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenCvDemo.Test
{
    public class ImageProcessingServiceTests
    {
        [Fact]
        public void DetectPhotosSuccessTest()
        {
            var service = new ImageProcessor("scan.jpg");
            int count = service.DetectPhotos(sharpen: true);
            Assert.True(count == 2);
        }

        [Fact]
        public void DetectPhotosInTextSuccessTest()
        {
            var service = new ImageProcessor("scan_text_image.jpg");
            int count = service.DetectPhotos(sharpen: false);
            Assert.True(count == 1);
        }

        [Fact]
        public void GetTextSuccessTest()
        {
            var service = new ImageProcessor("scan_text_image.jpg");
            var text = service.ExtractText();
            Assert.Contains("natürlich", text);
        }
    }
}