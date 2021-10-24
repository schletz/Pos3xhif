# Dependency Injection

Eine praktische Anwendung von Interfaces findet sich in der Technik der *Dependency Injection*. In unserem
Beispiel simulieren wir den Zugriff auf den GPS Sensor des Smartphones. Je nach Betriebssystem (Android oder iOS)
benötigen wir spezifischen Code, um den Standort herauszufinden.

Die Trackeranwendung selbst soll auf allen Geräten laufen. Daher müssen wir gerätespezifischen Code
auslagern. Wir definieren einmal eine Klasse Point, die Längen- und Breitengrad speichert sowie ein 
Interface *ILocationProvider*:
```c#
class Point
{
    public double Lat { get; }
    public double Lng { get; }

    public Point(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }
}

interface ILocationProvider
{
    DateTime LastMeasurement { get; }
    Point GetLocation();
}
```

Damit wir unterschiedliche Betriebssysteme simulieren können, implementieren wir dieses Interface in
2 Klassen: *AppleLocationProvider* und *AndroidLocationProvider*. Sie liefern beide in unserer Demo Zufallszahlen,
jedoch liefert die Apple Version nur alle 5 Sekunden einen neuen Standort, während die Android Version alle
2 Sekunden einen neuen Standort liefert:

```c#
class AppleLocationProvider : ILocationProvider
{
    private Point? _currentPoint;
    public DateTime LastMeasurement { get; private set; } = DateTime.MinValue;
    public Point GetLocation()
    {
        // DEVICE SPECIFIC CODE

        Random rnd = new Random();
        DateTime now = DateTime.Now;
        if ((now - LastMeasurement).TotalSeconds > 5)
        {
            LastMeasurement = now;
            _currentPoint = new Point(rnd.NextDouble() * 180 - 90, rnd.NextDouble() * 360);
        }
        // Die Nullable analyse schlägt hier fehl. Wenn noch nie gemessen wurde, ist LastMeasurement
        // der 1.1.0001 und daher wird sicher ein Wer generiert. Mit NULL forgiving (!) sagen
        // wir, dass wir es besser wissen.
        return _currentPoint!;
    }
}

class AndroidLocationProvider : ILocationProvider
{
    private Point? _currentPoint;
    public DateTime LastMeasurement { get; private set; } = DateTime.MinValue;
    public Point GetLocation()
    {
        // DEVICE SPECIFIC CODE

        Random rnd = new Random();
        DateTime now = DateTime.Now;
        if ((now - LastMeasurement).TotalSeconds > 2)
        {
            LastMeasurement = now;
            _currentPoint = new Point(rnd.NextDouble() * 180 - 90, rnd.NextDouble() * 360);
        }
        return _currentPoint!;
    }
}

```
Der Tracker selbst soll geräteunabhängig sein. Um das zu erreichen, übergeben wir im Konstruktor einfach
eine Instanz des Interfaces und verlangen nicht eine spezifische Implementierung als Parameter. Die Methode
*DoTracking()* ruft dann einfach jede Sekunde die Methode *GetLocation()* auf:

```c#
class MyTracker
{
    private readonly ILocationProvider locator;

    // Die "Injection" erfolgt im Konstruktor.
    public MyTracker(ILocationProvider locator)
    {
        this.locator = locator;
    }

    public void DoTracking(int seconds)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < seconds)
        {
            Point position = locator.GetLocation();
            Console.WriteLine($"Lat: {position.Lat:0.00}°, Lng: {position.Lng:0.00}°");
            System.Threading.Thread.Sleep(1000);
        }
    }
}
```

Zum Testen instanzieren wir in unserer *Program* Klasse einmal mit einer Instanz von *AndroidLocationProvider*
und einmal mit einer Instanz von *AppleLocationProvider*.
```c#
class Program
{
    private static void Main(string[] args)
    {
        MyTracker tracker;
        Console.WriteLine("Tracking mit ANDROID (alle 2 Messungen ein neuer Wert):");
        tracker = new MyTracker(new AndroidLocationProvider());
        tracker.DoTracking(10);
        Console.WriteLine("Tracking mit APPLE (alle 5 Messungen ein neuer Wert):");
        tracker = new MyTracker(new AppleLocationProvider());
        tracker.DoTracking(10);
    }
}
```

