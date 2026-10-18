@echo off
REM ===============================================================
REM Skriptname: convert_adoc.cmd
REM Autor: Michael Schletz
REM Datum: 24.04.2026
REM Beschreibung: Dieses Skript konvertiert AsciiDoc-Dateien (adoc)
REM               in PDF, DOCX oder Markdown (md) mithilfe von Docker.
REM               Unterstützt die Weitergabe beliebiger Parameter (KWARGS).
REM ===============================================================
setlocal

REM Überprüfen, ob Docker läuft.
docker info >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Docker scheint nicht zu laufen. Bitte starte Docker und versuche es erneut.
    pause
    exit /b 1
)

REM Überprüfen, ob das Docker-Image asciidoctor-pandoc vorhanden ist
docker image inspect asciidoctor-pandoc >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [INFO] Das Docker-Image "asciidoctor-pandoc" ist nicht vorhanden. Es wird nun erstellt.
    REM Dockerfile-Inhalt schreiben
    (
        echo FROM asciidoctor/docker-asciidoctor
        echo RUN apk add --no-cache pandoc
        echo WORKDIR /documents
        echo CMD ["sh"]
    ) > asciidoctor.dockerfile
    docker build -t asciidoctor-pandoc -f asciidoctor.dockerfile .
    del asciidoctor.dockerfile
    echo Dockerimage erstellt. Druecke eine Taste um fortzufahren.
    pause
)

REM Überprüfen der Eingabeparameter
if "%~1"=="" (
    echo [ERROR] Es wurde keine Eingabedatei angegeben.
    echo Nutzung: convert_adoc.cmd "eingabedatei.adoc" "ausgabedatei.[pdf|docx|md|html]" [weitere asciidoctor Parameter...]
    exit /b 1
)
if "%~2"=="" (
    echo [ERROR] Es wurde keine Ausgabedatei angegeben.
    echo Nutzung: convert_adoc.cmd "eingabedatei.adoc" "ausgabedatei.[pdf|docx|md|html]" [weitere asciidoctor Parameter...]
    exit /b 1
)

REM Pflicht-Parameter sichern
set "INPUT_FILE=%~1"
set "OUTPUT_FILE=%~2"

REM Die ersten beiden Parameter (Infile/Outfile) aus dem Stack entfernen
shift
shift

REM Alle verbleibenden Parameter in KWARGS einsammeln
set "KWARGS="
:CollectArgs
if "%~1"=="" goto DoneArgs
set "KWARGS=%KWARGS% %1"
shift
goto CollectArgs
:DoneArgs

REM Führendes Leerzeichen in KWARGS entfernen
if not "%KWARGS%"=="" set "KWARGS=%KWARGS:~1%"

REM Erweiterung der Ausgabedatei extrahieren
for %%x in ("%OUTPUT_FILE%") do set EXT=%%~xx
set EXT=%EXT:~1%

REM Basename für die Ausgabedatei
for %%x in ("%OUTPUT_FILE%") do set BASENAME=%%~nx

REM Basename für die Eingabedatei
for %%x in ("%INPUT_FILE%") do set BASENAME_INPUT_FILE=%%~nx

REM Automatisches Theme prüfen
set "AUTO_THEME="
REM Prüfen ob --theme in KWARGS vorkommt. Nur wenn nicht, Fallback nutzen.
echo "%KWARGS%" | find /I "--theme" >nul
if errorlevel 1 (
    if exist "%BASENAME_INPUT_FILE%.yml" (
        set "AUTO_THEME=--theme '%BASENAME_INPUT_FILE%.yml'"
    )
)

REM Entscheidung basierend auf der Erweiterung
if /i "%EXT%"=="pdf" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach PDF...
    if not "%AUTO_THEME%"=="" echo [INFO] Verwende Auto-Theme %AUTO_THEME%
    if not "%KWARGS%"=="" echo [INFO] Zusaetzliche Parameter KWARGS %KWARGS%
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor-pdf %AUTO_THEME% %KWARGS% -r asciidoctor-mathematical -r asciidoctor-diagram -a allow-uri-read -a stem -a mathematical-format=svg -o '%BASENAME%.pdf' '%INPUT_FILE%' && rm -rf .asciidoctor"

) else if /i "%EXT%"=="docx" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach DOCX...
    if not "%KWARGS%"=="" echo [INFO] Zusaetzliche Parameter KWARGS %KWARGS%
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b docbook5 %KWARGS% -r asciidoctor-diagram -a data-uri -o - '%INPUT_FILE%' | pandoc -f docbook -t docx --highlight-style pygments -o '%BASENAME%.docx' && rm -rf .asciidoctor"

) else if /i "%EXT%"=="md" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach Markdown...
    if not "%KWARGS%"=="" echo [INFO] Zusaetzliche Parameter KWARGS %KWARGS%
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b docbook5 %KWARGS% -r asciidoctor-diagram -a data-uri -o - '%INPUT_FILE%' | pandoc -f docbook -t gfm --highlight-style pygments -o '%BASENAME%.md' && rm -rf .asciidoctor"

) else if /i "%EXT%"=="html" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach HTML...
    if not "%KWARGS%"=="" echo [INFO] Zusaetzliche Parameter KWARGS %KWARGS%
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b html %KWARGS% -r asciidoctor-diagram -a data-uri -o '%BASENAME%.html' '%INPUT_FILE%' && rm -rf .asciidoctor"

) else (
    echo [ERROR] Unbekannte Ausgabedateierweiterung "%EXT%". Unterstuetzt sind pdf, docx, md, html
    exit /b 1
)

echo [INFO] Fertig!
exit /b 0
