@echo off
chcp 65001
echo Achtung: Alle lokalen Änderungen werden zurückgesetzt. Drücke CTRL+C zum Abbrechen.
pause
git add -A
git fetch --all
git reset --hard origin/master
