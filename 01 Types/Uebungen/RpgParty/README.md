# Eine Heldengruppe für das Rollenspiel

Diese Übung setzt die Übung *Charaktere für ein Rollenspiel* aus dem Kapitel *Properties* im Kurs
fort. Ein Charakter bekommt ein Inventar mit Waffen (List) und Fähigkeiten (HashSet). Mehrere
Charaktere bilden eine Heldengruppe, die *Party*. Sie verwaltet ihre Mitglieder und ihre Vorräte in
Dictionaries.

## Vorbereitung

Öffne die Solution *RpgParty.sln* in diesem Ordner. Kopiere deine Klassen *Weapon* und
*Character* aus der Solution *RpgDemo* in die Dateien *Weapon.cs* und *Character.cs* im Projekt
*RpgParty.Application*. Ändere den Namespace auf *RpgParty.Application*. *Weapon* bleibt gleich,
*Character* wird erweitert, und die Klasse *Party* in der Datei *Party.cs* ist neu.

Speichere jede Collection intern in einer `private readonly` Variable. Das Property nach außen ist
read-only und liefert nur ein Interface ohne ändernde Methoden (`IReadOnlyList<T>`,
`IReadOnlySet<T>` oder `IReadOnlyDictionary<TKey, TValue>`). Die Collection ändert sich also nur
über die Methoden der Klasse. Verwende `AsReadOnly()` und keinen Typecast (siehe Abschnitt
*Read-only Collections* im Kapitel *Collections*). Das Testprogramm prüft das.

## Erweiterung der Klasse Character

### Waffen (List)

- Das Property **Weapon** aus dem Kapitel Properties speichert weiterhin die ausgerüstete Waffe.
  Es darf aber nur noch innerhalb der Klasse gesetzt werden. Zum Ausrüsten gibt es die folgenden
  Methoden.
- Der Charakter hat ein Inventar mit beliebig vielen Waffen. Das Property **Weapons** liefert es
  als `IReadOnlyList<Weapon>`. Am Anfang ist das Inventar leer.
- **PickUp(Weapon weapon)** fügt die Waffe am Ende des Inventars ein. Hat der Charakter noch keine
  Waffe ausgerüstet, wird diese Waffe sofort ausgerüstet.
- **Equip(Weapon weapon)** rüstet die Waffe aus. Ist sie nicht im Inventar, wirft die Methode eine
  *ArgumentException*.
- **EquipStrongestWeapon()** rüstet die Waffe mit dem höchsten *Damage* aus dem Inventar aus. Ist
  das Inventar leer, passiert nichts.
- **Drop(Weapon weapon)** entfernt die Waffe aus dem Inventar und liefert *true*. War sie
  ausgerüstet, hat der Charakter danach keine Waffe mehr. Ist die Waffe nicht im Inventar, liefert
  die Methode *false*.

### Fähigkeiten (HashSet)

- Ein Charakter kann Fähigkeiten wie *Feuerball* oder *Heilen* lernen. Jede Fähigkeit kommt nur
  einmal vor. Das Property **Skills** liefert sie als `IReadOnlySet<string>`.
- **LearnSkill(string skill)** fügt die Fähigkeit hinzu. Die Methode liefert *true*, wenn die
  Fähigkeit neu ist, sonst *false*. Tipp: Sieh dir den Rückgabewert von `HashSet.Add()` an.
- **HasSkill(string skill)** liefert *true*, wenn der Charakter die Fähigkeit beherrscht.

## Klasse Party (Dictionary)

- Die Klasse hat einen Konstruktor mit dem Parameter *name* (string). Das Property **Name** ist
  immutable (nicht änderbar).
- Die Mitglieder liegen in einem Dictionary. Der Key ist der Name des Charakters. Das Property
  **Members** liefert es als `IReadOnlyDictionary<string, Character>`.
- **AddMember(Character character)** fügt den Charakter hinzu und liefert *true*. Gibt es schon
  ein Mitglied mit diesem Namen oder hat die Party schon 4 Mitglieder, fügt die Methode nichts
  hinzu und liefert *false*.
- **RemoveMember(string name)** entfernt das Mitglied mit diesem Namen. Die Methode liefert
  *true*, wenn es gefunden wurde, sonst *false*.
- **FindMember(string name)** liefert das Mitglied mit diesem Namen oder *null*. Verwende
  `TryGetValue()`. Überlege, welchen Rückgabetyp die Methode braucht.
