using OpenCvDemo.Application.Services;
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
        public void EdgeDetectionSuccessTest()
        {
            var service = new ImageProcessingService();
            service.EdgeDetection("lena.png");
        }

        [Fact]
        public void DetectPhotosSuccessTest()
        {
            var service = new ImageProcessingService();
            int count = service.DetectPhotos("scan.jpg", sharpen: true);
            Assert.True(count == 2);
        }

        [Fact]
        public void DetectPhotosInTextSuccessTest()
        {
            var service = new ImageProcessingService();
            int count = service.DetectPhotos("scan_text_image.jpg", sharpen: false);
            Assert.True(count == 1);
        }

        [Fact]
        public void GetTextSuccessTest()
        {
            var service = new ImageProcessingService();
            var text = service.GetText("scan_text_image.jpg");
            Assert.Contains("natürlich", text);
        }
    }
}