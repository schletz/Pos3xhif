import * as vscode from 'vscode';

export async function copyAsTsv() {
    try {
        const editor = vscode.window.activeTextEditor;
        if (!editor) {
            vscode.window.showErrorMessage('Kein Editor geöffnet.');
            return;
        }

        const selection = editor.selection;
        if (selection.isEmpty) {
            vscode.window.showWarningMessage('Bitte markiere zuerst eine AsciiDoc-Tabelle.');
            return;
        }
        const text =  editor.document.getText(selection);
        const rows = text
            .replace(/^\|===/gm, '')
            .trim()
            .split(/(?:\r?\n){2,}/);

        const tsvRows = rows.map(row => {
            const cells = row
                .replace(/^\|/g, '')
                .split('|')
                .map(cell => cell.trim()); 
            return cells.join('\t').trim();
        });
        const tsvOutput = tsvRows.join('\n');
        if (!tsvOutput) {
            vscode.window.showWarningMessage('Konnte keine Tabellendaten extrahieren.');
            return;
        }
        await vscode.env.clipboard.writeText(tsvOutput);
        vscode.window.showInformationMessage('Tabelle als TSV in die Zwischenablage kopiert!');

    } catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(error.message);
        } else {
            vscode.window.showErrorMessage('Ein unbekannter Fehler ist aufgetreten.');
        }
    }
}
