# Dependency Injection

## Projekt anlegen

Für die Beispiele brauchst du eine .NET Konsolenapplikation. Führe diese Befehle in der Konsole
aus. Unter macOS ersetzt du *rd* und *md* durch *rm -rf* und *mkdir*.

```text
rd /S /Q WebshopDemo
md WebshopDemo
cd WebshopDemo
md WebshopDemo.Application
cd WebshopDemo.Application
dotnet new console
cd ..
dotnet new sln -f sln
dotnet sln add WebshopDemo.Application
start WebshopDemo.sln

```

Öffne danach die Projektdatei *WebshopDemo.Application.csproj* (Doppelklick auf das Projekt).
Entferne die Zeile *ImplicitUsings* und füge *TreatWarningsAsErrors* hinzu.
Die Datei sieht dann so aus:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

</Project>
```

## Ausgangslage

Wir setzen das Webshop-Beispiel aus dem Kapitel [Interfaces](06_Interfaces.md) fort. In echten
Anwendungen trennt man **Daten** und **Abläufe**:
- *Order* speichert nur die Daten einer Bestellung.
- Ein *Service* (hier *CheckoutService*) führt den Ablauf „Bestellung bezahlen“ aus. Dafür braucht
  er einen Payment Provider.

Für dieses Kapitel reicht eine vereinfachte *Order*:

**Order.cs**
```c#
namespace WebshopDemo.Application;

class Order
{
    public Order(string customerEmail, decimal totalAmount)
    {
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
    }

    public string CustomerEmail { get; }
    public decimal TotalAmount { get; }
    public bool IsPaid { get; private set; }

    public void MarkAsPaid() => IsPaid = true;
}
```

*IPaymentProvider* und *CreditCard* übernimmst du aus dem Kapitel Interfaces.

## Das Problem: Eine Klasse erzeugt ihre Abhängigkeiten selbst

Der Service soll protokollieren, ob eine Zahlung geklappt hat. Den Payment Provider bekommt er
schon von außen, so wie *Order* im Kapitel Interfaces. Den Logger erzeugt er aber selbst, direkt in der Methode:

```c#
class ConsoleLogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

class CheckoutService
{
    private readonly IPaymentProvider _paymentProvider;

    public CheckoutService(IPaymentProvider paymentProvider)
    {
        _paymentProvider = paymentProvider;
    }

    public bool Checkout(Order order)
    {
        ConsoleLogger logger = new ConsoleLogger();   // (!)
        if (order.IsPaid) { return false; }
        if (!_paymentProvider.Pay(order.TotalAmount))
        {
            logger.Log($"Zahlung von {order.TotalAmount} für {order.CustomerEmail} abgelehnt.");
            return false;
        }
        order.MarkAsPaid();
        logger.Log($"Zahlung von {order.TotalAmount} für {order.CustomerEmail} erfolgreich.");
        return true;
    }
}
```

Die Zeile mit *new ConsoleLogger()* macht mehrere Probleme:

| Problem                 | Warum?                                                                     |
| ----------------------- | -------------------------------------------------------------------------- |
| **Nicht austauschbar**  | Sollen die Logs in eine Datei oder an einen Logging-Dienst gehen, musst du *CheckoutService* ändern. |
| **Nicht testbar**       | Im Test kannst du nicht prüfen, ob und was geloggt wurde. Jeder Test schreibt in die Konsole. |
| **Nicht abschaltbar**   | Jeder, der den Service verwendet, bekommt Konsolenausgaben. Auch eine Web-App, die gar keine Konsole hat. |
| **Versteckt**           | Der Konstruktor zeigt nicht, dass der Service in die Konsole schreibt.     |

## Die Lösung: Dependency Injection

**Dependency Injection (DI)** bedeutet: Eine Klasse erzeugt ihre Abhängigkeiten **nicht selbst**
mit *new*. Sie bekommt sie **von außen** übergeben („injiziert“). Sie verlangt dabei nur ein
Interface, keine konkrete Klasse.

> [!IMPORTANT]
> **Grundsatz: Depend on interfaces, not on concretes**
>
> Eine Klasse soll von **Interfaces** abhängen, nicht von **konkreten Klassen**.
> - *CheckoutService* kennt nur *IPaymentProvider* und *ILogger*.
> - *CreditCard* oder *ConsoleLogger* kommen in *CheckoutService* nicht vor.
>
> **Prüffrage:** Steht im Konstruktor, in einem Feld oder in einem Property eines Services eine
> konkrete Klasse wie *ConsoleLogger*? Oder steht *new ConsoleLogger()* in einer Methode? Dann
> ist der Service fest an diese Klasse gebunden.
>
> **Ausnahme:** Datenklassen wie *Order*, *Product* oder *string* darfst du direkt verwenden. Sie
> enthalten nur Daten, keine austauschbare Logik.
>
> Das ist der Kern des *Dependency Inversion Principle* (das **D** in SOLID). Dependency Injection
> ist die Technik, mit der du diesen Grundsatz umsetzt.

Beim Payment Provider machen wir das schon. Jetzt behandeln wir den Logger genauso: Wir
definieren ein Interface *ILogger*, und der Service bekommt den Logger von außen.

**CheckoutService.cs**
```c#
namespace WebshopDemo.Application;

