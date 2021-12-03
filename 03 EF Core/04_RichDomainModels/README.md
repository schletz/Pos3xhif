# Rich Domain Models mit EF Core

## Was ist ein "Rich Domain Model"?

Bis jetzt waren unsere Modelklassen reine datenhaltende Klassen. Sie enthielten
keinerlei Logik. Dies bezeichnet man auch als "Anemic Domain Model". Solche Modelle
haben jedoch Nachteile:

- Die Logik wird in der darüberliegenden Schicht untergebracht. Dies erhöht die Gefahr
von Codeduplizierungen.
- Oft sind Validierungen erforderlich (eine Bestellung kann z. B. nur aufgegeben werden, wenn
der Kunde ein Zahlungsmittel hinterlegt hat). Die Modelklassen können nach Belieben erstellt
werden, also kann auch ohne Prüfung eine Bestellung in die Datenbank geschrieben werden.
- Die darüberliegende Schicht (Servicelayer) ist mit reinen CRUD Operationen "überfrachtet".

Daher wollen wir uns von den reinen Modelklassen lösen und gehen zurück zum Beginn der
Programmierausbildung. Dort wurden Klassen mit Feldern und Methoden definiert. An eine
Persistierung wurde gar nicht gedacht, alle Daten waren im Hauptspeicher verfügbar.
Genau diese Technik verwenden wir beim Erstellen von Rich Domain Models wieder. Wir achten
nicht darauf, wie die Persistenz die Daten ablegt und schreiben einfach die Klassen aus
dem logischen Blickwinkel.

Eine gute Einführung ist auf der Seite
https://paulovich.net/rich-domain-model-with-ddd-tdd-reviewed/
dargestellt.

## Ein kleines Bestellsystem

Würde - bevor das Thema Persistenz behandelt wurde - ein kleines Bestellsystem entworfen
werden, könnte es so aussehen:

```plantuml
@startuml
hide empty methods
class Store {
    +Name : string
    +Address :Address
}
Store *--> Address

class Offer  {
    +Product : Product
    +Store : Store
    +Price : decimal
    +LastUpdate : DateTime
}
Offer o--> Store
Offer <--> Product

class ProductCategory {
    +Name : string
    +NameEn : string?
}

class Product {
    +Ean : int <<unique>>
    +Name : string
    +ProductCategory : ProductCategory
    +Offers : List<Offer>
    ---
    CalculateAveragePrice()
}
Product o--> ProductCategory

class Address<<value object>> {
    +Street : string
    +Zip : string
    +City : string
}
class Customer <<Aggregate>> {
    +Firstname : string
    +Lastname : string
    +Address : Address
    +Orders : IReadOnlyCollection<Order>
    ---
    ConfirmOrder(Order order)
    PlaceOrder(Order order)
}
Customer *--> Address

class Order <<Aggregate>> {
    +Customer : Customer
    +Date : DateTime
    +OrderItems : IReadOnlyCollection<OrderItem>
    +ShippingAddress : Address
    ---
    +AddOrderItem(OrderItem orderItem)
}
Order <--> Customer
Order <--> OrderItem

class ConfirmedOrder {
    +ShippingDate : DateTime
    +ShippingCost : decimal
    ---
    +ConfirmedOrder(Order order, DeliveryDate deliveryDate, decimal shippingCost)
    +CalculateTotalPrice()
}
ConfirmedOrder -up-|> Order 

class OrderItem {
    +Offer : Offer
    +Price : decimal
    +Quantity : int
    +Order : Order
}
OrderItem o--> Offer

@enduml
```

Dieses Rich Domain Model bietet durch die Verwendung von Methoden viele Vorteile:

- **ConfirmOrder()** berechnet die ShippingCost und das ShippingDate aufgrund der
  Daten im Customer. Diese Methode ist daher in dieser Klasse viel besser aufgehoben als
  in einer darüberliegenden Schicht.
- **PlaceOrder()** fügt eine Bestellung in die Liste *Orders* ein und kann Prüfen durchführen.
- **AddOrderItem()** prüft, ob das Angebot schon in den OrderItems enthalten ist. Wenn ja,
  wird die Anzahl einfach erhöht statt ein neues OrderItem anzulegen.
- **CalculateAveragePrice()** kann aufgrund der Liste der Offers berechnen, zu welchem
  Preis im Mittel das Produkt angeboten wird.

Diese Methoden brauchen natürlich die Daten in den Klassen, damit sie funktionieren. Das muss
beim Definieren der Methoden im Klassenmodell immer berücksichtigt werden. So kann z. B. 
*CalculateAveragePrice()* nur den Durchschnittspreis berechnen, wenn sich eine Liste der Offers
in der Klasse befindet.
**Methoden in der Domain können nicht auf die Datenbank greifen und nach belieben "nachladen"!**

Folgende Designpatterns fallen auf:

### Aggregates

Einige Klassen wie *Customer* und *Ordner* speichern eine Collection (die Liste der Orders oder
die Liste der OrderItems). Daher bezeichnen wir die Klasse als *Aggregate*. Ein *Aggregate*
verwaltet die Elemente "seiner" Collection und achtet darauf, dass nicht falsche Werte
hinzugefügt werden können. Beachte den Datentyp der Collection: *IReadOnlyList\<T\>*.
Dadurch ist es nicht möglich, z. B. außerhalb der *PlaceOrder()* Methode eine Bestellung
hinzuzufügen.

### Beidseitige Navigations

Die Pfeile in diesem Diagramm haben manchmal 2 Pfeilspitzen. So speichert *Product* eine
Liste der Klasse *Offer*. Somit kann sehr bequem auf alle Angebote des Produktes zugegriffen
werden. Umgekehrt speichert die Klasse *Offer* das zugehörige *Product*. Wir sprechen hier
von einer *beidseitigen Navigation*.

### Verwenden von Vererbung

Oftmals durchlaufen Objekte einen State, wo Informationen hinzugefügt werden. Eine Bestellung
(Order) wird mit den Grunddaten angelegt. Wird die Bestellung dann bearbeitet, entstehen
neue Daten (*ShippingDate* und *ShippingCost*). Ohne Vererbung müssten diese Felder
nullable sein, da erst nach der Bearbeitung Werte zur Verfügung stehen. Dies sollte jedoch
vermieden werden. Was passiert, wenn z. B. *ShippingDate* ausgefüllt wird, aber *ShippingCost*
noch null ist?

Vererbung hilft also, nullable Werte zu reduzieren und erhöht so die Sicherheit des Codes.

### Value Objects

Oft werden Properties, die logisch zusammenhängend sind (also ein "zusammengesetztes Attribut" bilden)
als einzelne Properties in den Modelklassen definiert:

```c#
public class Customer 
{
    /* ... */
    public string Street {get; set;}
    public string Zip {get; set;}
    public string City {get; set;}
}

public class Shop 
{
    /* ... */
    public string Street {get; set;}
    public string Zip {get; set;}
    public string City {get; set;}
}
```

Die mit C# 9 eingeführten Records (sind immutable und bieten eine Überladung von *Equals*) helfen
uns, mit sehr wenig Code solche zusammengesetzten Attribute zu definieren.

```c#
public record Address(string Street, string Zip, string City);
public class Customer 
{
    /* ... */
    public Address Address {get; set;}
}

public class Shop 
{
    /* ... */
    public Address Address {get; set;}
}
```

In der Sprache der Domainmodellierung bezeichnen wir diese Klassen als *value objects*. Sie werden
später in die Datenbanktabelle als eigene Spalten (1 Spalte pro Property) integriert.

## Umsetzung mit EF Core
