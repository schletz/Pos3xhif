@echo off
REM https://stackoverflow.com/questions/203090/how-do-i-get-current-datetime-on-the-windows-command-line-in-a-suitable-format
REM mit Adaptierungen für das deutsche Datumsformat (geht nur bei deutschem Windows!)
For /f "tokens=1-3 delims=. " %%a in ('date /t') do (set mydate=%%c%%b%%a)
git add -A
git commit -a -m "Commit %mydate%"
git pull
IF ERRORLEVEL 1 GOTO error
git push
IF ERRORLEVEL 1 GOTO error
GOTO end
:error
echo Ein Fehler ist aufgetreten.
pause
:end
