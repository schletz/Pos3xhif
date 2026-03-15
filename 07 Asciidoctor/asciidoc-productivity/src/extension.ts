import * as vscode from 'vscode';
import * as path from 'path'; // Für path.extname und path.basename
import { exec } from 'child_process';
import EditorService from './EditorService';

const sourceTypes: Record<string, string> = {
    "cs": "[source,csharp]",
    "csproj": "[source,xml]",
    "java": "[source,java]",
    "rb": "[source,ruby]",
    "json": "[source,json]",
    "js": "[source,javascript]",
    "ts": "[source,typescript]",
    "jsx": "[source,jsx]",
    "tsx": "[source,tsx]",
    "py": "[source,python]",
    "txt": "[source]",
    "xml": "[source,xml]",
    "adoc": "[source,asciidoc]",
    "md": "[source,markdown]",
    "cmd": "[source]",
    "sh": "[source,bash]",
    "sql": "[source,sql]",
    "yaml": "[source,yaml]",
    "puml": "[source]",
    "docx": "" // Markiert als nicht unterstützt
};

export function activate(context: vscode.ExtensionContext) {

    // --- BEFEHL 1: Source Block einfügen ---
    let insertSourceBlock = vscode.commands.registerCommand('asciidoc-productivity.insertSourceBlock', async () => {
        try {
            const editorService = new EditorService(); // Wirft Error, wenn kein Editor offen

            const language = await vscode.window.showInputBox({
                prompt: 'Sprache für den Source-Block eingeben (z. B. csharp, java, python)',
                value: 'csharp'
            }) ?? '';

            const clipboardText = await vscode.env.clipboard.readText();
            if (!clipboardText) {
                vscode.window.showWarningMessage('Die Zwischenablage ist leer.');
                return;
            }

            const langDef = language.trim() !== '' ? `,${language.trim()}` : '';
            const block = `[source${langDef}]\n----\n${clipboardText}\n----\n`;
            await editorService.insertAtCurrentPosition(block);
        } catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            } else {
                vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
            }
        }
    });

