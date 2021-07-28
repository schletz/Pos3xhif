# Initialisierung des Projektes und erstes Service

## Anlegen des Projektes

```text
rd /S /Q OpenCvDemo
md OpenCvDemo
cd OpenCvDemo
md OpenCvDemo.Application
cd OpenCvDemo.Application
dotnet new classlib
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.win
cd ..
md OpenCvDemo.Test
cd OpenCvDemo.Test
dotnet new xunit
dotnet add reference ..\OpenCvDemo.Application
cd ..
dotnet new sln
dotnet sln add OpenCvDemo.Application
dotnet sln add OpenCvDemo.Test
start OpenCvDemo.sln

```

## Anlegen des ersten Services

Füge im Projekt *OpenCvDemo.Backend* den Ordner *Services* hinzu. Lege darin die folgende
Klasse *ImageProcessingService* an:

```c#
// Füge 
// using OpenCvSharp;
// am Beginn der Datei hinzu.
public class ImageProcessingService
{
    public void EdgeDetection(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new ArgumentException("Die Datei existiert nicht.");
        }
        using var src = new Mat(filename, ImreadModes.Grayscale);
        using var dst = new Mat();

        Cv2.Canny(src, dst, 50, 200);
        using (new Window("src image", src))
        using (new Window("dst image", dst))
        {
            Cv2.WaitKey();
        }
    }
}
```

## Anlegen des ersten Unittests

- Kopiere die Datei *lena.png* in den Ordner *OpenCvDemo.Test*. Sie befindet sich
  auf http://optipng.sourceforge.net/pngtech/img/lena.html
- Stelle in den Eigenschaften in Visual Studio bei dieser Datei die Option *Copy always* ein.
- Lege eine Klasse *ImageProcessingServiceTests* an und teste die oben beschriebene Methode.
- Es sollten sich 2 Fenster öffnen. Durch Tastendruck wird das Fenster geschlossen.

```c#
public class ImageProcessingServiceTests
{
    [Fact]
    public void EdgeDetectionSuccessTest()
    {
        var service = new ImageProcessingService();
        service.EdgeDetection("lena.png");
    }
}
```

## Weitere Infos

- NuGet Paket OpenVcSharp 4: [https://www.nuget.org/packages/OpenCvSharp4/](https://www.nuget.org/packages/OpenCvSharp4/)
- Projektseite: [https://github.com/shimat/opencvsharp](https://github.com/shimat/opencvsharp)
- Samples auf [https://github.com/shimat/opencvsharp_samples/](https://github.com/shimat/opencvsharp_samples/) und
  [https://github.com/shimat/opencvsharp/wiki](https://github.com/shimat/opencvsharp/wiki)
- PdfSharp [https://www.nuget.org/packages/PdfSharp/1.50.5147](https://www.nuget.org/packages/PdfSharp/1.50.5147)