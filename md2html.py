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
req = requests.get("https://raw.githubusercontent.com/microsoft/vscode/main/extensions/markdown-language-features/media/markdown.css")
cssString = req.text

with open(mdFilename, "r", encoding="utf-8") as input_file:
    mdString = input_file.read()

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
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title}</title>
    <style type="text/css">
        {cssString}
        @media screen and (min-width: 105em) {{
            main {{
                columns:50em 12;
                column-gap:5em;
            }}
        }}
        @media screen and (max-width: 104.999em) and (min-width: 55.001px) {{
            main {{
                margin-left:auto;
                margin-right:auto;
                max-width:50em;
            }}
        }}        
        @media screen and (max-width: 55em) {{
            html,body {{
                padding:0;
            }}
        }}                
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
