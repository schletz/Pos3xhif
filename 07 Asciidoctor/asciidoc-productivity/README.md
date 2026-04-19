# AsciiDoc Productivity

![](https://raw.githubusercontent.com/Die-Spengergasse/course-pos-csharp_basics/master/07%20Asciidoctor/extension_screenshot_2103.png)

This extension turbocharges your AsciiDoc editing experience in Visual Studio Code, providing smart tools for handling images, source code snippets, data tables, LLM integrations, and PDF exports.

## 1. Insert source code from clipboard (Insert as source block)
Copy any code. Right-click in your AsciiDoc document and select _Insert as source block_.
An input line will open at the top where you can type the programming language (e.g., _csharp_, _java_, _python_). The code will then be inserted as a perfectly formatted AsciiDoc source block.

## 2. Insert TSV tables (Insert as tsv table)
Copy table-like data (e.g., directly from Excel) to your clipboard. Select _Insert as tsv table_ from the right-click menu.
The extension automatically detects the number of columns (based on the tabs) and generates a completed AsciiDoc table block in TSV format.

## 3. Insert image from local file (Insert image from file)
Do you want to embed an image located on your hard drive? Select _Insert image from file_.
A dialog window opens in the current folder. Select your image. The extension automatically calculates the _relative path_ from your document to the image and inserts the correct `image::path/to/image.png[]` syntax.
*Note: To ensure the path matches the .adoc file, you must save your document first.*

## 4. Save image from clipboard (Insert image from clipboard)
Have you taken a screenshot that is currently in your clipboard?
Select _Insert image from clipboard_. A save dialog will open. Give the image a name. The extension saves the image from memory to your hard drive and immediately inserts the code with the relative path into the document.
*Note: To ensure the path matches the .adoc file, you must save your document first.*

## 5. Download image from the internet (Insert image URL)
Copy the URL of an image (e.g., _https://example.com/image.png_) to the clipboard. Select _Insert image URL_.
The extension downloads the image from the internet, asks where you would like to save it locally, and then inserts it into the document along with the original source. This ensures that no images are lost if the website later goes offline.
*Note: To ensure the path matches the .adoc file, you must save your document first.*

## 6. Copy AsciiDoc table as TSV to clipboard
Highlight a table in AsciiDoc including the start and end markers (`|===`).
In the context menu, select _Copy AsciiDoc Table as TSV to clipboard_.
This copies the table as a tab-separated text to your clipboard, allowing you to easily paste it into spreadsheet software like Excel.

## 7. Import file directly as code block (File Explorer Feature)
This is a powerful feature for writing technical documentation:
In the _left-hand file tree view_ of VS Code (File Explorer), right-click on any code file (e.g., _.cs_, _.java_, _.py_) and select _Insert file as source block_.
The extension reads the entire file, automatically detects the programming language, calculates the relative path, and inserts a clickable link to the file along with the source code into your currently open AsciiDoc document.
*Note: To ensure the path matches the .adoc file, you must save your document first.*

## 8. Copy files and directories to the clipboard
When writing AI prompts, you often need to provide your source code as context. 
Select **one or multiple files and directories** in the File Explorer, right-click, and choose _Copy sources to clipboard_. You can also click the dedicated icon next to a directory name.
This recursively bundles your selected code into an XML structure optimized for LLMs, and will notify you exactly how many files were processed. 
_Note: Extractors are included for `.docx` and `.pdf` files. Ensure you add these to your included extensions if you want their text extracted!_

## 9. PDF generation
In the Explorer context menu, you can export an ADOC file to PDF by right-clicking on the file and selecting _Export as PDF_.

**Prerequisite: Docker must be installed and running.**
The conversion is executed using the `asciidoctor/docker-asciidoctor` Docker image. 
* **Linux:** Ensure your user is in the `docker` group (`sudo usermod -aG docker $USER`).
* **Troubleshooting:** Check if communication with the daemon is working by running `docker ps` in your terminal.

## 10. AI Features: Translation and Spellchecking
You can trigger AI-assisted edits directly from your editor or explorer:
* **Translate Selection:** Select text, right-click, and choose _LLM: Translate_.
* **Translate Entire File:** Right-click an `.adoc` file in the Explorer and choose _LLM: Translate entire file_.
* **Spellcheck:** Select text, right-click, and choose _LLM: Check spelling and grammar_. 

*Prerequisite: An OpenAI-compatible endpoint (like LM Studio, Ollama, or an OpenAI API key wrapper) is required. Configure this in your workspace settings.*

## 11. Live Preview with highlight.js and PlantUML
Click the **Preview Icon** in the top right corner of the editor title bar, or run _Open AsciiDoc Preview_ from the Command Palette.
The live preview natively renders integrated PlantUML diagrams (marked with `[plantuml]`), supports MathJax equations, and provides syntax highlighting via `highlight.js`. It closely resembles the final PDF output.

---

## 💡 Tips for a Smooth Workflow
* **Use the Command Palette:** All features can be triggered by pressing `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (Mac) and typing `AsciiDoc Productivity`.
* **Save Frequently:** Many features rely on calculating relative paths. Ensure your `.adoc` file is saved to your workspace before inserting images or files.
* **Local LLMs:** If you use Ollama or LM Studio locally, set your `completionsUrl` to your local endpoint (e.g., `http://127.0.0.1:1234/v1/chat/completions` for LM Studio).

---

## Configuration

You can customize the extension via your `settings.json`. Here are the available parameters and their default values:

```json
{
  "asciidoc-productivity.includeExtensions": "cs|csproj|java|rb|json|js|ts|jsx|tsx|py|txt|xml|adoc|md|cmd|sh|sql|yaml|puml",
  "asciidoc-productivity.excludeDirectories": ["bin", "obj", "node_modules"],
  "asciidoc-productivity.excludeFiles": ["package-lock.json"],
  "asciidoc-productivity.completionsUrl": "http://127.0.0.1:8000/v1/chat/completions",
  "asciidoc-productivity.llm": "LilaRest/gemma-4-31B-it-NVFP4-turbo",
  "asciidoc-productivity.maxOutputTokens": 4096,
  "asciidoc-productivity.defaultLanguage": "en-US"
}
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