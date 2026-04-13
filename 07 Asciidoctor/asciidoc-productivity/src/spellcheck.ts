import * as vscode from 'vscode';
import ConfigurationService from './ConfigurationService';
import LLMService from './LLMService';

const outputChannel = vscode.window.createOutputChannel("LLM-Spellcheck");

export async function checkSpelling(configurationService: ConfigurationService, llmService: LLMService) {
    try {
        const editor = vscode.window.activeTextEditor;
        if (!editor) { return; }
        const selection = editor.selection;
        if (selection.isEmpty) {
            vscode.window.showWarningMessage('Please first select the text that should be checked.');
            return;
        }

        const textToProcess = editor.document.getText(selection).trim();
        const MAX_CHARS = 16384;
        if (textToProcess.length > MAX_CHARS) {
            vscode.window.showWarningMessage(`You have selected ${textToProcess.length} characters. Make sure that the response is not cut off.`);
        }
        const systemPrompt = `You are an expert editor for asciidoc documents. Correct spelling, grammar, and punctuation of the provided text. 
        Maintain the original tone and all formatting (especially AsciiDoc syntax and source blocks). 
        Output ONLY the corrected text. Do not add explanations or comments.`;

        const userPrompt = textToProcess;

        await vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            title: `Checking spelling...`,
            cancellable: false
        }, async () => {
            // We use the same sendPrompt method
            const result = await llmService.sendPrompt(
                systemPrompt, userPrompt, 0.1, outputChannel);

            await editor.edit(editBuilder => {
                editBuilder.replace(selection, result.content);
            });

            if (result.stats) {
                vscode.window.showInformationMessage(
                    `Correction finished in ${result.stats.durationSeconds} sec, ${result.stats.completionTokens} tokens, ${result.stats.tokensPerSecond} tokens/sec.`
                );
            }
        });

    } catch (error: any) {
        vscode.window.showWarningMessage('Error during correction: ' + error.message);
    }
}
