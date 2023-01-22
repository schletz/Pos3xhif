# Reset aller lokalen Branches

Oft möchte man auf Knopfdruck den lokalen Stand des Repositories auf den Stand des remote Repositories
"ohne Rücksicht auf Verluste" bringen. Das bedeutet, dass lokale Änderungen gegebenenfalls gelöscht
werden. Das ist nützlich, wenn man z. B. am Laptop arbeitet, in das remote Repo committed und
zu Hause am PC weiterarbeiten will.

Lege dafür 2 Dateien im Hauptverzeichnis des Repositories an und kopiere den nachfolgenden
Inhalt dorthin.

**resetGit.cmd (Windows)**
```bat
@echo off
chcp 65001
echo Achtung: Alle lokalen Änderungen werden zurückgesetzt. Drücke CTRL+C zum Abbrechen.
pause

FOR /F "tokens=*" %%a IN ('git branch --show-current') DO (SET current_branch=%%a)

git fetch --all --prune
FOR /F "tokens=* delims=* " %%a IN ('git branch') DO (
    echo Reset branch %%a...
    git checkout %%a
    git clean -df
    git reset --hard origin/%%a
)
git checkout %current_branch%
echo You are in branch %current_branch%
```

**resetGit.sh (macOS, Linux)**
```bash
#!/bin/bash
# Setzt alle lokalen Branches zurück

current_branch=$(git branch --show-current)
for branch in $(git branch | tr '*' ' ')
do
    echo Reset branch $branch
    git checkout $branch
    git clean -df
    git reset --hard origin/$branch
done

git checkout $current_branch
echo "You are in Branch $current_branch"
```

Committe danach diese Dateien und spiele sie ins remote Repository. Wenn du die Datei zum Testen
ohne commit ausführst, wird sie natürlich verschwinden, da lokale Änderungen zurückgesetzt werden.


## Was passiert mit diesen Befehlen

- Zuerst wird mit *git fetch* der Stand des remote Repos auf den Rechner geladen, allerdings ohne
  die Dateien zu verändern (das ist der Unterschied zu *git pull*). Die Option *prune* löscht
  Branches, die es nicht mehr im remote Repository gibt.
- Danach durchläuft die Schleife alle branches, die von *git branch* zurückgegeben werden.
- Mit *git checkout* wird in jeden Branch gewechselt.
- *git clean* löscht alle lokalen Dateien, die nicht unter Versionsverwaltung stehen. Da die Option
  *-x* nicht gesetzt wird, werden ignorierte Dateien (definiert in *.gitignore*) nicht angetastet.
- Mit *git reset --hard* wird auf den Stand des remote Repos (origin) gesetzt.
- Am Ende wird in den vorher aktiven Branch gewechselt.

