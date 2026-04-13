import * as vscode from 'vscode';
import path from 'path';
import * as mammoth from 'mammoth';
import ConfigurationService from './ConfigurationService';
// @ts-expect-error: Ignore ESM/CommonJS conflict (TS1479)
import { PdfReader } from "pdfreader";

async function copyDocx(uri: vscode.Uri): Promise<string> {
    try {
        const fileData = await vscode.workspace.fs.readFile(uri);
        const buffer = Buffer.from(fileData);

        const result = await mammoth.extractRawText({ buffer: buffer });
        return result.value.trim();
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : String(error);
        console.error(`Error reading DOCX ${uri}:`, errorMessage);
        return `[Error extracting DOCX file: ${errorMessage}]`;
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
        console.error(`Error reading PDF ${uri.fsPath}:`, errorMessage);
        return `[Error extracting PDF file: ${errorMessage}]`;
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
    if (!targetUri) {
        if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
            targetUri = vscode.workspace.workspaceFolders[0].uri;
        } else {
            vscode.window.showErrorMessage('Please open a folder (workspace) or select a directory.');
            return;
        }
    }

    try {
        const stat = await vscode.workspace.fs.stat(targetUri);
        if (stat.type !== vscode.FileType.Directory) {
            vscode.window.showErrorMessage('Please select a directory.');
            return;
        }

        const includeExtensions = await vscode.window.showInputBox({
            prompt: 'Extensions to consider. Regex expression. Example: cs|java',
            value: configurationService.getIncludeExtensions()
        }) ?? '';
        const extRegex = new RegExp(`^(${includeExtensions})$`, 'i');

        const MAX_FILE_SIZE_BYTES = 10_485_760; // 10 MB
        const excludedDirectories = configurationService.getExcludedDirectories();
        const excludedFiles = configurationService.getExcludedFiles();

        const rootName = path.basename(targetUri.fsPath);
        const parentPath = path.dirname(targetUri.fsPath);

        const now = new Date().toISOString();
        let output = '<?xml version="1.0"?>\n';
        output += `<documents root="${rootName}" created="${now}">\n\n`;

        async function processDirectory(dirUri: vscode.Uri) {
            const entries = await vscode.workspace.fs.readDirectory(dirUri);

            for (const [name, type] of entries) {
                const itemUri = vscode.Uri.joinPath(dirUri, name);
                const relativePath = path.relative(parentPath, itemUri.fsPath).replace(/\\/g, '/');

                if (type === vscode.FileType.Directory) {
                    if (excludedDirectories.includes(name.toLowerCase()) || name.startsWith('.')) {
                        continue;
                    }
                    await processDirectory(itemUri);

                } else if (type === vscode.FileType.File) {
                    if (excludedFiles.includes(name.toLowerCase())) { continue; }
                    const ext = path.extname(name).replace('.', '').toLowerCase();
                    if (!extRegex.test(ext)) { continue; }

                    const fileStat = await vscode.workspace.fs.stat(itemUri);
                    if (fileStat.size > MAX_FILE_SIZE_BYTES) { continue; }

                    let fileContent = "";

                    if (parsers[ext]) {
                        fileContent = await parsers[ext](itemUri);
                    } else {
                        const fileData = await vscode.workspace.fs.readFile(itemUri);
                        // Keep your custom method:
                        fileContent = (Buffer.from(fileData) as any).getStringWithEncodingDetection();
                    }

                    output += `<file path="${relativePath}" language="${ext}">\n<![CDATA[\n${fileContent}\n]]>\n</file>\n\n`;
                }
            }
        }

        await processDirectory(targetUri);
        output += `</documents>\n`;

        await vscode.env.clipboard.writeText(output);
        const userChoice = await vscode.window.showInformationMessage(
            `Source code from '${rootName}' copied as XML! Would you also like to save the content as a file?`,
            'Yes',
            'No'
        );

        if (userChoice === 'Yes') {
            const saveUri = await vscode.window.showSaveDialog({
                defaultUri: vscode.Uri.file(`${rootName}_sources.xml`),
                filters: {
                    'XML Files': ['xml'],
                    'All Files': ['*']
                },
                saveLabel: 'Save XML'
            });

            if (saveUri) {
                // Convert content to Uint8Array/Buffer and write
                const fileData = Buffer.from(output, 'utf8');
                await vscode.workspace.fs.writeFile(saveUri, fileData);
                vscode.window.showInformationMessage('XML file saved successfully!');
            }
        }
    } catch (error: any) {
        if (error instanceof Error) {
            vscode.window.showErrorMessage(`Error during copying: ${error.message}`);
        }
    }
}