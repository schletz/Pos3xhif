import * as vscode from 'vscode';
import EditorService from './EditorService';

export async function insertImageFromFile() {
    try {
        const editorService = new EditorService();

        // We use the encapsulated logic from the service
        const imagePath = await editorService.showOpenDialog({
            canSelectMany: false,
            openLabel: 'Select image',
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
            vscode.window.showErrorMessage('An unknown error occurred.');
        }
    }
}