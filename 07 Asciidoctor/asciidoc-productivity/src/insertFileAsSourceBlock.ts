import * as vscode from 'vscode';
import EditorService from './EditorService';
import path from 'path';
import { sourceTypes } from './globals';

export async function insertFileAsSourceBlock(clickedUri: vscode.Uri) {
    if (!clickedUri) {
        vscode.window.showErrorMessage('This command must be called from the file explorer.');
        return;
    }

    try {
        const editorService = new EditorService(); // Throws error if no editor is open
        const filePath = clickedUri.fsPath;
        const fileName = path.basename(filePath);
        const ext = path.extname(filePath).replace('.', '').toLowerCase();
        const sourceHeader = sourceTypes[ext] !== undefined ? sourceTypes[ext] : "[source]";
        if (sourceHeader === "") {
            vscode.window.showWarningMessage(`Files of type .${ext} cannot be inserted as a source block.`);
            return;
        }

        const fileData = await vscode.workspace.fs.readFile(clickedUri);
        const fileContent = Buffer.from(fileData).getStringWithEncodingDetection();
        const relativePath = editorService.getRelativeAsciiDocPath(filePath);
        const block = `.link:${relativePath}[→ ${fileName}]\n${sourceHeader}\n----\n${fileContent}\n----\n`;
        await editorService.insertAtCurrentPosition(block);
        await editorService.focusEditor();

    } catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(error.message);
        }
    }
}