import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertImageBlock() {
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
        if (!savePath) { return; } // Benutzer hat den Dialog abgebrochen

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
}
