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
