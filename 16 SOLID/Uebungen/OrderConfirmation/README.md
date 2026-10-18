# OrderConfirmation: Bestellbestätigung per E-Mail

Der Webshop kann Bestellungen bezahlen. Die Kunden bekommen danach aber keine Nachricht und
fragen beim Support nach, ob ihre Bestellung angekommen ist. Ab jetzt soll jeder Kunde nach einer
erfolgreichen Zahlung eine Bestellbestätigung per E-Mail bekommen. Wie die E-Mail verschickt
wird (Mailserver, externer Dienst, ...), steht noch nicht fest. Der *CheckoutService* darf das
deshalb nicht wissen. Er bekommt den E-Mail Versand über ein Interface, so wie den Payment
Provider.

Die Tests sind schon geschrieben und zu Beginn alle rot. Deine Aufgabe ist es, sie grün zu machen.

## Aufbau der Solution

Öffne die Solution *OrderConfirmation.sln* in diesem Ordner.

| Projekt | Datei | Inhalt |
|---|---|---|
| *OrderConfirmation.Application* | *Order.cs*, *IPaymentProvider.cs*, *ILogger.cs*, *IEmailSender.cs* | Fertig. Nicht ändern. |
| *OrderConfirmation.Application* | *Email.cs*, *CheckoutService.cs* | Stubs: Die Methoden werfen `NotImplementedException`. Hier programmierst du. |
| *OrderConfirmation.Tests* | *FakePaymentProvider.cs* | Fertiger Fake aus dem Kapitel. Nicht ändern. |
| *OrderConfirmation.Tests* | *FakeEmailSender.cs* | Stub für den Fake des E-Mail Versands. Hier programmierst du. |
| *OrderConfirmation.Tests* | *CheckoutServiceTests.cs*, *EmailTests.cs* | Die Tests. Nicht ändern. |

## Die Anforderungen an Checkout()

`CheckoutService.Checkout(Order order)` arbeitet so:

1. Ist die Bestellung **schon bezahlt**, liefert die Methode `false`. Der Payment Provider wird
   nicht aufgerufen, und es wird keine E-Mail gesendet.
2. Lehnt der Payment Provider die Zahlung **ab**, liefert die Methode `false`. Die Bestellung
   bleibt unbezahlt, und es wird keine E-Mail gesendet.
3. Klappt die Zahlung, wird die Bestellung als bezahlt markiert. Danach wird **genau eine** E-Mail
   an `order.CustomerEmail` gesendet. Der Betreff ist `Order confirmation`. Der Text ist auf
   Englisch und enthält den Betrag (`TotalAmount`). Die Methode liefert `true`.
4. Ist ein Logger gesetzt, protokolliert die Methode wie im Kapitel, ob die Zahlung geklappt hat.
   Ohne Logger funktioniert sie genauso.

## Aufgabe

1. **Email:** Programmiere den Konstruktor und die Properties `To`, `Subject` und `Body`. Alle
   Properties sind read-only. Danach ist der Test in *EmailTests.cs* grün.
2. **FakeEmailSender:** Programmiere den Fake im Testprojekt. Er verschickt nichts, sondern
   speichert jede E-Mail, die `Send()` bekommt. `SentEmails` liefert alle gespeicherten E-Mails.
3. **CheckoutService:** Programmiere den Konstruktor und die Methode `Checkout()` nach den
   Anforderungen oben.
4. Führe nach jedem Schritt die Tests aus. Du bist fertig, wenn alle Tests grün sind.

Regeln für die Arbeit:

- Ändere die Tests und die fertigen Dateien nicht. Die öffentlichen Signaturen (Klassennamen,
  Konstruktoren, Methoden, Properties) bleiben, wie sie sind.
- **Depend on interfaces, not on concretes:** In *CheckoutService* kommen nur Interfaces und
  Datenklassen wie `Order` vor. `FakeEmailSender`, `FakePaymentProvider` oder eine andere Klasse
  mit Logik stehen dort nicht.
- Benenne Felder und Variablen auf Englisch, so wie im restlichen Code.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.

**Zusatzaufgabe:** Die Tests prüfen das Logging nicht. Schreibe im Testprojekt einen
`FakeLogger`, der jede Meldung speichert. Schreibe dann eigene Tests: Bei einer abgelehnten und
bei einer erfolgreichen Zahlung wird je genau eine Meldung protokolliert. Halte dich an die
Regel *ein Verhalten pro Test* und an sprechende Testnamen.
