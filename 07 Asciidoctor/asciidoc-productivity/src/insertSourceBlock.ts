import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertSourceBlock() {
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
}
