@echo off
SET ZIP="C:\Program Files\7-Zip\7z.exe"
REM Aktuelles Verzeichnis ohne den Pfad davor lesen.
REM https://superuser.com/questions/160702/get-current-folder-name-by-a-dos-command
for %%I in (.) do set CurrDirName=%%~nxI
REM https://stackoverflow.com/questions/203090/how-do-i-get-current-datetime-on-the-windows-command-line-in-a-suitable-format
REM mit Adaptierungen für das deutsche Datumsformat (geht nur bei deutschem Windows!)
For /f "tokens=1-3 delims=. " %%a in ('date /t') do (set mydate=%%c%%b%%a)
SET OUTPATH=..
REM Dateiname GfsDownloader20190107.7z z. B. generieren
SET FILENAME=%CurrDirName%%mydate%.zip

del /F /Q "%OUTPATH%\%CurrDirName%*.zip"
%ZIP% a -tzip -mx=9 "%OUTPATH%\%FILENAME%" "..\%CurrDirName%" -xr!packages -xr!TestResults -xr!.vs -x!Release -x!Debug -xr!bin -xr!obj
pause
