#!/bin/bash
# Geht in alle Unterverzeichnisse und schreibt die Pullrequests in eine Textdatei.
# (c) 2025, Michael Schletz

OUTFILE="log.html"      # Ausgabedatei (wird ersetzt)

# **************************************************************************************************

if [ -z "$(command -v gh)" ]; then
    echo "gh cli tool not found. Please download github cli tool from https://cli.github.com"
    read
    exit 1
fi

BASE_DIR=$(pwd)
PULLREQUESTS="$BASE_DIR/pullrequests.txt"
PULLREQUESTS_PIVOT="$BASE_DIR/pullrequests_pivot.txt"
echo Write log to $PULLREQUESTS...

echo -e "REPO\tCREATE_DATE\tTITLE\tBRANCH\tREVIEW_REQUESTS\tSTATE\tURL\tREVIEW_DECISION"  > "$PULLREQUESTS"
for DIR in */ ; do
    cd "$BASE_DIR/$DIR"
    REPO=${DIR//\//}
    URL=$(git remote get-url origin)

    echo "Analyze $REPO"
    # PULL REQUESTS
    gh pr list -R $URL \
    --state all \
    --json url,mergeStateStatus,createdAt,updatedAt,state,title,reviewDecision,headRefName,reviewRequests \
    --template  '{{range .}}{{printf "'$REPO'\t%s\t%s\t%s\t%d\t%s\t%s\t%s\n" .createdAt .title .headRefName (len .reviewRequests) .state .url .reviewDecision}}{{end}}' >> "$PULLREQUESTS"
done
cd "$BASE_DIR"


# Schritt 1: Extrahiere Branches und die Repos
BRANCHES=$(awk -F'\t' 'NR > 1 {print $4}' "$PULLREQUESTS" | sort | uniq)
REPOS=$(awk -F'\t' 'NR > 1 {print $1}' "$PULLREQUESTS" | sort | uniq)

# Schritt 2: Erstelle die dynamische Kopfzeile
HEADER="REPO"
for BRANCH in $BRANCHES; do
    HEADER="${HEADER}\t${BRANCH}"
done
echo -e "$HEADER" > "$PULLREQUESTS_PIVOT"

# Schritt 3: Initialisiere eine leere Tabelle für die Repos und Branches
declare -A DATA

while IFS=$'\t' read -r REPO CREATE_DATE _ BRANCH _; do
    # Skip header row
    [[ $REPO == "REPO" ]] && continue
    # Berechne das Minimum von CREATE_DATE für jeden Branch und Repo
    KEY="${REPO}_${BRANCH}"
    if [[ -z "${DATA[$KEY]}" || "${CREATE_DATE}" < "${DATA[$KEY]}" ]]; then
        DATA[$KEY]="$CREATE_DATE"
    fi
done < <(tail -n +2 "$PULLREQUESTS")

# Schritt 4: Schreibe die Daten in die Datei
for REPO in $REPOS; do
    ROW="$REPO"
    for BRANCH in $BRANCHES; do
        KEY="${REPO}_${BRANCH}"
        # Verwende nur die ersten 10 Zeichen des Datums
        DATE=${DATA[$KEY]:0:10}
        ROW="${ROW}\t${DATE:-}"
    done
    echo -e "$ROW" >> "$PULLREQUESTS_PIVOT"
done

