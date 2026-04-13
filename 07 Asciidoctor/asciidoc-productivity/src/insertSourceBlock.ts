import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertSourceBlock() {
    try {
        const editorService = new EditorService(); // Throws error if no editor is open

        const language = await vscode.window.showInputBox({
            prompt: 'Enter language for the source block (e.g., csharp, java, python)',
            value: 'csharp'
        }) ?? '';

        const clipboardText = await vscode.env.clipboard.readText();
        if (!clipboardText) {
            vscode.window.showWarningMessage('The clipboard is empty.');
            return;
        }

        const langDef = language.trim() !== '' ? `,${language.trim()}` : '';
        const block = `[source${langDef}]\n----\n${clipboardText}\n----\n`;
        await editorService.insertAtCurrentPosition(block);
    } catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(error.message);
        } else {
            vscode.window.showErrorMessage('An unknown error occurred.');
        }
    }
}