class CheckoutService
{
    private readonly IPaymentProvider _paymentProvider;

    public CheckoutService(IPaymentProvider paymentProvider)   // (1)
    {
        _paymentProvider = paymentProvider;
    }

    public ILogger? Logger { get; set; }                        // (2)

    public bool Checkout(Order order)
    {
        if (order.IsPaid) { return false; }
        if (!_paymentProvider.Pay(order.TotalAmount))
        {
            Logger?.Log($"Zahlung von {order.TotalAmount} für {order.CustomerEmail} abgelehnt.");
            return false;
        }
        order.MarkAsPaid();
        Logger?.Log($"Zahlung von {order.TotalAmount} für {order.CustomerEmail} erfolgreich.");
        return true;
    }
}
```

- **(1) Constructor Injection:** Ohne Payment Provider kann der Service nicht arbeiten. Daher
  verlangt der Konstruktor ihn. Ohne Payment Provider lässt sich der Service gar nicht erstellen.
- **(2) Property Injection:** Statt *new ConsoleLogger()* steht hier nur noch das Interface
  *ILogger*. Logging ist optional. Daher ist der Logger ein nullable Property, das man von außen
  setzen **kann**. Mit *?.* wird *Log()* nur aufgerufen, wenn ein Logger gesetzt ist.

*ConsoleLogger* bleibt fast gleich. Er implementiert jetzt nur zusätzlich das Interface:

**ILogger.cs** und **ConsoleLogger.cs**
```c#
namespace WebshopDemo.Application;

interface ILogger
{
    void Log(string message);
}
```

```c#
using System;

namespace WebshopDemo.Application;

class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}
```

### Wer erzeugt die Objekte?

Irgendwo muss *new* stehen. Das passiert an **einer** Stelle beim Programmstart, der
*Composition Root*. Bei uns ist das *Main()*:

**Program.cs**
```c#
using System;

namespace WebshopDemo.Application;

class Program
{
    private static void Main()
    {
        // Composition root: the only place that decides which implementations are used.
        IPaymentProvider paymentProvider = new CreditCard(limit: 1000, expiration: new DateTime(2030, 12, 31));
        CheckoutService checkoutService = new CheckoutService(paymentProvider);
        checkoutService.Logger = new ConsoleLogger();

        Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
        Console.WriteLine($"Checkout erfolgreich: {checkoutService.Checkout(order)}");
    }
}
```

```text
[LOG] Zahlung von 250 für max@example.com erfolgreich.
Checkout erfolgreich: True
```

Willst du in eine Datei loggen oder mit einer Prepaid Karte zahlen, änderst du nur die
entsprechende Zeile in *Main()*. Willst du gar nicht loggen, lässt du die Zeile mit *Logger* weg.
Der *CheckoutService* bleibt unverändert.

> [!NOTE]
> **Ausblick:** In ASP.NET Core übernimmt ein *DI Container* die Arbeit des Composition Root.
> Du registrierst dort nur, welches Interface zu welcher Klasse gehört. Der Container erzeugt
> die Objekte und übergibt sie an die Konstruktoren.

## Testen mit einem Fake

Jetzt zahlt sich DI aus: Im Test übergeben wir eine **Fake**-Implementierung. Sie zahlt nicht
wirklich, sondern liefert ein festgelegtes Ergebnis und merkt sich jeden Aufruf.

**FakePaymentProvider.cs**
```c#
using System.Collections.Generic;

namespace WebshopDemo.Application;

class FakePaymentProvider : IPaymentProvider
{
    private readonly bool _result;
    private readonly List<decimal> _payments = new();

    public FakePaymentProvider(bool result)
    {
        _result = result;
    }

    public IReadOnlyList<decimal> Payments => _payments;

    public bool Pay(decimal amount)
    {
        _payments.Add(amount);
        return _result;
    }
}
```

So testest du den Fall „Zahlung abgelehnt“ ohne echte Kreditkarte:

```c#
FakePaymentProvider paymentProvider = new FakePaymentProvider(result: false);
CheckoutService checkoutService = new CheckoutService(paymentProvider);
Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);

