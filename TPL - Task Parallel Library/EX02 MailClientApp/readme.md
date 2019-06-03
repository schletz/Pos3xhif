# Mailclient, der im Hintergrund lädt
Über 2 GET Aufrufe wird das Abrufen eines Mailpostfaches simuliert:

- **http://schletz.org/getMails** liefert ein JSON Array mit dem Inhalt des Postfaches.
- **http://schletz.org/getMailContent?mailid=1234** liefert den Inhalt der Mail mit der ID 1234.

Bei jedem Aufruf von *getMails* ändert sich der Inhalt, sodass ein Aktualisieren in der Applikation sichtbar ist.
Öffne die Solution *MailClientApp* und führe folgende Ergänzungen durch:

- Implementiere einen Background Loader in der Klasse *MailClient* (Namespace Service). Die Methoden
  sind schon vorgegeben. Verwende für den Abbruch einen CancellationToken, sodass auch der HTTP Aufruf
  abgebrochen werden kann.

- Wenn die Mails vom Server geladen wurden, wird das bereitgestellte Event *NewMail* im MailClient aufgerufen.
  Das ViewModel kann dann darauf reagieren und ein Update der Liste veranlassen.

- Ergänze die Bindings für den Mailinhalt und passe das Viewmodel entsprechend an.

- Der Button Start loading soll das Laden im Hintergrund starten.

- Der Button Stop loading beendet das Laden. Über die Statusbar soll sichtbar gemacht werden, wann
  der Hintergrundtask wirklich beendet wurde.
