# SmsGateway: Eine Klasse, die alles macht

Seit 1. Oktober 2026 müssen in Österreich Firmen ihre SMS-Absendernamen registrieren. So sollen
gefälschte SMS (*Phishing*) weniger werden. Die kleine Firma *TextBlitz* verschickt SMS für
Firmenkunden: Pizzadienste, Arztpraxen, Paketdienste. Ein Kunde liefert eine Liste mit
Nachrichten. TextBlitz prüft jede Zeile, blockiert SMS von nicht registrierten Absendern und
verschickt den Rest.

Ein Praktikant hat das Programm mit einem Chatbot geschrieben. Es funktioniert, und alle Tests
sind grün. Die ganze Logik steht aber in **einer** Klasse `SmsDispatcher` mit fast 200 Zeilen.
Jetzt kommen die ersten Änderungswünsche, und niemand traut sich, die Klasse anzufassen. Deine
Aufgabe ist es, die Klasse nach dem **Single Responsibility Principle** aufzuteilen. Das
Verhalten bleibt dabei gleich.

> [!NOTE]
> Die Registrierungspflicht gibt es wirklich. Alle Regeln in dieser Übung (Format der Liste,
> Namen, Längen, Kosten) sind aber erfunden.

## Aufbau der Solution

Öffne die Solution *SmsGateway.sln* in diesem Ordner.

| Projekt | Inhalt |
|---|---|
| *SmsGateway.Application* | `SmsMessage` (eine Nachricht) und `SmsDispatcher` (macht alles). Hier arbeitest du. |
| *SmsGateway.Tests* | Tests für die öffentliche API von `SmsDispatcher`. Sie sind alle grün. |

## Die fachlichen Regeln

`SmsDispatcher.Dispatch(lines)` bekommt Textzeilen im Format `sender;recipient;text`, zum
Beispiel `PizzaPronto;+436641234567;Your pizza is on the way.`

1. **Leere Zeilen** werden übersprungen. Sie zählen aber bei der Zeilennummer mit. Die erste
   Zeile hat die Nummer 1. Jeder Aufruf von `Dispatch` beginnt wieder bei 1.
2. **Felder:** Eine Zeile hat 3 Felder, getrennt durch `;`. Ab dem zweiten `;` gehört alles zum
   Text, der Text darf also selbst `;` enthalten. Absender und Empfänger werden ohne Leerzeichen
   am Anfang und Ende übernommen, der Text bleibt unverändert.
3. **Absender** darf nicht leer sein.
4. **Empfänger** ist eine österreichische Handynummer: `+43` und danach 8 bis 12 Ziffern, ohne
   Leerzeichen.
5. **Text** darf nicht leer sein und nicht nur aus Leerzeichen bestehen.
6. **SMS-Teile:** Ein Text mit bis zu 160 Zeichen passt in eine SMS. Längere Texte werden in
   Teile zu je 153 Zeichen geteilt, der letzte Teil ist kürzer. Erlaubt sind höchstens 3 Teile,
   also 459 Zeichen.
7. Verletzt eine Zeile die Regeln 2 bis 6, ist sie **ungültig**. In `Errors` steht dann ein Text
   wie `Line 4: invalid recipient '0664 1234567'.` Die Regeln werden in dieser Reihenfolge
   geprüft, nur der erste Fehler zählt.
8. **Register:** Ist eine Zeile gültig, aber der Absender nicht registriert, wird die Nachricht
   **blockiert** (`BlockedMessages`). Groß- und Kleinschreibung spielen keine Rolle.
9. Alle anderen Nachrichten werden **gesendet** (`SentMessages`). In dieser Version heißt das:
   Sie kommen in eine Liste im Speicher.
10. **Absendernamen** (Konstruktor und `RegisterSender`) haben 3 bis 11 Zeichen und enthalten nur
    Buchstaben und Ziffern. Sonst kommt eine `ArgumentException`.
11. **Report:** `CreateReport()` liefert die Anzahl der gesendeten Nachrichten und Teile, der
    blockierten Nachrichten und der ungültigen Zeilen. Jeder gesendete Teil kostet 0,09 €. Gibt es
    blockierte Nachrichten, listet der Report jeden blockierten Absender einmal auf, sortiert.

## Das Zieldesign

Nach deinem Umbau gibt es diese Klassen. Jede steht in einer eigenen Datei im Projekt
*SmsGateway.Application* und ist `public`.

