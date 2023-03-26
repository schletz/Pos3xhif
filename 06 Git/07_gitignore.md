# Nachträgliches Ändern der .gitignore Datei

Falls die Datei *.gitignore* nachträglich geändert wurde, kann es sein, dass schon Dateien, die wir ignorieren wollen, im Repository sind.
Technisch gesehen sind sie im *Index*.
Daher müssen wir uns zuerst mit der Funktion der Datei *.gitignore* befassen.
Erstellen wir Dateien, fügt sie die IDE im Hintergrund zum Index hinzu.
Manuell kann dies mit dem Kommando `git add -A` in der Shell gemacht werden.
Wird nun in *.gitignore* ein Ausschluss definiert, übergeht das *git add* Kommando diese Dateien.
Der Filter wird also nur beim *Hinzufügen von Dateien zum Index* wirksam.
Stehen diese Dateien aber schon im Index, brauchen wir härtere Werkzeuge.

## Vorbereitung des lokalen Repos

1. Bringe zuerst das lokale Repo auf den aktuellsten Stand und führe ein Commit aller offenen Änderungen durch.
2. Editiere nun die Datei *.gitignore* und erstelle einen Commit (z. B. mit der Meldung *fix gitignore*).
3. Schreibe die Änderungen mit `git push` oder der IDE in das Remote Repository.

## Neues Schreiben des Index

Führe in der Konsole (Bash, Windows Command) die folgenden Befehle durch.
**Lese allerdings vorher die Anmerkungen am Ende.**

```bash
git rm -rf --cached .
git clean -dfX
git add -A
git commit --amend --no-edit
git push --force
```

Was passiert hier?
Das `git rm` Kommando löscht Dateien aus dem Index oder sogar von der Festplatte.
Durch die Option *--cached* geben wir an, dass wir alle Dateien aus dem Index, aber nicht von der Festplatte löschen möchten.
Danach werden mit `git clean` die Dateien, die wir ignorieren, physisch gelöscht.
Die Option *X* erledigt diesen Schritt.
Danach wird mit `git add` alles wieder zum Index hinzugefügt.
Da wir die Datei *.gitignore* schon angepasst und committed haben, berücksichtigt *git add* diesen Filter.
Am Schluss schreiben wir mit einem *amend commit* den letzten Commit (*fix gitignore*) neu und übertragen diese Änderung in das remote Repository.
Dabei brauchen wir die Option *force*, da wir die History durch den amend Commit neu geschrieben haben.

> **Vorsicht:** Diese Operation löscht Dateien, die in *.gitignore* gelistet sind.
> Brauchst du diese Dateien lokal (Konfigurationseinstellungen, ...) musst du sie vorher sichern.

## Variante ohne Löschen der lokalen Dateien

Wenn der *git clean* Befehl ausgelassen wird, bleiben die ignorierten Dateien erhalten, werden aber nicht mehr eingecheckt.
Das ist hilfreich, um z. B. Konfigurationsdateien auf der Platte zu belassen.

```bash
git rm -rf --cached .
git add -A
git commit --amend --no-edit
git push --force
```

### Bereinigen der lokalen Dateien

Möchte man gezielt Builds, etc. bereinigen, hilft der find Befehl.
Er ist in der **git bash** im obersten Verzeichnis des Repos auszuführen (nicht im Windows Command).

Der nachfolgende Befehl löscht alle *bin* Verzeichnisse.
Es wird nach Verzeichnissen gesucht (Parameter *-type d*) und für den gefundenen Pfad wird das Löschkommando (*rm -rf*) ausgeführt:

```bash
find . -type d -name "bin" -exec rm -rf {} \;
```

Es kann auch nach Dateien (z. B. alle SQLite Datenbanken mit der Endung db) gesucht werden:

```bash
find . -name "*.db*" -exec rm -rf {} \;
```
