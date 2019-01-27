# Pos3xhif
C# Beispiele für den 3. Jahrgang in POS.

## Synchronisieren des Repositories in einen Ordner
1. Lege einen Ordner auf der Festplatte an, wo du die Daten speichern möchtest 
    (z. B. *C:\Schule\POS\Examples*). Das
    Repository ist nur die lokale Version des Repositories auf https://github.com/schletz/Pos3xhif.git.
    Hier werden keine Commits gemacht und alle lokalen Änderungen dort werden bei der 
    nächsten Synchronisation überschrieben.
2. Initialisiere den Ordner mit folgenden Befehlen:
```bash {.line-numbers}
git init
git remote add origin https://github.com/schletz/Pos3xhif.git
```
3. Lege dir in diesem Ordner mit dem Texteditor eine Datei *syncGit.cmd* mit folgenden Befehlen an. 
    Durch Doppelklick auf diese Datei im Explorer wird immer der neueste Stand geladen.
```bash {.line-numbers}
git fetch --all
git reset --hard origin/master

```

## Optional: Anlegen eines eigenen Repositories:
1. Lege dir auf [GitHub] einen Zugang an. Über *Repositories* kannst du dir ein neues Repository mit
    dem Namen *POS* (oder anders) anlegen. Nach dem Anlegen des Repositories erscheint eine URL,
    die du dann beim Initialisieren noch brauchen wirst.

2. Lege einen Ordner auf der Festplatte an, wo du dein lokales Work Repository speichern möchtest 
    (z. B. *C:\Schule\POS\Work*). Der Example Ordner darf kein Unterverzeichnis in diesem Ordner sein.

3. Setze in der Konsole deinen Namen und deine Mailadresse in den globalen Einstellungen deiner
   git Installation:
```bash {.line-numbers}
git config --global user.name "FIRST_NAME LAST_NAME"
git config --global user.email "MY_NAME@example.com"
```

4. Initialisiere dein Work Repository mit folgenden Befehlen. Statt *(URL)* schreibe die URL deines
    auf Github angelegten Repositories hinein (z. B. *https://github.com/username/POS.git*)
```bash {.line-numbers}
git init
git remote add origin (URL)
```
5. Lege dir in diesem Ordner eine Datei *syncGit.cmd* mit folgenden Befehlen an. Durch Doppelklick
    auf diese Datei im Explorer werden alle Änderungen bestätigt ("Commit") und der Inhalt mit dem
    Online Repository auf Github synchronisiert.
```bash {.line-numbers}
git add -A
git commit -a -m "Commit"
git pull origin master
git push origin master
```

6. Erstellen von .gitignore: Damit nicht Builds und temporäre Dateien von Visual Studio hochgeladen werden, gibt es im Example
Repository eine Datei *.gitignore*. Kopiere diese Datei im Explorer aus deinem *Examples* Verzeichnis
in das *Work* Verzeichnis. Bei der Synchronisation wird nun die Datei übertragen. Ab jetzt werden die
temporären Dateien von Visual Studio zwar lokal gespeichert, aber nicht übertragen.

7. Zur Dokumentation wird im Programmierbereich die sogenannte Markdown Syntax verwendet. Sie definiert
    Formatierungsanweisungen in Textdateien. Eine Übersicht ist unter
    https://guides.github.com/features/mastering-markdown/ abrufbar. 

    Mit der Extension *Markdown Editor* kannst du in Visual Studio unter *Tools* - *Extensions and Updates* solche Dateien
    mit einer Voransicht entwerfen. In Chrome gibt es die Extension *Markdown Reader* für die Anzeige
    von lokalen md Dateien, wenn der Extension der Zugriff auf das *file://* Protokoll gestattet wurde.
    

[GitHub]: https://github.com

