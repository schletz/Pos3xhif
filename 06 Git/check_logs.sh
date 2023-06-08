#!/bin/bash
# Geht in alle Unterverzeichnisse und schreibt die Commits,
# Pullrequests und Statistiken in eine HTML Datei.
# (c) 2023, Michael Schletz

DAYS_AGO=30             # Die commits der letzten n Tage auslesen.
OUTFILE="log.html"      # Ausgabedatei (wird ersetzt)

# **************************************************************************************************

if [ -z "$(command -v gh)" ]; then
    echo "gh cli tool not found. Please download github cli tool from https://cli.github.com"
    read
    exit 1
fi

BASE_DIR=$(pwd)
OUTFILE_ABS="$BASE_DIR/$OUTFILE"
echo Write log to $OUTFILE_ABS...

echo '<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Git logs</title>
    <style>
        html,body { 
            font-family: Gill Sans, Gill Sans MT, Calibri, Trebuchet MS, sans-serif;
            font-size: 12px;            
            margin: 0; padding: 0;
        }
        table { table-layout: fixed; width: 100%; border-collapse: collapse; }
        h1 { background-color: hsl(0, 0%, 90%); padding:0 0.25em; }
        h1 a {font-size: 50%; font-weight: normal; }
        a { color:inherit; }
        td { border:1px solid hsl(0, 0%, 80%); padding: 0em 0.25em; vertical-align: top; word-break: break-all; }
        td:nth-child(1) { width: 12em; }
        td:nth-child(2) { width: 10em; }
        td div {white-space:nowrap; overflow:hidden; text-overflow: ellipsis;}
        body  { columns: 50em 99;  }
        div.repo { break-inside: avoid; padding:5px; }
        .pullrequests_open { background-color: hsl(60, 100%, 90%); }
        .pullrequests_merged { background-color: hsl(120, 50%, 90%); }
        .branches, .author_summary { font-style: italic; margin-top:0.25rem; }
        tr[data-wants_review] { background-color: hsl(60, 100%, 50%); }
        tr[data-wants_review="0"] { background-color: hsl(60, 100%, 90%); }
        .legend { display: flex; column-gap: 1em; }
        .generated { margin-top: 1rem; }
    </style>    
</head>
<body>
' > "$OUTFILE_ABS"

COUNT=0
for DIR in */ ; do
    ((COUNT++))
    cd "$BASE_DIR/$DIR"
    REPO=${DIR//\//}
    URL=$(git remote get-url origin)

    echo "Fetch $REPO"
    git gc --prune=now
    git fetch --all --prune

    echo '<div class="repo">' >> "$OUTFILE_ABS"
    echo "<h1>$COUNT: $REPO <a href=\"$URL\" target=\"_blank\">$URL</a></h1>" >> "$OUTFILE_ABS"

    BRANCHES=$(git branch | tr '*' ' ')
    if [ -z "$BRANCHES" ]; then
        echo "Repo $REPO has no branches, it is not initialized."
        echo "</div>" >> "$OUTFILE_ABS"
        continue
    fi

    # OPEN PULL REQUESTS
    echo '<table class="pullrequests_open">' >> "$OUTFILE_ABS"
    gh pr list -R $URL --state open --json url,mergeStateStatus,createdAt,updatedAt,state,title,reviewDecision,headRefName,reviewRequests --template \
      '{{range .}}{{printf "<tr data-wants_review=\"%d\"><td>%s</td><td><div><a href=\"%s\" target=\"_blank\">%s</a></div></td><td>%s (wants review: %d, Branch %s, %s, %s)</td></tr>\n" (len .reviewRequests) .createdAt .url .url .title (len .reviewRequests) .headRefName .state .reviewDecision}}{{end}}' >> "$OUTFILE_ABS"
    echo '</table>' >> "$OUTFILE_ABS"

    # MERGED PULL REQUESTS
    echo '<table class="pullrequests_merged">' >> "$OUTFILE_ABS"
    gh pr list -R $URL --state merged --json url,mergeStateStatus,createdAt,state,title,reviewDecision,headRefName --template \
      '{{range .}}{{printf "<tr><td>%s</td><td><div><a href=\"%s\" target=\"_blank\">%s</a></div></td><td>%s (Branch %s, %s)</td></tr>\n" .createdAt .url .url .title .headRefName .state}}{{end}}' >> "$OUTFILE_ABS"
    echo '</table>' >> "$OUTFILE_ABS"

    # COMMITS
    echo '<table class="commits">' >> "$OUTFILE_ABS"
    #git log origin --all --since="30 days ago" --date=iso-strict --pretty=format:"%ad|%an (%al)|%s" | column -t -s '|' >> "$OUTFILE_ABS"
    git log origin --all --since="$DAYS_AGO days ago" --date=iso-strict --pretty=format:"<tr><td><div>%ad</div></td><td><div>%an (%al)</div></td><td>%s</td></tr>" >> "$OUTFILE_ABS"
    echo "</table>" >> "$OUTFILE_ABS"

    # LIST BRANCHES
    echo '<div class="branches"><strong>Branches:</strong> ' >> "$OUTFILE_ABS"
    git branch -r --format="%(refname:short)" >> "$OUTFILE_ABS"
    echo '</div>' >> "$OUTFILE_ABS"

    # LIST AUTHOR SUMMARY
    echo '<div class="author_summary"><strong>Authors:</strong> ' >> "$OUTFILE_ABS"
    git shortlog -sn --all | sed -E 's/([0-9]+)\s+(.*)+/\2 (\1)/' >> "$OUTFILE_ABS"
    echo '</div>' >> "$OUTFILE_ABS"
    echo "</div>" >> "$OUTFILE_ABS"
done
echo '<div class="legend">' >> "$OUTFILE_ABS"
echo '<div><span style="color: hsl(60, 100%, 50%); font-size:200%; ">&#9632;</span> open pull request, wants review</div>' >> "$OUTFILE_ABS"
echo '<div><span style="color: hsl(60, 100%, 90%); font-size:200%; ">&#9632;</span> open pull request</div>' >> "$OUTFILE_ABS"
echo '<div><span style="color: hsl(120, 50%, 90%); font-size:200%; ">&#9632;</span> merged pull request</div>' >> "$OUTFILE_ABS"
echo '</div>' >> "$OUTFILE_ABS"
echo "<div class="generated">Generated at $(date -Iseconds)</div>" >> "$OUTFILE_ABS"
echo '</body></html>' >> "$OUTFILE_ABS"
cd "$BASE_DIR"
