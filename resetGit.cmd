@echo off
FOR /F "tokens=2-2 delims= " %%a in ('git branch') do (set branch=%%a)
chcp 65001
echo Achtung: Alle lokalen Änderungen im Branch %branch% werden zurückgesetzt. Drücke CTRL+C zum Abbrechen.
pause
git add -A
git fetch --all
git reset --hard origin/%branch%
IF ERRORLEVEL 1 GOTO error
GOTO end
:error
echo Ein Fehler ist aufgetreten.
pause
:end


