@echo off
REM ===============================================================
REM Skriptname: convert_adoc.cmd
REM Autor: Michael Schletz
REM Datum: 23.11.2024
REM Beschreibung: Dieses Skript konvertiert AsciiDoc-Dateien (adoc)
REM               in PDF, DOCX oder Markdown (md) mithilfe von Docker.
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
    echo Dockerimage erstellt. Drücke eine Taste um fortzufahren.
    pause
)

REM Überprüfen der Eingabeparameter
if "%~1"=="" (
    echo [ERROR] Es wurde keine Eingabedatei angegeben.
    echo Nutzung: convert_adoc.cmd "eingabedatei.adoc" "ausgabedatei.[pdf|docx|md|html]"
    exit /b 1
)
if "%~2"=="" (
    echo [ERROR] Es wurde keine Ausgabedatei angegeben.
    echo Nutzung: convert_adoc.cmd "eingabedatei.adoc" "ausgabedatei.[pdf|docx|md|html]"
    exit /b 1
)
if "%~3"=="--processor" (
    if not exist "%~4" (
        echo [ERROR] Das Verzeichnis %~4 existiert nicht.
        exit /b 1
    ) else (
        set PROCESSOR_DIR=%~4
    )
)

REM Parameter setzen
set INPUT_FILE=%~1
set OUTPUT_FILE=%~2

REM Erweiterung der Ausgabedatei extrahieren
for %%x in ("%OUTPUT_FILE%") do set EXT=%%~xx
set EXT=%EXT:~1%

REM Basename für die Ausgabedatei
for %%x in ("%OUTPUT_FILE%") do set BASENAME=%%~nx

REM Basename für die Eingabedatei
for %%x in ("%INPUT_FILE%") do set BASENAME_INPUT_FILE=%%~nx

REM Überprüfen, ob theme.yml existiert
if exist "%BASENAME_INPUT_FILE%.yml" (
    set THEME_OPTION=--theme '%BASENAME_INPUT_FILE%.yml'
) else (
    set THEME_OPTION=
)

REM Wurde ein Processor angegeben, rufen wir ihn auf
if not "%PROCESSOR_DIR%"=="" (
    echo [INFO] Führe Processor aus...
    dotnet restore "%PROCESSOR_DIR%" --no-cache
    dotnet run --project "%PROCESSOR_DIR%" -- "%INPUT_FILE%" > "%BASENAME%.processed.adoc"
    if %ERRORLEVEL% neq 0 (
        echo [ERROR] Fehler beim Ausführen des Processors.
        exit /b 1
    )
    set INPUT_FILE=%BASENAME%.processed.adoc
)

REM Entscheidung basierend auf der Erweiterung
if /i "%EXT%"=="pdf" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach PDF...
    if not "%THEME_OPTION%"=="" (
        echo [INFO] Verwende Theme Paramter %THEME_OPTION%
    )
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor-pdf %THEME_OPTION% -r asciidoctor-mathematical -r asciidoctor-diagram -a allow-uri-read -a stem -a mathematical-format=svg -o '%BASENAME%.pdf' '%INPUT_FILE%' && rm -rf .asciidoctor"
) else if /i "%EXT%"=="docx" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach DOCX...
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b docbook5 -r asciidoctor-diagram -a data-uri -o - '%INPUT_FILE%' | pandoc -f docbook -t docx --highlight-style pygments -o '%BASENAME%.docx' && rm -rf .asciidoctor"
) else if /i "%EXT%"=="md" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach Markdown...
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b docbook5 -r asciidoctor-diagram -a data-uri -o - '%INPUT_FILE%' | pandoc -f docbook -t gfm --highlight-style pygments -o '%BASENAME%.md' && rm -rf .asciidoctor"
) else if /i "%EXT%"=="html" (
    echo [INFO] Konvertiere "%INPUT_FILE%" nach HTML...
    docker run -i --rm -v "%cd%":/documents asciidoctor-pandoc ^
        sh -c "asciidoctor -b html -r asciidoctor-diagram -a data-uri -o '%BASENAME%.html' '%INPUT_FILE%' && rm -rf .asciidoctor"
) else (
    echo [ERROR] Unbekannte Ausgabedateierweiterung "%EXT%". Unterstützt sind: pdf, docx, md, html
    exit /b 1
)

echo [INFO] Fertig!
exit /b 0
