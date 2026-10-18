@echo off
SET ZIP="C:\Program Files\7-Zip\7z.exe"
REM Aktuelles Verzeichnis ohne den Pfad davor lesen.
REM https://superuser.com/questions/160702/get-current-folder-name-by-a-dos-command
for %%I in (.) do set CurrDirName=%%~nxI
REM https://stackoverflow.com/questions/203090/how-do-i-get-current-datetime-on-the-windows-command-line-in-a-suitable-format
REM mit Adaptierungen f�r das deutsche Datumsformat (geht nur bei deutschem Windows!)
For /f "tokens=1-3 delims=/. " %%a in ('date /t') do (set mydate=%%c%%b%%a)
SET OUTPATH=..
REM Dateiname GfsDownloader20190107.7z z. B. generieren
SET FILENAME=%CurrDirName%%mydate%.7z

del /F /Q "%OUTPATH%\%CurrDirName%*.7z"
%ZIP% a -mx=9 "%OUTPATH%\%FILENAME%" "..\%CurrDirName%" -xr!.vs -xr!bin -xr!obj -xr!TestResults
pause
