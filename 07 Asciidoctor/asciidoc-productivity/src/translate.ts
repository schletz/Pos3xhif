import * as vscode from 'vscode';
import * as path from 'path';
import ConfigurationService from './ConfigurationService';
import LLMService from './LLMService';

const outputChannel = vscode.window.createOutputChannel("LLM");

export async function translate(
    configurationService: ConfigurationService,
    llmService: LLMService,
    clickedUri?: vscode.Uri) {

    try {
        let textToTranslate = '';
        let targetUri: vscode.Uri | undefined = clickedUri;
        let activeEditor = vscode.window.activeTextEditor;

        // CASE 1: Call via file explorer (Entire file)
        if (targetUri) {
            const fileData = await vscode.workspace.fs.readFile(targetUri);
            textToTranslate = (Buffer.from(fileData) as any).getStringWithEncodingDetection();
        }
        // CASE 2: Call via editor (Selection)
        else if (activeEditor) {
            const selection = activeEditor.selection;
            if (selection.isEmpty) {
                vscode.window.showWarningMessage('Please select the text first.');
                return;
            }
            textToTranslate = activeEditor.document.getText(selection).trim();
        } else {
            return;
        }

        if (!textToTranslate) {
            vscode.window.showWarningMessage('No content found to translate.');
            return;
        }

        const MAX_CHARS = 16384;
        if (textToTranslate.length > MAX_CHARS) {
            vscode.window.showWarningMessage(`You have selected ${textToTranslate.length} characters. Make sure that the response is not cut off.`);
        }

        const destinationLanguage = await vscode.window.showInputBox({
            prompt: 'Target language. Example: en-US',
            value: configurationService.getDefaultTargetLanguage()
        });
        if (!destinationLanguage) { return; }

        const systemPrompt = `You are a professional translator for asciidoc documents. Translate the given text into ${destinationLanguage}.
        Output ONLY the translation. Do not add explanations or quotes. Keep all formatting and syntax intact.`;

        await vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            title: targetUri ? `Translating file ${path.basename(targetUri.fsPath)}...` : `Translating selection...`,
            cancellable: false
        }, async () => {
            const result = await llmService.sendPrompt(
                systemPrompt, textToTranslate, 0.1, outputChannel);

            if (targetUri) {
                const doc = await vscode.workspace.openTextDocument(targetUri);
                const editor = await vscode.window.showTextDocument(doc);
                const fullRange = new vscode.Range(
                    doc.positionAt(0),
                    doc.positionAt(doc.getText().length)
                );

                await editor.edit(editBuilder => {
                    editBuilder.replace(fullRange, result.content);
                });
            } else if (activeEditor) {
                await activeEditor.edit(editBuilder => {
                    editBuilder.replace(activeEditor!.selection, result.content);
                });
            }

            if (result.stats) {
                vscode.window.showInformationMessage(
                    `Translation finished in ${result.stats.durationSeconds} sec, ${result.stats.completionTokens} tokens, ${result.stats.tokensPerSecond} tokens/sec.`
                );
            }
        });

    } catch (error: any) {
        vscode.window.showErrorMessage(error.message);
    }
}