# REQUIREMENTS
# pip install markdown
# pip install pygments

import markdown as md
import sys, re, requests, base64

# **************************************************************************************************
# Codiert eine Datei und gibt einen Base64 String zurück, der direkt als img src verwendet werden kann.
def encodeFile(filename):
    extension = re.search("^.+\.(?P<ext>.*)$", filename).group("ext")
    with open(filename, "rb") as file:
        mimeType = "image/svg+xml" if extension == "svg" else f'image/{extension}'
        content = file.read()
        contentString = base64.b64encode(content).decode("utf-8")
        return f'data:{mimeType};charset=utf-8;base64,{contentString}'
# **************************************************************************************************

if len(sys.argv) < 2:
    raise TypeError("Usage: python md2html.py filename")

filename = re.search("^(?P<name>.*)\.md$", sys.argv[1])
if filename is None:
    raise TypeError("Only md files are supported. Usage: python md2html.py filename")

mdFilename = filename.group()
htmlFilename = f'{filename.group("name")}.html'
title = filename.group("name")

# CSS aus dem Template lesen
# cssUrl = "https://gist.githubusercontent.com/tuzz/3331384/raw/fc0160dd7ea0b4a861533c4d6c232f56291796a3/github.css"
# cssUrl = "https://raw.githubusercontent.com/pxlrbt/markdown-css/master/markdown.css"
# cssUrl = "https://gist.githubusercontent.com/tuzz/3331384/raw/fc0160dd7ea0b4a861533c4d6c232f56291796a3/github.css"
# cssUrl = "https://latex.vercel.app/style.min.css"
# req = requests.get(cssUrl)
# cssString = req.text
# cssString = cssString.replace("./fonts", "https://latex.vercel.app/fonts")
cssString = """
    @page  
    { 
        size: auto;
        margin: 20mm 25mm !important;  
    } 
    html, body {
        font-size:16px;
        padding:0;
        margin:0;
        font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;      
        line-height: 1.3em;
    }
    h1, h2, h3, h4, h5, h6 {
        font-family: 'Segoe UI', sans-serif;
        font-weight: 500;
        break-after:avoid;
    }
    h1 {
        column-span: all;
        line-height: 1.3em;
        border-bottom: 1px solid gray;
    }
    h2 {
        column-span: all;
        background-color:#eee;
        padding:0.5rem;
        margin-left:-0.5rem;
    }
    p {
        widows:4;
        orphans: 4;
    }
    .codehilite {
        padding-left:0.5rem;
        overflow-x:auto;
        break-inside: avoid;
    }
    blockquote {
        border-left:2px solid black;
        margin-left:1rem;
        background-color:#EEE;
        padding:0.25rem;

    }
    blockquote p {
        margin-left:0.5rem;
    }
    sup {
        display: block;
        word-break: break-all;
        font-size:80%;
    }
    em {
        font-weight:500;
    }
    ul {
        padding-inline-start: 1rem;
    }
    table {
        border-collapse: collapse;
        min-width:70%;
    }
    th {
        text-align:left;
    }
    th, td {
        border-top:1px solid black;
        border-bottom:1px solid black;
        padding:0.25rem 0.5rem;
    }
    th {
        border-top:2px solid black;
        border-bottom:2px solid black;
    }
    @media screen and (min-width: 110em) {
        body {
            columns: 2;
            column-gap:5em;
            margin: 0rem 2rem;                
        }            
    }
    @media screen and (max-width: 109.999em) and (min-width: 55.001px) {
        body {
            margin-left:auto;
            margin-right:auto;
            max-width:50em;
        }
    }        
    @media screen and (max-width: 55em) {
        html,body {
            padding:0;
        }
    }            
"""
with open(mdFilename, "r", encoding="utf-8") as input_file:
    mdString = input_file.read()

mdString = mdString.replace("\\<", "&lt;").replace("\\>", "&gt;")


htmlString = md.markdown(mdString,
    output_format="html5",
    encoding='utf8',
    extensions=['tables', 'codehilite', 'fenced_code'],
    extension_configs={
        'fenced_code': {
        },
        'codehilite': {
            'linenums': False,
            'noclasses': True
        }
    }
)

# Bilder durch Base64 Content ersetzen.
images = map(lambda x: (
    {
        'element': x.group(),
        'file': x.group('filename'),
        'content': encodeFile(x.group('filename'))
    }), re.finditer("\<img [^\>]*src=\"(?P<filename>[^\"]+)\"[^\>]*\>", htmlString))

for image in images:
    htmlString = htmlString.replace(image["element"], f'<img src=\"{image["content"]}\">')

with open(htmlFilename, "w", encoding="utf-8", errors="xmlcharrefreplace") as output_file:
    output_file.write(f"""
<!DOCTYPE html>
<html lang="de">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title}</title>
    <style>
        {cssString}
    </style>
</head>    
<body>
    <main>
    """)
    output_file.write(htmlString)
    output_file.write("""
    </main>
</body>
</html>    
    """)
