# AsciiDoc Productivity

## 1. Insert source code from clipboard (Insert as source block)
Copy any code. Right-click in your AsciiDoc document and select _Insert as source block_.
An input line will open at the top where you can type the programming language (e.g., _csharp_, _java_, _python_). The code will then be inserted as a perfectly formatted AsciiDoc source block.

## 2. Insert TSV tables (Insert as tsv table)
Copy table-like data (e.g., directly from Excel) to your clipboard. Select _Insert as tsv table_ from the right-click menu.
The extension automatically detects the number of columns (based on the tabs) and generates a completed AsciiDoc table block in TSV format.

## 3. Insert image from local file (Insert image from file)
Do you want to embed an image located on your hard drive? Select _Insert image from file_.
A dialog window opens in the current folder. Select your image. The extension automatically calculates the _relative path_ from your document to the image and inserts the correct _image::path/to/image.png[]_ syntax.
Note: To ensure the path matches the .adoc file, you must save the file first.

## 4. Save image from clipboard (Insert image from clipboard)
Have you taken a screenshot that is currently in your clipboard?
Select _Insert image from clipboard_. A save dialog will open. Give the image a name. The extension saves the image from memory to your hard drive and immediately inserts the code with the relative path into the document.
Note: To ensure the path matches the .adoc file, you must save the file first.

## 5. Download image from the internet (Insert image URL)
Copy the URL of an image (e.g., _https://example.com/image.png_) to the clipboard. Select _Insert image URL_.
The extension downloads the image from the internet, asks where you would like to save it locally, and then inserts it into the document along with the original source. This ensures that no images are lost if the website later goes offline.
Note: To ensure the path matches the .adoc file, you must save the file first.

## 6. Copy AsciiDoc table as TSV to clipboard
Highlight a table in AsciiDoc including the start and end markers (_|===_).
In the context menu, there is an option _Copy AsciiDoc Table as TSV to clipboard_.
This copies the table as a tab-separated table to the clipboard.
This data can then be pasted, for example, into Excel.

## 7. Import file directly as code block (File Explorer Feature)
This is the most powerful feature for programmers:
In the _left-hand file tree view_ of VS Code (File Explorer), go to a code file (e.g., _.cs_, _.java_, _.py_). Right-click _on the file_ and select _Insert as source block_.
The extension reads the entire file, automatically detects the programming language, calculates the relative path, and inserts a clickable link to the file along with the source code into your currently open AsciiDoc document.
Note: To ensure the path matches the .adoc file, you must save the file first.

## 8. Copy files of a directory to the clipboard
Source code is required in the context window, especially for AI prompts.
When clicking on a *directory* in the File Explorer, a menu item _Copy sources to clipboard_ appears.
If you want to copy the current folder, you can click the button next to the directory name in the Explorer (see screenshot).
When prompting, check whether the entire code was copied.
Especially in the Free Plan, the context window is very limited.

## 9. PDF generation

In the Explorer context menu, you can export an ADOC file to PDF.
Right-click on the file.

**Prerequisite: Docker must be installed**.
The conversion is executed with the Docker image *asciidoctor/docker-asciidoctor*.

## 10. AI Features: LLM translate and LLM check spelling and grammar

You can translate a selected text or an entire file.
_LLM: Translate_ is available in the context menu of the Editor and Explorer.
_LLM: Check spelling and grammar_ is available when you select text in a document.
An OpenAI compatible endpoint such as LM Studio, Ollama, etc., is required for these features.

## Configuration

The following settings can be set in the _settings.json_ file (examples):

```json
"asciidoc-productivity.includeExtensions": "cs|csproj|java|rb|json|js|ts|jsx|tsx|py|txt|xml|adoc|md|cmd|sh|sql|yaml|puml",
"asciidoc-productivity.excludeDirectories": ["bin", "obj", "node_modules", "TestResults"],
"asciidoc-productivity.excludeFiles": ["package-lock.json"],
"asciidoc-productivity.completionsUrl": "http://127.0.0.1:8000/v1/chat/completions",
"asciidoc-productivity.llm": "LilaRest/gemma-4-31B-it-NVFP4-turbo",
"asciidoc-productivity.maxOutputTokens": 4096,
```

### For copy files to clipboard (explorer menu)

The app asks which extensions should be considered.
The default is read from the _settings.json_ file (_includeExtensions_).

An extractor is available for the _docx_ and _pdf_ extensions (_mammoth_ for Word files, _pdfreader_ for PDF files).
To copy these files as well, you must add the _docx_ and _pdf_ extensions in the settings or enter them before copying.

Files larger than 10 MB will not be read.

## Extending the app and Creating the VSIX File

The source code can be found at https://github.com/Die-Spengergasse/course-pos-csharp_basics/tree/master/07%20Asciidoctor/asciidoc-productivity.

**package.json:**
Defines the menu entries in the _contributes_ key and refers to the methods in the extension.

**src/extension.ts:**
The actual extension.
It is loaded at the start.
When a menu item is clicked, the corresponding method is called.

**src/EditorService.ts:**
A classic service file used to summarize the editor methods.

**src/ConfigurationService.ts:**
Reads the configuration from the _settings.json_ file and provides it to the application.

**src/LLMService.ts:**
A wrapper for the fetch requests to the OpenAI-compatible endpoint for LLM prompts.

### Debugging

If you open the extension directory with _Open Folder_, you can simply open a VS Code window with _F5_ or _Run -> Start Debugging_ and test your extension.

### Exporting to a VSIX File

To build the extension yourself and install it in Visual Studio Code (VS Code), the global Node.js tool _@vscode/vsce_ is required.
You can install it via the console with _npm install -g @vscode/vsce_.
Now go to the extension folder (where the _package.json_ is located).
Then execute the following command to generate the finished installation file:

```bash
vsce package --allow-missing-repository
```

After the process, you will find a new file with the extension _.vsix_ (e.g., _asciidoc-productivity-1.0.0.vsix_) in your folder.
You can now install this in VS Code.