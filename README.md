# Pos3xhif
C# Beispiele für den 3. Jahrgang in POS.

## Anlegen eines Ordners und Vorbereiten des Repositories:
1. Lege dir auf [GitHub] einen Zugang an.

2. Lege einen Ordner auf der Festplatte an, wo du deine lokalen Repositories speichern möchtest 
    (z. B. *C:\Schule\POS*). Erstelle danach 2 Unterordner: *Examples* und *Work*. Das *Examples*
    Repository ist nur die lokale Version des Repositories auf https://github.com/schletz/Pos3xhif.git.
    Hier werden keine Commits gemacht und alle lokalen Änderungen dort werden bei der 
    nächsten Synchronisation überschrieben.

3. Setze in der Konsole deinen Namen und deine Mailadresse in den globalen Einstellungen deiner
   git Installation:
```
git config --global user.name "FIRST_NAME LAST_NAME"
git config --global user.email "MY_NAME@example.com"
```

4. Lege auf GitHub unter *Repositories* ein Repository *POS* an. 
    
5.  Nach dem Anlegen erscheint ein Textfeld mit der URL des Repositories 
    (z. B. *https://github.com/username/POS.git*).
    Ersetze *(URL)* in den nachfolgenden Befehlen durch diese Adresse und führe sie
    in der Konsole in deinem lokalen git Verzeichnis aus. Hinweis: Kopiere alle Befehle. Mit 
    Rechtsklick kannst du sie in der Konsole aus der Zwischenablage einfügen.

**Code für das Example Repository**
```
git init
git remote add origin https://github.com/schletz/Pos3xhif.git
```

**Code für das Work Repository**
```
git init
git remote add origin (URL)
```


## Aktualisieren des Repositories
Erstelle für jedes Repository eine Datei *syncGit.cmd* im Texteditor und füge folgende die 
jeweiligen Befehle ein.

**Code für das Example Repository**
```
git fetch --all
git reset --hard origin/master

```

**Code für das Work Repository**
```
git add -A
git commit -a -m "Commit"
git pull origin master
git push origin master
```

Speichere die Datei im root deines git Ordners und synchroisiere durch Doppelklick auf die Datei im
Explorer dein lokales Repository mit dem Github Server.

## Erstellen von .gitignore
Damit nicht Builds und temporäre Dateien von Visual Studio hochgeladen werden, gibt es im Example
Repository eine Datei *.gitignore*. Kopiere diese Datei im Explorer aus deinem *Examples* Verzeichnis
in das *Work* Verzeichnis. Bei der Synchronisation wird nun die Datei übertragen. Ab jetzt werden die
temporären Dateien von Visual Studio zwar lokal gespeichert, aber nicht übertragen.

[GitHub]: https://github.com
