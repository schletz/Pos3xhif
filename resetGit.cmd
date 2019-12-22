@echo off
chcp 65001
echo Achtung: Alle lokalen Žnderungen werden zurckgesetzt. Drcke CTRL+C zum Abbrechen.
pause
git fetch --all
git reset --hard origin/master
