import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertImageBlock() {
    try {
        const editorService = new EditorService();

        const clipboardText = await vscode.env.clipboard.readText();
        if (!clipboardText) {
            vscode.window.showWarningMessage('Clipboard is empty.');
            return;
        }

        const url = clipboardText.trim();
        if (!url.startsWith('http://') && !url.startsWith('https://')) {
            vscode.window.showErrorMessage('Clipboard does not contain a valid HTTP/HTTPS URL.');
            return;
        }
        if (!editorService.isDocumentSaved()) {
            vscode.window.showErrorMessage('Please save the AsciiDoc document first to insert images with relative paths.');
            return;
        }
        const savePath = await editorService.showSaveDialog({
            saveLabel: 'Download & save image',
            filters: { 'Images': ['png', 'jpg', 'jpeg', 'gif', 'svg', 'webp'] }
        });
        if (!savePath) { return; } // User cancelled the dialog

        const response = await fetch(url);
        if (!response.ok) {
            vscode.window.showErrorMessage(`Error downloading image (Status: ${response.status}).`);
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
            vscode.window.showErrorMessage('An unknown error occurred.');
        }
    }
}