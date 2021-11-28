# Entleihsystem für eine Bibliothek

Eine Bibliothek möchte ihren Kunden ein Reservierungssystem anbieten. Jeder Kunde kann sich
ein Buch oder eine Zeitschrift reservieren. Nach einer bestimmten Frist muss das Werk
zurückgegeben werden. Werden Fristen versäumt, sollen Kunden benachrichtigt werden. 1 Kunde
kann nur 1 Werk gleichzeitig entleihen.

```plantuml
@startuml
hide empty fields

class ReservationManager {
    +Reservations : IReadOnlyList<PublicationReservation> 
    ---
    +bool TryAddReservation(PublicationReservation reservation)
    +IReadOnlyList<PublicationReservation> GetPendingReservations(DateTime date)
}
ReservationManager --> PublicationReservation

class PublicationReservation {
  +Customer : Customer
  +Publication : Publication
  +ReservationFrom : DateTime 
  +ReturnDate : DateTime? 
  +MaxReturnDate : DateTime  <<calculated>
  +IsReturned : bool <<calculated>
  ---
  +bool IsPendingReservation(DateTime date)
}
PublicationReservation --> Customer
PublicationReservation --> Publication

class Customer {
  +CardId : int
  +Firstname : string
  +Lastname : string
}

abstract class Publication <<abstract>> {
  +Publisher : string
  +{abstract} ReturnAfterDays : int
}

class Book {
  +Title : string
  +Year : int
  +ReturnAfterDays : int
}
Book -up-|> Publication

class Magazine {
  +Name : string
  +PublicationDate : DateTime
  +ReturnAfterDays : int
}
Magazine -up-|> Publication

@enduml
```

## Klassenbeschreibung

#### Klasse ReservationManager

- **Reservations**: Eine read-only Liste, die alle Reservierungen zurückgibt.
- **TryAddReservation()**: Fügt eine Reservierung hinzu.
  - Liefert false, wenn der betroffene Kunde eine offene Reservierung hat (*IsReturned* false liefert).
    Hier wird keine Reservierung eingefügt.
  - Liefert true, wenn die Reservierung eingefügt wurde.
- **GetPendingReservations()**: Liefert eine Liste aller Reservierungen, die zum
  übergebenen Datum ausständig sind. Zur Bestimmung wird dabei die Methode
  *IsPendingReservation()* der Reservierung verwendet.

#### Klasse PublicationReservation
- **ReturnDate**: Null, wenn das Werk noch nicht zurückgebracht wurde.
- **MaxReturnDate**: Berechnetes spätestes Rückgabedatum. Es wird dabei das Property
*ReturnAfterDays* des Werkes verwendet. Hinweis: Verwende *AddDays()*.
- **IsReturned**: true, wenn das Rückgabedatum gesetzt ist. Sonst false.
- **IsPendingReservation()**: Liefert true, wenn die Reservierung zum übergebenen Zeitpunkt
ausständig ist. Das ist dann der Fall, wenn kein Rückgabedatum gesetzt ist und das
übergebene Datum nach dem maximalen Rückgabedatum ist.

#### Klasse Publication
- **ReturnAfterDays**: Abstraktes Property, welches die Tage, nach denen das Werk zurückgegeben
werden soll, liefert.

#### Klasse Book
- **ReturnAfterDays**: Liefert 14.

#### Klasse Magazine
- **ReturnAfterDays**: Liefert 7.

## Ihre Aufgabe

Starten Sie die Solution *LibraryManager.sln*. Die Datei *Program.cs* beinhaltet ein Testprogramm,
welches Ihre Implementierung testet. Diese Datei darf nicht verändert werden.

Legen Sie eine Datei für jede Klasse an. Starten Sie am Ende nach dem Schließen der IDE *cleanSolution.cmd*, um den Ordner *bin* und *obj* zu löschen. Danach laden Sie die ZIP Datei
mit Ihrer gesamten Solution (sln und alle Quellcodedateien) in Teams hoch.

Programme, die nicht kompilieren oder Programme, die Laufzeitfehler verursachen werden mit
"Nicht genügend" gewertet.

