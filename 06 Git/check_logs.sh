#!/bin/bash

# Geht in alle unterverzeichnisse und schreibt die Commits in die Datei log.html
BASE_DIR=$(pwd)
rm "log.html" 2> /dev/null
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
            margin: 0;
            padding: 0;
        }
        table {
            table-layout: fixed;
            width: 100%;
            border-collapse: collapse;
        }
        h1 { background-color: hsl(0, 0%, 90%); padding:0 0.25em; }
        h1 a {font-size: 50%; font-weight: normal; color:inherit; }
        td { border:1px solid hsl(0, 0%, 80%); padding: 0em 0.25em; vertical-align: top; }
        td:nth-child(1) { width: 12em; }
        td:nth-child(2) { width: 10em; }
        td div {white-space:nowrap; overflow:hidden; text-overflow: ellipsis;}
        body  { columns: 50em 99;  }
        body > div { break-inside: avoid; padding:5px; }
    </style>    
</head>
<body>
' > log.html

for DIR in */ ; do
    cd "$BASE_DIR/$DIR"
    REPO=${DIR//\//}
    URL=$(git remote get-url origin)
    echo "Fetch $REPO"
    git fetch --all --prune
    echo "<div>" >> "../log.html"
    echo "<h1>$REPO <a href=\"$URL\" target=\"_blank\">$URL</a></h1>" >> "../log.html"
    #git remote -v >> "../log.html"
    echo "<table>" >> "../log.html"
    #git log origin --all --since="30 days ago" --date=iso-strict --pretty=format:"%ad|%an (%al)|%s" | column -t -s '|' >> "../log.html"
    git log origin --all --since="30 days ago" --date=iso-strict --pretty=format:"<tr><td><div>%ad</div></td><td><div>%an (%al)</div></td><td>%s</td></tr>" >> "../log.html"
    echo "</table>" >> "../log.html"
    echo "</div>" >> "../log.html"
done

echo '
    </body>
</html>' >> log.html
