@echo off
chcp 65001
echo Achtung: Alle lokalen Änderungen werden zurückgesetzt. Drücke CTRL+C zum Abbrechen.
pause

FOR /F "tokens=*" %%a IN ('git branch --show-current') DO (SET current_branch=%%a)

git fetch --all --prune
FOR /F "tokens=* delims=* " %%a IN ('git branch') DO (
    echo Reset branch %%a...
    git checkout %%a > nul 2>&1
    REM git clean -df
    git reset --hard origin/%%a > nul 2>&1
)
git checkout %current_branch% > nul 2>&1
echo You are in branch %current_branch%
