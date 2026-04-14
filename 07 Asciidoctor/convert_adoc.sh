#!/bin/bash

# ===============================================================
# Skriptname: convert_adoc.sh
# Autor: Michael Schletz
# Datum: 23.11.2024
# Beschreibung: Dieses Skript konvertiert AsciiDoc-Dateien (adoc)
#               in PDF, DOCX, Markdown (md) oder HTML mithilfe von Docker.
#               Unterstützt die Weitergabe beliebiger Parameter (KWARGS).
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
    echo "Nutzung: ./convert_adoc.sh eingabedatei.adoc ausgabedatei.[pdf|docx|md|html] [weitere asciidoctor Parameter...]"
    exit 1
fi

if [ -z "$2" ]; then
    echo "[ERROR] Es wurde keine Ausgabedatei angegeben."
    echo "Nutzung: ./convert_adoc.sh eingabedatei.adoc ausgabedatei.[pdf|docx|md|html] [weitere asciidoctor Parameter...]"
    exit 1
fi

# Pflicht-Parameter setzen
INPUT_FILE="$1"
OUTPUT_FILE="$2"

# Die ersten beiden Parameter ($1 und $2) verwerfen
shift 2

# Alle verbleibenden Parameter in KWARGS speichern
KWARGS="$@"

# Erweiterung der Ausgabedatei extrahieren
EXT="${OUTPUT_FILE##*.}"

# Den reinen Dateinamen ohne Pfad und Endung extrahieren
BASENAME=$(basename "${OUTPUT_FILE%.*}")
BASENAME_INPUT_FILE=$(basename "${INPUT_FILE%.*}")

# Automatisches Theme prüfen
AUTO_THEME=""
# Prüfen ob --theme in KWARGS vorkommt. Nur wenn nicht, Fallback nutzen.
if [[ ! "$KWARGS" == *"--theme"* ]]; then
    if [ -f "${BASENAME_INPUT_FILE}.yml" ]; then
        AUTO_THEME="--theme '${BASENAME_INPUT_FILE}.yml'"
    fi
fi

# Entscheidung basierend auf der Erweiterung
case "$EXT" in
    pdf|PDF)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach PDF..."
        [ -n "$AUTO_THEME" ] && echo "[INFO] Verwende Auto-Theme: $AUTO_THEME"
        [ -n "$KWARGS" ] && echo "[INFO] Zusätzliche Parameter (KWARGS): $KWARGS"
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor-pdf $AUTO_THEME $KWARGS -r asciidoctor-mathematical -r asciidoctor-diagram -a allow-uri-read -a stem -a mathematical-format=svg -o '${BASENAME}.pdf' \"$INPUT_FILE\" && rm -rf .asciidoctor"
        ;;
    docx|DOCX)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach DOCX..."
        [ -n "$KWARGS" ] && echo "[INFO] Zusätzliche Parameter (KWARGS): $KWARGS"
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b docbook5 $KWARGS -r asciidoctor-diagram -a data-uri -o - \"$INPUT_FILE\" | pandoc -f docbook -t docx --highlight-style pygments -o '${BASENAME}.docx' && rm -rf .asciidoctor"
        ;;
    md|MD)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach Markdown..."
        [ -n "$KWARGS" ] && echo "[INFO] Zusätzliche Parameter (KWARGS): $KWARGS"
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b docbook5 $KWARGS -r asciidoctor-diagram -a data-uri -o - \"$INPUT_FILE\" | pandoc -f docbook -t gfm --highlight-style pygments -o '${BASENAME}.md' && rm -rf .asciidoctor"
        ;;
    html|HTML)
        echo "[INFO] Konvertiere \"$INPUT_FILE\" nach HTML..."
        [ -n "$KWARGS" ] && echo "[INFO] Zusätzliche Parameter (KWARGS): $KWARGS"
        docker run -i --rm -v "$(pwd)":/documents asciidoctor-pandoc \
            sh -c "asciidoctor -b html $KWARGS -r asciidoctor-diagram -a data-uri -o '${BASENAME}.html' \"$INPUT_FILE\" && rm -rf .asciidoctor"
        ;;
    *)
        echo "[ERROR] Unbekannte Ausgabedateierweiterung '$EXT'. Unterstützt sind: pdf, docx, md, html"
        exit 1
        ;;
esac

echo "[INFO] Fertig!"
exit 0