In der Ausgabe erkennen wir die unterschiedlichen Intervalle bei der Aktualisierung der Werte.
```
Tracking mit ANDROID (alle 2 Messungen ein neuer Wert):
Lat: -12.95°, Lng: 3.2°
Lat: -12.95°, Lng: 3.2°
Lat: 45.51°, Lng: 26.2°
Lat: 45.51°, Lng: 26.2°
Lat: -8.77°, Lng: 3.5°
Lat: -8.77°, Lng: 3.5°
Lat: -56.19°, Lng: 232.6°
Lat: -56.19°, Lng: 232.6°
Lat: -27.74°, Lng: 185.7°
Lat: -27.74°, Lng: 185.7°
Tracking mit APPLE (alle 5 Messungen ein neuer Wert):
Lat: -7.93°, Lng: 202.5°
Lat: -7.93°, Lng: 202.5°
Lat: -7.93°, Lng: 202.5°
Lat: -7.93°, Lng: 202.5°
Lat: -7.93°, Lng: 202.5°
Lat: -13.71°, Lng: 86.0°
Lat: -13.71°, Lng: 86.0°
Lat: -13.71°, Lng: 86.0°
Lat: -13.71°, Lng: 86.0°
Lat: -13.71°, Lng: 86.0°
```

## Was bringt das?
Diese Implementierung bietet viele Möglichkeiten:
- Leichteres Testen: Wir können zum Testen unser Interface in einer "Dummy" Klasse implementieren, die Demowerte
  liefert. Diese Implementierung nennt sich "Mockup" und kann z. B. mit dem Paket
  [Moq](https://github.com/moq/moq4) durchgeführt werden.
- Trennung von gerätespezifischem Code: Wir sagen nur was wir brauchen und nicht schon wie wir dazu kommen.
- Erweiterbarkeit: Ein weiterer Gerätetyp kann ohne Änderung des Trackers angebunden werden.

## Übung

Unser Tracker soll nicht fix auf der Konsole ausgeben, sondern über einen zu definierenden Logger schreiben. 
Dabei wird der Logger in der Klasse *MyTracker* als Property definiert und von außen gesetzt.
1. Verwende als Basis statt der obigen *MyTracker* und *Program* Klasse die untenstehenden Versionen.
1. Schreibe ein Interface *ILogger*, welches die Methode *Log(string)* beinhaltet.
1. Implementiere die Klasse *ConsoleLogger*, die den übergebenen String in der *Log()* Methode einfach in
   der Konsole ausgibt.
1. Implementieren die Klasse *FileLogger*, die den Übergeben String einfach in den beim Instanzieren gewählten
   Dateinamen schreibt. Hinweis: Mit *System.IO.File.AppendAllText(pfad, inhalt)* kann Text zu einer Datei
   hinzugefügt werden.
1. Ergänze das Property Logger in der Klasse *MyTracker* mit dem korrekten Datentyp.

```c#
class MyTracker
{
    private readonly ILocationProvider locator;
    public MyTracker(ILocationProvider locator)
    {
        this.locator = locator;
    }

    public void DoTracking(int seconds)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < seconds)
        {
            Point position = locator.GetLocation();
            Logger?.Log($"Lat: {position.Lat:0.00}°, Lng: {position.Lat:0.00}°");
            System.Threading.Thread.Sleep(1000);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        MyTracker tracker;
        Console.WriteLine("Tracking mit ANDROID:");
        tracker = new MyTracker(new AndroidLocationProvider());
        tracker.Logger = new ConsoleLogger();
        tracker.DoTracking(10);
        Console.WriteLine("Tracking mit APPLE:");
        tracker = new MyTracker(new AppleLocationProvider());
        tracker.Logger = new FileLogger("locations.txt");
        tracker.DoTracking(10);
    }
}
```

## Weitere Informationen
- https://www.codementor.io/mrfojo/c-with-dependency-injection-k2qfxbb8q
- https://blogs.msdn.microsoft.com/dmx/2014/10/14/was-ist-eigentlich-dependency-injection-di/