| Klasse | Zuständigkeit | Öffentliche Member |
|---|---|---|
| `SenderRegistry` | Verwaltet die registrierten Absendernamen (Regeln 8 und 10). | `SenderRegistry(IEnumerable<string> senderNames)`<br>`void Register(string senderName)`<br>`bool IsRegistered(string senderName)` |
| `SmsParser` | Macht aus einer Zeile eine `SmsMessage`. Prüft dabei die Regeln 2 bis 6 und teilt den Text in Teile. | `SmsMessage Parse(string line)` |
| `SmsOutbox` | Nimmt gesendete Nachrichten auf (Regel 9). Später kann ein echter Versand über einen SMS-Anbieter diese Klasse ersetzen. | `void Send(SmsMessage message)`<br>`IReadOnlyList<SmsMessage> Messages { get; }` |
| `DispatchReport` | Berechnet Teile und Kosten und erstellt den Report-Text (Regel 11). | `string Create(IReadOnlyList<SmsMessage> sentMessages, IReadOnlyList<SmsMessage> blockedMessages, int invalidLineCount)` |
| `SmsDispatcher` | Koordiniert den Ablauf: Zeile für Zeile parsen, im Register nachsehen, senden oder blockieren, Fehler mit Zeilennummer sammeln. Für den Report ruft er `DispatchReport` auf. | unverändert |

Genauere Vorgaben:

- `SmsParser.Parse` wirft bei einer ungültigen Zeile eine `FormatException`. Ihre Message ist der
  Fehlertext **ohne** Zeilennummer, zum Beispiel `sender is missing.` Der Parser kennt die
  Zeilennummer nicht. `SmsDispatcher` setzt `Line 4: ` davor.
- `SenderRegistry.Register` wirft bei einem ungültigen Namen dieselbe `ArgumentException` wie
  bisher `RegisterSender`.
- `SmsDispatcher` behält seine öffentliche API: Konstruktor, `RegisterSender`, `Dispatch`,
  `SentMessages`, `BlockedMessages`, `Errors` und `CreateReport`. Er enthält danach keine eigenen
  Prüfungen, keine Konstanten für Längen oder Kosten und keine Texte für den Report mehr.
- `SmsMessage` bleibt unverändert.
- Jede neue Klasse und jede öffentliche Methode bekommt einen XML-Kommentar (`/// <summary>`)
  auf Englisch.

## Aufgabe

1. **Analysieren:** Lies `SmsDispatcher` und markiere die Abschnitte. Schreibe für jeden
   Abschnitt auf: Was ist seine Zuständigkeit? Wer im Unternehmen würde eine Änderung daran
   verlangen (Fachabteilung, Buchhaltung, IT, Gesetzgeber)? Notiere auch, welche Felder der
   Klasse jeder Abschnitt verwendet.
2. **Codezeilen messen:** Ermittle mit den Code Metrics in Visual Studio die Lines of Code von
   `SmsDispatcher` und notiere den Wert (siehe Kapitel *DRY* im Kurs). Ohne Visual
   Studio zählst du die Zeilen der Datei.
3. **`SenderRegistry`** erstellen. `SmsDispatcher` verwendet sie statt der eigenen Liste.
   Führe `dotnet test` aus.
4. **`SmsParser`** erstellen und in `Dispatch` verwenden. `dotnet test`.
5. **`SmsOutbox`** erstellen und verwenden. `dotnet test`.
6. **`DispatchReport`** erstellen und in `CreateReport` verwenden. `dotnet test`.
7. **Aufräumen:** Entferne alles, was `SmsDispatcher` nicht mehr braucht (Felder, Konstanten,
   `using`s). Miss die Lines of Code aller Klassen und vergleiche sie mit Schritt 2.

Regeln für die Arbeit:

- **Ändere die Tests nicht.** Sie sind dein Regressionstest: Sie zeigen, dass das Verhalten nach
  jedem Schritt gleich geblieben ist.
- Die öffentliche API von `SmsDispatcher` und `SmsMessage` bleibt gleich. Baue keine neuen
  Funktionen ein und repariere nichts. Refactoring ändert nur die Struktur.
- Arbeite in kleinen Schritten und führe nach jedem Schritt die Tests aus. Ist ein Test rot,
  korrigiere zuerst diesen Schritt.

Mit

```text
dotnet test
```

im Ordner der Solution oder im Test Explorer siehst du das Ergebnis.

**Zusatzfrage:** `SmsParser` prüft die Felder **und** teilt den Text in Teile. Sind das eine oder
zwei Zuständigkeiten? Begründe deine Antwort mit der Frage, wer eine Änderung daran verlangen
würde. Ab wann würdest du eine eigene Klasse für das Aufteilen anlegen?
