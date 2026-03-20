import * as vscode from 'vscode';
import path from 'path';
import * as mammoth from 'mammoth';
import { sourceTypes } from './globals';
import ConfigurationService from './ConfigurationService';
// @ts-expect-error: Ignoriert den ESM/CommonJS Konflikt (TS1479)
import { PdfReader } from "pdfreader";

async function copyDocx(uri: vscode.Uri): Promise<string> {
    try {
        const fileData = await vscode.workspace.fs.readFile(uri);
        const buffer = Buffer.from(fileData);

        // mammoth liest den Text direkt aus dem Buffer
        const result = await mammoth.extractRawText({ buffer: buffer });
        return result.value.trim();
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : String(error);
        console.error(`Fehler beim Lesen der DOCX ${uri}:`, errorMessage);
        return `[Fehler beim Extrahieren der DOCX-Datei: ${errorMessage}]`;
    }
}

async function copyPdf(uri: vscode.Uri): Promise<string> {
    try {
        const fileData = await vscode.workspace.fs.readFile(uri);
        const buffer = Buffer.from(fileData);

        const reader = new PdfReader();
        const content = await new Promise<string>((resolve, reject) => {
            let extractedText = "";

            reader.parseBuffer(buffer, (err: any, item: any) => {
                if (err) { reject(err); }
                else if (!item) { resolve(extractedText); }
                else if (item.text) { extractedText += item.text + " "; }
            });
        });

        return content.trim();
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : String(error);
        console.error(`Fehler beim Lesen der PDF ${uri.fsPath}:`, errorMessage);
        return `[Fehler beim Extrahieren der PDF-Datei: ${errorMessage}]`;
    }
}

const parsers: Record<string, (filename: vscode.Uri) => Promise<string>> = {
    "docx": copyDocx,
    "pdf": copyPdf
};

export async function copySourcesToClipboard(
    clickedUri: vscode.Uri | undefined,
    configurationService: ConfigurationService) {

    let targetUri = clickedUri;
    // Hat der User auf das Icon im Explorer beim Titel geklickt?
    if (!targetUri) {
        if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
            targetUri = vscode.workspace.workspaceFolders[0].uri;
        } else {
            vscode.window.showErrorMessage('Bitte öffne einen Ordner (Workspace) oder wähle ein Verzeichnis aus.');
            return;
        }
    }

    try {
        const stat = await vscode.workspace.fs.stat(targetUri); // targetUri verwenden
        if (stat.type !== vscode.FileType.Directory) {
            vscode.window.showErrorMessage('Bitte wähle ein Verzeichnis aus.');
            return;
        }
        const includeExtensions = await vscode.window.showInputBox({
            prompt: 'Zu berücksichtigende Erweiterungen. Regex Ausdruck. Beispiel: cs|java',
            value: configurationService.getIncludeExtensions()
        }) ?? '';
        const extRegex = new RegExp(`^(${includeExtensions})$`, 'i');

        const MAX_FILE_SIZE_BYTES = 10_485_760; // 10 MB
        const excludedDirectories = configurationService.getExcludedDirectories();
        const excludedFiles = configurationService.getExcludedFiles();

        // Root Name von der targetUri ableiten
        const rootName = path.basename(targetUri.fsPath);
        const parentPath = path.dirname(targetUri.fsPath);
        let output = `= Content of ${rootName}\n`;
        output += `:source-highlighter: rouge\n\n`;
        const now = new Date().toISOString().replace(/\.\d{3}Z$/, 'Z');
        output += `Created: ${now}\n\n`;

        async function processDirectory(dirUri: vscode.Uri, currentDepth: number) {
            const entries = await vscode.workspace.fs.readDirectory(dirUri);
            for (const [name, type] of entries) {
                const itemUri = vscode.Uri.joinPath(dirUri, name);
                const relativePath = path.relative(parentPath, itemUri.fsPath).replace(/\\/g, '/');
                const headingLevel = Math.min(currentDepth + 1, 6);
                const headingPrefix = '='.repeat(headingLevel);

                if (type === vscode.FileType.Directory) {
                    if (excludedDirectories.includes(name.toLowerCase()) || name.startsWith('.')) {
                        continue;
                    }
                    output += `${headingPrefix} ${relativePath}\n\n`;
                    await processDirectory(itemUri, currentDepth + 1);

                } else if (type === vscode.FileType.File) {
                    if (excludedFiles.includes(name.toLowerCase())) { continue; }
                    const ext = path.extname(name).replace('.', '').toLowerCase();
                    if (!extRegex.test(ext)) { continue; }
                    const fileStat = await vscode.workspace.fs.stat(itemUri);
                    if (fileStat.size > MAX_FILE_SIZE_BYTES) { continue; }

                    const sourceHeader = sourceTypes[ext] ?? "";
                    output += `${headingPrefix} ${relativePath}\n\n`;

                    if (parsers[ext]) {
                        const fileData = await parsers[ext](itemUri);
                        output += `${sourceHeader}\n----\n${fileData}\n----\n\n`;
                    }
                    else {
                        const fileData = await vscode.workspace.fs.readFile(itemUri);
                        const fileContent = Buffer.from(fileData).getStringWithEncodingDetection();
                        output += `${sourceHeader}\n----\n${fileContent}\n----\n\n`;
                    }
                }
            }
        }

        await processDirectory(targetUri, 1); // targetUri verwenden
        await vscode.env.clipboard.writeText(output.trim() + '\n');
        vscode.window.showInformationMessage(`Source code von '${rootName}' wurde kopiert!`);

    } catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(`Fehler beim Kopieren: ${error.message}`);
        }
    }
}
