import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertImageFromFile() {
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
}
