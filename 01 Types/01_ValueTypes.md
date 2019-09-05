# Wertedatentypen in C#

Wertedatentypen belegen bei der Dekleration schon Speicher für ihren Wert. Sie
können daher - im Gegensatz zu Referenztypen - nicht null enthalten.

Neben den bekannten Datentypen aus Java gibt es in C# noch unsigned Datentypen
wie *uint*, *ulong*, ... Sie werden allerdings selten verwendet, da die Frameworkmethoden mit "normalen" 
signed Typen wie int, ... arbeiten.

Die Dekleration erfolgt wie aus Java gewohnt:
```c#
int myInt = 1;
bool myBool = true;
long myLong;   // Wird mit 0 initialisiert.
```

## Gleitkommatypen
Folgendes Programm liefert eine Endlosschleife, da 0.1 nicht exakt gespeichert werden
kann:
```c#
double sum = 0;
// Vergleiche mit double sind durch die Ungenauigkeit
// kritisch. 1000000 wird nie erreicht.
while (sum != 1000000)
{
    sum += 0.1;
}
```

## decimal
Möchte man Dezimalzahlen exakt speichern, kann der Typ *decimal* verwendet werden.
Die Performance ist allerdings schlechter als bei *double*:
```c#
decimal sum2 = 0;
while (sum2 != 1000000)
{
    sum2 += 0.1M;
}
```

## Explizite und implizite Typencasts
Kann jeder *mögliche* Wert (unabhängig vom aktuellen Wert bei der Zuweisung) zugewiesen werden, ist 
ein impliziter Typencast möglich.
Beispiel:
```c#
myLong = myInt;
myInt = myLong; // Geht nicht
```

Möchte man trotzdem eine Zuweisung zu einen "kürzeren" Typ machen, brauchen wir den
expliziten Typencast:
```c#
myInt = (int) myLong; // Wird notfalls abgeschnitten
```

## Nullable Types
Eine Zuweisung von null ist bei Wertetypen wie *int*, *long*, ... nicht möglich. Möchte man
trotzdem null zuweisen, etwa weil der Wert nicht bekannt ist, kann ein *nullable Type* definiert
werden:
```c#
int? myInt2;    // ? bedeutet nullable.
myInt2 = null;  // Gültige Zuweisung
```

Allerdings ist im weiteren Programmverlauf darauf zu achten, dass das Ergebnis einer Berechnung
mit einem nullable Type auch *null* liefern kann.
```c#
myInt = myInt2 + 1;  // Geht nicht, da es null liefern kann.
```

Es gibt allerdings den *null-coalescing Operator* ??. Er liefert den zweiten Wert falls
der erste Wert null ist:
```c#
myInt2 ?? 0;  // Liefert 0
1 ?? 2;       // Liefert 1
```

Durch diesen Operator können wir unsere Berechnung in myInt schreiben. Ob 0 ein sinnvoller Standardwert
ist, hängt natürlich von der Aufgabenstellung ab.
```c#
myInt = myInt2 ?? 0 + 1;  // Liefert 1, wenn myInt2 null ist.
```
