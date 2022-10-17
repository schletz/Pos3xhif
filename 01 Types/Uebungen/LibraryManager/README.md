# Entleihsystem für eine Bibliothek

Eine Bibliothek möchte ihren Kunden ein Reservierungssystem anbieten. Jeder Kunde kann sich
ein Buch oder eine Zeitschrift reservieren. Nach einer bestimmten Frist muss das Werk
zurückgegeben werden. Werden Fristen versäumt, sollen Kunden benachrichtigt werden. 1 Kunde
kann nur 1 Werk gleichzeitig entleihen.

![](modell_1837.svg)

<small><sup>
https://www.plantuml.com/plantuml/uml/ZLFDZjem4BxdANmCWhp02j7TThKLqjAkLbnw7EA1jUeuaUrKJIdlledD18UO24uMyyryFpDvD1QqhGkPV0YE38lADcmlK7AJ9Ba4OzWx6jH_m8fIRK310JKx9gpzpOEAOGzi_Ox0VohPl0fZbs_rJehS5GFSYXtr6NLoGx8iyyDsPIdPLZTFd0VuD3w6wSjxvlllO_-EzWqL5-eGYa-VmU9M5CXu-vWbfoHYEijMB3wMaefNNLhpRxMnPO6wpOcUmoZcGJTx26TTuOBHBxeiMYof7m5jhLLN331Vkie6_WwByG5hur78MupRJr2drStZhjFUPNv-B0QUgijxSU7NGs-pX0vZRSPh6syjBiLjZkHCOQWKh9JQpai4pTTT64BPgwnUX3PMGT75QQnkbLu1Nc6gJc4INzenv8vkGVWzKVJ_I3_JUKzWPwo6tBBHCRBbaeghbNVYgkO3zIsvH-euCNy4JtkB-XaQG_QdJo4zJws8o6dqqhwMvMylOokil9dOBmGTpVf-GHtJI4aVd-tO5wwvGN0AdRIihhB_qNFOm07-2OLU_O-9TGVDqn_P_MQ8_BOXVnmzKJNKqFcZ6J6FZwXuNSXF
</sup></small>


## Klassenbeschreibung

Für die Implementierung notwendige private fields sind selbstständig zu definieren.

#### Klasse ReservationManager

- **Reservations**: Eine read-only Liste, die alle Reservierungen zurückgibt.
- **TryAddReservation()**: Fügt eine Reservierung hinzu.
  - Liefert false, wenn **der betroffene Kunde** eine offene Reservierung hat (*IsReturned* false liefert).
    Hier wird keine Reservierung eingefügt.
  - Liefert true, wenn die Reservierung eingefügt wurde.
- **GetPendingReservations()**: Liefert eine Liste aller Reservierungen, die zum
  übergebenen Datum ausständig sind. Zur Bestimmung wird dabei die Methode
  *IsPendingReservation()* der Reservierung verwendet.

#### Klasse PublicationReservation

- **Customer**: Read-only Property, das den Kunden für diese Reservierung speichert.
- **Publication**: Read-only Property, das das Werk (die Publication) für diese Reservierung speichert.
- **ReservationFrom**: Read-only Property, das den Startzeitpunkt der Reservierung speichert.
- **ReturnDate**: Property; ist null, wenn das Werk noch nicht zurückgebracht wurde. Ansonsten
  wird es von außen auf das Rückgabedatum gesetzt.
- **MaxReturnDate**: Berechnetes Property. Berechnetes spätestes Rückgabedatum. Es wird dabei das 
  Property *ReturnAfterDays* des Werkes verwendet. Hinweis: Verwende *AddDays()*.
- **IsReturned**: Berechnetes Property. Ist true, wenn das Rückgabedatum gesetzt ist. Sonst false.
- **IsPendingReservation()**: Liefert true, wenn die Reservierung zum übergebenen Zeitpunkt
  ausständig ist. Das ist dann der Fall, wenn kein Rückgabedatum gesetzt ist und das
  übergebene Datum nach dem maximalen Rückgabedatum ist.

#### Klasse Publication

- **Publisher**: Read-only Property, das den Verlag (Publisher) speichert.
- **ReturnAfterDays**: Abstraktes berechnetes Property, welches die Tage, nach denen das Werk
  zurückgegeben werden soll, liefert.

#### Klasse Book

- **Title**: Read-only Property, das den Titel (Title) speichert.
- **Year**: Read-only Property, das das Erscheinungsjahr speichert.
- **ReturnAfterDays**: Liefert 14.

#### Klasse Magazine

- **Name**: Read-only Property, das den Namen der Zeitschrift speichert.
- **PublicationDate**: Read-only Property, das den Tag der Erscheinung speichert.
- **ReturnAfterDays**: Liefert 7.

## Ihre Aufgabe

Starten Sie die Solution *LibraryManager.sln*. Die Datei *Program.cs* beinhaltet ein Testprogramm,
welches Ihre Implementierung testet. Diese Datei darf nicht verändert werden.

Legen Sie eine Datei für jede Klasse an. Starten Sie am Ende nach dem Schließen der IDE *cleanSolution.cmd*, um den Ordner *bin* und *obj* zu löschen. Danach laden Sie die ZIP Datei
mit Ihrer gesamten Solution (sln und alle Quellcodedateien) in Teams hoch.

Programme, die nicht kompilieren oder Programme, die Laufzeitfehler verursachen werden mit
"Nicht genügend" gewertet.
