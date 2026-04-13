import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertTsvTable() {
    try {
        const editorService = new EditorService();

        const clipboardText = await vscode.env.clipboard.readText();
        if (!clipboardText || clipboardText.trim() === '') {
            vscode.window.showWarningMessage('Clipboard is empty.');
            return;
        }

        const firstLine = clipboardText.split(/\r?\n/)[0];
        const colCount = firstLine.split('\t').length;
        const cols = Array(colCount).fill('a').join(',');
        const tableContent = clipboardText
            .replace(/(\r?\n)+$/g, '')    // Replace empty lines at the end
            .replace(/\r?\n/g, '\n\n|')
            .replace(/\t/g, '\n|');
        const block = `[%header,cols="${cols}"]\n|===\n|${tableContent}\n|===\n`;
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
