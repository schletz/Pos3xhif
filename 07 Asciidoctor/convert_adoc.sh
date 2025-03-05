#!/bin/bash

# ===============================================================
# Skriptname: convert_adoc.sh
# Autor: Michael Schletz
# Datum: 23.11.2024
# Beschreibung: Dieses Skript konvertiert AsciiDoc-Dateien (adoc)
#               in PDF, DOCX, Markdown (md) oder HTML mithilfe von Docker.
# ===============================================================

# Überprüfen, ob Docker läuft.
if ! docker info > /dev/null 2>&1; then
    echo "[ERROR] Docker scheint nicht zu laufen. Bitte starte Docker und versuche es erneut."
    exit 1
fi

# Überprüfen, ob das Docker-Image asciidoctor-pandoc vorhanden ist
if ! docker image inspect asciidoctor-pandoc > /dev/null 2>&1; then
    echo "[INFO] Das Docker-Image 'asciidoctor-pandoc' ist nicht vorhanden. Es wird nun erstellt."
    # Dockerfile-Inhalt erstellen und Image bauen
    cat <<EOF > asciidoctor.dockerfile
FROM asciidoctor/docker-asciidoctor
RUN apk add --no-cache pandoc
WORKDIR /documents
CMD ["sh"]
EOF
    docker build -t asciidoctor-pandoc -f asciidoctor.dockerfile .
    rm asciidoctor.dockerfile
    echo "[INFO] Dockerimage wurde erfolgreich erstellt."
fi

# Überprüfen der Eingabeparameter
if [ -z "$1" ]; then
    echo "[ERROR] Es wurde keine Eingabedatei angegeben."
    echo "Nutzung: ./convert_adoc.sh eingabedatei.adoc ausgabedatei.[pdf|docx|md|html]"
    exit 1
fi

if [ -z "$2" ]; then
    echo "[ERROR] Es wurde keine Ausgabedatei angegeben."
    echo "Nutzung: ./convert_adoc.sh eingabedatei.adoc ausgabedatei.[pdf|docx|md|html]"
    exit 1
fi

if [ "$3" == "--processor" ]; then
    if [ ! -d "$4" ]; then
        echo "[ERROR] Das Verzeichnis $4 existiert nicht."
        exit 1
    else
        PROCESSOR_DIR="$4"
    fi
fi

# Parameter setzen
INPUT_FILE="$1"
OUTPUT_FILE="$2"

# Erweiterung der Ausgabedatei extrahieren
EXT="${OUTPUT_FILE##*.}"
BASENAME="${OUTPUT_FILE%.*}"

# Überprüfen, ob ein Theme-File existiert
if [ -f "${BASENAME}.yml" ]; then
    THEME_OPTION="--theme \"${BASENAME}.yml\""
else
    THEME_OPTION=""
fi

# Wurde ein Processor angegeben, rufen wir ihn auf
if [ -n "$PROCESSOR_DIR" ]; then
    echo "[INFO] Führe Processor aus..."
    dotnet restore "$PROCESSOR_DIR" --no-cache
    if ! dotnet run --project "$PROCESSOR_DIR" -- "$INPUT_FILE" > "${BASENAME}.processed.adoc"; then
        echo "[ERROR] Fehler beim Ausführen des Processors."
        exit 1
    fi
    INPUT_FILE="${BASENAME}.processed.adoc"
fi

# Entscheidung basierend auf der Erweiterung
case "$EXT" in
    pdf)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach PDF..."
        [ -n "$THEME_OPTION" ] && echo "[INFO] Verwende Theme-Parameter $THEME_OPTION"
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor-pdf $THEME_OPTION -r asciidoctor-mathematical -r asciidoctor-diagram -a allow-uri-read -a stem -a mathematical-format=svg -o '${BASENAME}.pdf' \"$INPUT_FILE\" && rm -rf .asciidoctor"
        ;;
    docx)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach DOCX..."
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b docbook5 -r asciidoctor-diagram -a data-uri -o - \"$INPUT_FILE\" | pandoc -f docbook -t docx --highlight-style pygments -o '${BASENAME}.docx' && rm -rf .asciidoctor"
        ;;
    md)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach Markdown..."
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b docbook5 -r asciidoctor-diagram -a data-uri -o - \"$INPUT_FILE\" | pandoc -f docbook -t gfm --highlight-style pygments -o '${BASENAME}.md' && rm -rf .asciidoctor"
        ;;
    html)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach HTML..."
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b html -r asciidoctor-diagram -a data-uri -o '${BASENAME}.html' \"$INPUT_FILE\" && rm -rf .asciidoctor"
        ;;
    *)
        echo "[ERROR] Unbekannte Ausgabedateierweiterung '$EXT'. Unterstützt sind: pdf, docx, md, html"
        exit 1
        ;;
esac

echo "[INFO] Fertig!"
exit 0
