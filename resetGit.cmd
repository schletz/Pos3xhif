echo Achtung: Alle lokalen Änderungen werden zurückgesetzt. Druecke CTRL+C zum Abbrechen.
pause
git fetch --all
git reset --hard origin/master