bool success = checkoutService.Checkout(order);
// Expected: success == false, order.IsPaid == false, paymentProvider.Payments.Count == 1
```

> [!NOTE]
> **Begriffe:** Eine einfache Ersatzimplementierung für Tests heißt *Fake*. Bibliotheken wie
> [Moq](https://github.com/devlooped/moq) oder [NSubstitute](https://nsubstitute.github.io/)
> erzeugen solche Klassen automatisch. Man spricht dann von *Mocks*.

## Constructor oder Property Injection?

Die Entscheidung folgt der **Multiplizität** aus dem Kapitel
[Interfaces](06_Interfaces.md#multiplizität-muss-es-das-objekt-geben):

|                   | Constructor Injection                        | Property Injection                         |
| ----------------- | -------------------------------------------- | ------------------------------------------ |
| Multiplizität     | **1** (Pflicht)                              | **0..1** (optional)                        |
| C#                | Konstruktorparameter, *private readonly* Feld | nullable Property mit *set*               |
| Beispiel          | *IPaymentProvider*                           | *ILogger?*                                 |
| Vorteil           | Objekt ist immer vollständig. Der Compiler erzwingt die Abhängigkeit. | Objekt funktioniert auch ohne. |
| Risiko            | –                                            | Vergisst man das Setzen, fehlt die Funktion ohne Fehlermeldung. |

**Regel:** Verwende Constructor Injection. Property Injection nur, wenn die Abhängigkeit wirklich
optional ist.

## Was bringt das?

| Vorteil                        | Im Beispiel                                                           |
| ------------------------------ | --------------------------------------------------------------------- |
| **Testbarkeit**                | Wir testen „Zahlung abgelehnt“ mit einem Fake, ohne echte Zahlung.    |
| **Austauschbarkeit**           | Konsole oder Datei, Kreditkarte oder Prepaid Karte: nur *Main()* ändert sich. |
| **Sichtbare Abhängigkeiten**   | Der Konstruktor zeigt, was der Service braucht.                       |
| **Konfiguration an einer Stelle** | Limit, Ablaufdatum, ... stehen nur im Composition Root.            |

> [!TIP]
> **Versteckte Abhängigkeiten gibt es oft:** Auch *DateTime.Now*, *new Random()* oder
> *Console.WriteLine()* in einer Methode sind Abhängigkeiten. Sie machen Code schwer testbar.
> Wie testest du z. B. die Prüfung des Ablaufdatums in *CreditCard*, ohne bis 2030 zu warten?
> Für die Zeit bietet .NET die Klasse [TimeProvider](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview),
> die du per DI übergeben kannst.

## Übung: Bestellbestätigung

Nach einer erfolgreichen Zahlung soll der Kunde eine Bestellbestätigung per E-Mail bekommen.

1. Erstelle die Klasse **Email** mit den read-only Properties *To*, *Subject* und *Body*
   (alle *string*) und einem Konstruktor für alle drei Werte.
1. Erstelle das Interface **IEmailSender** mit der Methode *void Send(Email email)*.
1. Erstelle die Klasse **ConsoleEmailSender**. Sie implementiert *IEmailSender* und gibt die
   E-Mail in der Konsole aus. Das Format kannst du frei wählen.
1. Erstelle die Klasse **FakeEmailSender** für Tests. Sie implementiert *IEmailSender* und
   speichert jede gesendete E-Mail. Das Property *SentEmails* vom Typ
   *IReadOnlyList&lt;Email&gt;* liefert die gesendeten E-Mails.
1. Erweitere **CheckoutService**:
   - Der Service braucht den E-Mail Sender immer. Entscheide, ob du ihn über den Konstruktor
     oder über ein Property übergibst.
   - Nach erfolgreicher Zahlung sendet *Checkout()* genau eine E-Mail an *order.CustomerEmail*.
     Der Betreff ist *Bestellbestätigung*. Der Text enthält den Betrag (*TotalAmount*).
   - Wird die Zahlung abgelehnt oder ist die Bestellung schon bezahlt, wird keine E-Mail gesendet.
   - Der Logger bleibt optional.
   - *Depend on interfaces, not on concretes:* *ConsoleEmailSender* und *FakeEmailSender* kommen
     in *CheckoutService* nicht vor.

Übernimm *FakePaymentProvider* von oben. Ersetze *Program.cs* durch den folgenden Code. Nach dem
Start muss das Programm die Ausgabe darunter zeigen. Am Ende gibt dein *ConsoleEmailSender* eine
E-Mail aus.

**Program.cs**
```c#
using System;
using System.Linq;

namespace WebshopDemo.Application;

class Program
{
    private static int _testCount = 0;
    private static int _testsSucceeded = 0;