- Das Property **AliveMembers** ist read-only. Es liefert eine neue `List<Character>` mit allen
  Mitgliedern, die noch leben.
- Das Property **Skills** ist read-only. Es liefert ein neues `HashSet<string>` mit allen
  Fähigkeiten der Mitglieder. Beherrschen mehrere Mitglieder dieselbe Fähigkeit, kommt sie trotzdem
  nur einmal vor. Tipp: `UnionWith()`.
- Die Party hat gemeinsame Vorräte, z. B. Heiltränke oder Fackeln. Sie liegen in einem Dictionary:
  Der Key ist der Name des Gegenstands, der Value die Anzahl. Das Property **Supplies** liefert es
  als `IReadOnlyDictionary<string, int>`.
- **AddSupply(string item, int count)** erhöht die Anzahl des Gegenstands um *count*. Gibt es den
  Gegenstand noch nicht, wird er mit dieser Anzahl angelegt. Ist *count* kleiner oder gleich 0,
  wirft die Methode eine *ArgumentException*.
- **GetSupplyCount(string item)** liefert die Anzahl des Gegenstands oder 0, wenn es ihn nicht
  gibt.
- **UseSupply(string item)** verringert die Anzahl um 1 und liefert *true*. Gibt es den Gegenstand
  nicht, liefert die Methode *false*. Ist danach keiner mehr übrig, entfernt sie den Key aus dem
  Dictionary.
- **AttackTogether(Character enemy)**: Alle lebenden Mitglieder greifen den Gegner mit *Attack()*
  an.

Überlege bei jeder Methode, welche Methode der Collection die Arbeit schon erledigt. Viele Methoden
brauchen dann nur eine Zeile. Achtung: `Contains()` und `Remove()` vergleichen bei einer Liste von
Objekten die Referenzen (siehe Abschnitt zu `List<T>` im Kapitel *Collections*).

## Durchzuführende Arbeiten

Die Dateien *Program.cs* und *TestHelpers.cs* darfst du nicht ändern. *Program.cs* ist ein
Testprogramm. Es kompiliert erst, wenn alle Klassen mit allen Properties und Methoden existieren.
Lass neue Methoden daher zuerst eine *NotImplementedException* werfen. Danach startest du das
Programm im Ordner *RpgParty.Application* mit `dotnet run`. Es zeigt dir bei jeder Prüfung *OK*
oder *FAILED*. Wirft eine Prüfung eine Exception, gilt sie als nicht erfüllt.

Eine vollständige Lösung liefert folgende Ausgabe:

```text
Checking the weapons of a character (List)
   (1) OK: Character.Weapons is a read-only IReadOnlyList<Weapon> and uses AsReadOnly().
   (2) OK: Character.Weapon cannot be set from outside the class.
   (3) OK: PickUp adds the weapon and equips the first weapon.
   (4) OK: Equip equips a weapon of the inventory.
   (5) OK: Equip throws an ArgumentException if the weapon is not in the inventory.
   (6) OK: EquipStrongestWeapon equips the weapon with the highest damage.
   (7) OK: Drop removes the weapon and unequips it.
Checking the skills of a character (HashSet)
   (8) OK: Character.Skills is a read-only IReadOnlySet<string> and uses AsReadOnly().
   (9) OK: LearnSkill ignores a skill the character already has.
   (10) OK: HasSkill returns true if the character has the skill.
Checking Party (Dictionary)
   (11) OK: Party does not have a default constructor.
   (12) OK: Party.Members and Party.Supplies are read-only IReadOnlyDictionary properties.
   (13) OK: AddMember adds the character with the name as key.
   (14) OK: AddMember rejects a second member with the same name.
   (15) OK: A party has at most 4 members.
   (16) OK: FindMember returns the member or null.
   (17) OK: RemoveMember removes the member with this name.
   (18) OK: AliveMembers returns the members who are alive.
   (19) OK: Party.Skills returns every skill of the members once.
   (20) OK: AddSupply and GetSupplyCount.
   (21) OK: AddSupply throws an ArgumentException if count is not greater than 0.
   (22) OK: UseSupply removes items that are used up.
   (23) OK: AttackTogether: all members who are alive attack the enemy.

Result of the program in C:\...\RpgParty.Application:
23 of 23 checks passed.
24 of 24 points. Grade: Sehr gut.
```
