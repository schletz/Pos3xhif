import * as vscode from 'vscode';
import ConfigurationService from './ConfigurationService';
import LLMService from './LLMService';

const outputChannel = vscode.window.createOutputChannel("LLM-Simplify");

export async function simplifyText(configurationService: ConfigurationService, llmService: LLMService) {
    try {
        const editor = vscode.window.activeTextEditor;
        if (!editor) { return; }
        const selection = editor.selection;
        if (selection.isEmpty) {
            vscode.window.showWarningMessage('Please first select the text that should be simplified.');
            return;
        }

        const textToProcess = editor.document.getText(selection).trim();
        const MAX_CHARS = 16384;
        if (textToProcess.length > MAX_CHARS) {
            vscode.window.showWarningMessage(`You have selected ${textToProcess.length} characters. Make sure that the response is not cut off.`);
        }

        const systemPrompt = configurationService.getSimplifyTextPrompt();
        const userPrompt = textToProcess;

        await vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            title: `Simplifying text (B2)...`,
            cancellable: false
        }, async () => {
            const result = await llmService.sendPrompt(
                systemPrompt, userPrompt, 0.2, outputChannel);

            await editor.edit(editBuilder => {
                editBuilder.replace(selection, result.content);
            });

            if (result.stats) {
                let message = `Simplification finished in ${result.stats.durationSeconds} sec, ${result.stats.completionTokens} tokens, ${result.stats.tokensPerSecond} tokens/sec.`
                if (result.stats.hasLengethExeeded) {
                    message = message + "\nWARNING: The limit for maxOutputTokens has been exceeded. The text is truncated."
                    vscode.window.showWarningMessage(message, { modal: true });
                }
                else
                    vscode.window.showInformationMessage(message);
            }
        });

    } catch (error: any) {
        vscode.window.showWarningMessage('Error during simplification: ' + error.message);
    }
}