    private static void Main()
    {
        Console.WriteLine("Teste Klassenimplementierung.");
        CheckAndWrite(() => typeof(IEmailSender).IsInterface, "IEmailSender ist ein Interface");
        CheckAndWrite(() => typeof(Email).GetProperties().Any() && typeof(Email).GetProperties().All(p => !p.CanWrite), "Alle Properties in Email sind read only");
        CheckAndWrite(() => typeof(CheckoutService).GetConstructor(new[] { typeof(IPaymentProvider), typeof(IEmailSender) }) is not null,
            "CheckoutService(IPaymentProvider, IEmailSender) existiert");
        CheckAndWrite(() => typeof(CheckoutService).GetConstructors().All(c => c.GetParameters().Any(p => p.ParameterType == typeof(IEmailSender))),
            "Jeder Konstruktor verlangt einen IEmailSender");

        {
            Console.WriteLine("Teste erfolgreiche Zahlung.");
            FakePaymentProvider paymentProvider = new FakePaymentProvider(result: true);
            FakeEmailSender emailSender = new FakeEmailSender();
            CheckoutService checkoutService = new CheckoutService(paymentProvider, emailSender);
            Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
            CheckAndWrite(() => checkoutService.Checkout(order) && order.IsPaid, "Checkout liefert true, Order ist bezahlt");
            CheckAndWrite(() => paymentProvider.Payments.SequenceEqual(new decimal[] { 250 }), "Genau 1 Zahlung über 250");
            CheckAndWrite(() => emailSender.SentEmails.Count == 1, "Genau 1 E-Mail gesendet");
            CheckAndWrite(() => emailSender.SentEmails.First().To == "max@example.com", "E-Mail an den Kunden");
            CheckAndWrite(() => emailSender.SentEmails.First().Subject == "Bestellbestätigung", "Betreff ist Bestellbestätigung");
            CheckAndWrite(() => emailSender.SentEmails.First().Body.Contains("250"), "Text enthält den Betrag");

            Console.WriteLine("Teste bereits bezahlte Bestellung.");
            CheckAndWrite(() => !checkoutService.Checkout(order), "Zweiter Checkout liefert false");
            CheckAndWrite(() => paymentProvider.Payments.Count == 1 && emailSender.SentEmails.Count == 1, "Keine weitere Zahlung und E-Mail");
        }
        {
            Console.WriteLine("Teste abgelehnte Zahlung.");
            FakeEmailSender emailSender = new FakeEmailSender();
            CheckoutService checkoutService = new CheckoutService(new FakePaymentProvider(result: false), emailSender);
            Order order = new Order(customerEmail: "max@example.com", totalAmount: 250);
            CheckAndWrite(() => !checkoutService.Checkout(order) && !order.IsPaid, "Checkout liefert false, Order ist nicht bezahlt");
            CheckAndWrite(() => emailSender.SentEmails.Count == 0, "Keine E-Mail gesendet");
        }
        Console.WriteLine($"{_testsSucceeded} von {_testCount} Punkte erreicht.");

        Console.WriteLine("Demo mit ConsoleEmailSender:");
        CheckoutService demoService = new CheckoutService(new FakePaymentProvider(result: true), new ConsoleEmailSender());
        demoService.Checkout(new Order(customerEmail: "anna@example.com", totalAmount: 99));
    }

    private static void CheckAndWrite(Func<bool> predicate, string message)
    {
        _testCount++;
        if (predicate())
        {
            Console.WriteLine($"   {_testCount} OK: {message}");
            _testsSucceeded++;
            return;
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"   {_testCount} Nicht erfüllt: {message}");
        Console.ResetColor();
    }
}
```

```text
Teste Klassenimplementierung.
   1 OK: IEmailSender ist ein Interface
   2 OK: Alle Properties in Email sind read only
   3 OK: CheckoutService(IPaymentProvider, IEmailSender) existiert
   4 OK: Jeder Konstruktor verlangt einen IEmailSender
Teste erfolgreiche Zahlung.
   5 OK: Checkout liefert true, Order ist bezahlt
   6 OK: Genau 1 Zahlung über 250
   7 OK: Genau 1 E-Mail gesendet
   8 OK: E-Mail an den Kunden
   9 OK: Betreff ist Bestellbestätigung
   10 OK: Text enthält den Betrag
Teste bereits bezahlte Bestellung.
   11 OK: Zweiter Checkout liefert false
   12 OK: Keine weitere Zahlung und E-Mail
Teste abgelehnte Zahlung.
   13 OK: Checkout liefert false, Order ist nicht bezahlt
   14 OK: Keine E-Mail gesendet
14 von 14 Punkte erreicht.
Demo mit ConsoleEmailSender:
```

## Weitere Informationen

- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (Microsoft Learn)
- [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) (Microsoft Learn)
