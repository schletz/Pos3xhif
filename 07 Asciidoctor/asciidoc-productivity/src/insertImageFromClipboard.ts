import * as vscode from 'vscode';
import { exec } from 'child_process';

import EditorService from './EditorService';

export async function insertImageFromClipboard() {
    try {
        const editorService = new EditorService();

        if (!editorService.isDocumentSaved()) {
            vscode.window.showErrorMessage('Please save the AsciiDoc document first to insert images with relative paths.');
            return;
        }
        const savePath = await editorService.showSaveDialog({
            saveLabel: 'Save image',
            filters: { 'Images': ['png'] }
        });
        if (!savePath) { return; }
        const relativePath = editorService.getRelativeAsciiDocPath(savePath);

        const platform = process.platform;
        let script = '';
        if (platform === 'win32') {
            script = `powershell -command "Add-Type -AssemblyName System.Windows.Forms; $clip = [System.Windows.Forms.Clipboard]::GetImage(); if ($clip -ne $null) { $clip.Save('${savePath}', [System.Drawing.Imaging.ImageFormat]::Png) } else { exit 1 }"`;
        } else if (platform === 'darwin') {
            script = `osascript -e 'set theFile to (open for access POSIX file "${savePath}" with write permission)' -e 'try' -e 'write (the clipboard as «class PNGf») to theFile' -e 'end try' -e 'close access theFile'`;
        } else {
            vscode.window.showErrorMessage('This function is not supported on Linux.');
            return;
        }

        exec(script, async (error) => {
            if (error) {
                vscode.window.showErrorMessage('Error: No image found in the clipboard.');
                return;
            }
            await editorService.insertAtCurrentPosition(`image::${relativePath}[]\n`);
        });

    }
    catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(error.message);
        } else {
            vscode.window.showErrorMessage('An unknown error occurred.');
        }
    }
}