// --- BEFEHL 2: Bild-URL herunterladen, speichern und einfügen ---
    let insertImageBlock = vscode.commands.registerCommand('asciidoc-productivity.insertImageBlock', async () => {
        try {
            const editorService = new EditorService();

            const clipboardText = await vscode.env.clipboard.readText();
            if (!clipboardText) {
                vscode.window.showWarningMessage('Die Zwischenablage ist leer.');
                return;
            }
            
            const url = clipboardText.trim();
            if (!url.startsWith('http://') && !url.startsWith('https://')) {
                vscode.window.showErrorMessage('Die Zwischenablage enthält keine gültige HTTP/HTTPS URL.');
                return;
            }
            if (!editorService.isDocumentSaved()) {
                vscode.window.showErrorMessage('Bitte speichere das AsciiDoc-Dokument zuerst, um Bilder mit relativen Pfaden einzufügen.');
                return;
            }
            const savePath = await editorService.showSaveDialog({
                saveLabel: 'Bild herunterladen & speichern',
                filters: { 'Images': ['png', 'jpg', 'jpeg', 'gif', 'svg', 'webp'] }
            });
            if (!savePath) return; // Benutzer hat den Dialog abgebrochen

            const response = await fetch(url);
            if (!response.ok) {
                vscode.window.showErrorMessage(`Fehler beim Herunterladen des Bildes (Status: ${response.status}).`);
                return;
            }
            const arrayBuffer = await response.arrayBuffer();
            const fileData = new Uint8Array(arrayBuffer);
            await vscode.workspace.fs.writeFile(vscode.Uri.file(savePath), fileData);

            const relativePath = editorService.getRelativeAsciiDocPath(savePath);
            const block = `.Source: ${url}\nimage::${relativePath}[]\n`;
            await editorService.insertAtCurrentPosition(block);
        }
        catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            } else {
                vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
            }
        }
    });

    // --- BEFEHL 3: TSV Tabelle einfügen ---
    let insertTsvTable = vscode.commands.registerCommand('asciidoc-productivity.insertTsvTable', async () => {
        try {
            const editorService = new EditorService();

            const clipboardText = await vscode.env.clipboard.readText();
            if (!clipboardText) {
                vscode.window.showWarningMessage('Die Zwischenablage ist leer.');
                return;
            }

            const firstLine = clipboardText.split(/\r?\n/)[0];
            const colCount = firstLine.split('\t').length;
            const cols = Array(colCount).fill('a').join(',');
            const block = `[%header,cols="${cols}",format=tsv]\n|===\n${clipboardText}\n|===\n`;
            await editorService.insertAtCurrentPosition(block);
        }
        catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            } else {
                vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
            }
        }
    });

    // --- BEFEHL 4: Bild aus Datei einfügen ---
    let insertImageFromFile = vscode.commands.registerCommand('asciidoc-productivity.insertImageFromFile', async () => {
        try {
            const editorService = new EditorService();

            // Wir nutzen die gekapselte Logik aus dem Service
            const imagePath = await editorService.showOpenDialog({
                canSelectMany: false,
                openLabel: 'Bild auswählen',
                filters: { 'Images': ['png', 'jpg', 'jpeg', 'gif', 'svg'] }
            });

            if (imagePath) {
                const relativePath = editorService.getRelativeAsciiDocPath(imagePath);
                await editorService.insertAtCurrentPosition(`image::${relativePath}[]\n`);
            }
        }
        catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            } else {
                vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
            }
        }
    });

    // --- BEFEHL 5: Bild aus Zwischenablage speichern und einfügen ---
    let insertImageFromClipboard = vscode.commands.registerCommand('asciidoc-productivity.insertImageFromClipboard', async () => {
        try {
            const editorService = new EditorService();

            if (!editorService.isDocumentSaved()) {
                vscode.window.showErrorMessage('Bitte speichere das AsciiDoc-Dokument zuerst, um Bilder mit relativen Pfaden einzufügen.');
                return;
            }
            const savePath = await editorService.showSaveDialog({
                saveLabel: 'Bild speichern',
                filters: { 'Images': ['png'] }
            });
            if (!savePath) return;
            const relativePath = editorService.getRelativeAsciiDocPath(savePath);

            const platform = process.platform;
            let script = '';
            if (platform === 'win32') {
                script = `powershell -command "Add-Type -AssemblyName System.Windows.Forms; $clip = [System.Windows.Forms.Clipboard]::GetImage(); if ($clip -ne $null) { $clip.Save('${savePath}', [System.Drawing.Imaging.ImageFormat]::Png) } else { exit 1 }"`;
            } else if (platform === 'darwin') {
                script = `osascript -e 'set theFile to (open for access POSIX file "${savePath}" with write permission)' -e 'try' -e 'write (the clipboard as «class PNGf») to theFile' -e 'end try' -e 'close access theFile'`;
            } else {
                vscode.window.showErrorMessage('Diese Funktion wird auf Linux nicht unterstützt.');
                return;
            }

            exec(script, async (error) => {
                if (error) {
                    vscode.window.showErrorMessage('Fehler: Es konnte kein Bild in der Zwischenablage gefunden werden.');
                    return;
                }
                await editorService.insertAtCurrentPosition(`image::${relativePath}[]\n`);
            });

        }
        catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            } else {
                vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
            }
        }
    });


    // --- BEFEHL 6: Datei aus dem Explorer als Source Block einfügen ---
    // Wichtig: Beim Klick im Explorer übergibt VS Code die "clickedUri" als Argument!
    let insertFileAsSourceBlock = vscode.commands.registerCommand('asciidoc-productivity.insertFileAsSourceBlock', async (clickedUri: vscode.Uri) => {
        if (!clickedUri) {
            vscode.window.showErrorMessage('Dieser Befehl muss aus dem Datei-Explorer aufgerufen werden.');
            return;
        }

        try {
            const editorService = new EditorService(); // Wirft den Error, falls kein Editor offen ist
            const filePath = clickedUri.fsPath;
            const fileName = path.basename(filePath);
            const ext = path.extname(filePath).replace('.', '').toLowerCase();
            const sourceHeader = sourceTypes[ext] !== undefined ? sourceTypes[ext] : "[source]";
            if (sourceHeader === "") {
                vscode.window.showWarningMessage(`Dateien vom Typ .${ext} können nicht als Source-Block eingefügt werden.`);
                return;
            }

            const fileData = await vscode.workspace.fs.readFile(clickedUri);
            const fileContent = Buffer.from(fileData).toString('utf8').replace(/^\uFEFF/, '');
            const relativePath = editorService.getRelativeAsciiDocPath(filePath);
            const block = `.link:${relativePath}[→ ${fileName}]\n${sourceHeader}\n----\n${fileContent}\n----\n`;
            await editorService.insertAtCurrentPosition(block);

        } catch (error: any) {
            if (error instanceof Error) {
                vscode.window.showErrorMessage(error.message);
            }
        }
    });

    context.subscriptions.push(
        insertSourceBlock,
        insertImageBlock,
        insertTsvTable,
        insertImageFromFile,
        insertImageFromClipboard,
        insertFileAsSourceBlock
    );
}

export function deactivate() { }
