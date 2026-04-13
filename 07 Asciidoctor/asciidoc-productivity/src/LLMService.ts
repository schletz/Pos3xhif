import { performance } from 'perf_hooks';
import ConfigurationService from './ConfigurationService';
import type * as vscode from 'vscode';

export interface PromptStats {
    promptTokens: number;
    completionTokens: number;
    durationSeconds: number;
    tokensPerSecond: number;
}

export interface PromptResult {
    content: string;
    stats: PromptStats | null;
}

export default class LLMService {
    private configService: ConfigurationService;

    constructor(configService: ConfigurationService) {
        this.configService = configService;
    }

    public async sendPrompt(
        systemPrompt: string, userPrompt: string, 
        temperature: number,
        outputChannel?: vscode.OutputChannel): Promise<PromptResult> {
        const url = this.configService.getCompletionsUrl();
        const model = this.configService.getLlm();

        if (outputChannel) {
            outputChannel.appendLine(`\n[LLMService] Sending request to: ${url}`);
            outputChannel.appendLine(`[LLMService] Model: ${model}`);
        }

        const startTime = performance.now();
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer EMPTY'
            },
            body: JSON.stringify({
                model: model,
                messages: [
                    { role: "system", content: systemPrompt },
                    { role: "user", content: userPrompt }
                ],
                max_tokens: this.configService.getMaxOutputTokens(),
                temperature: temperature
            })
        });

        if (!response.ok) {
            throw new Error(`API Error: Status ${response.status} - ${response.statusText}`);
        }

        const data: any = await response.json();
        const content = data.choices[0].message.content.trim();
        const endTime = performance.now();

        // Calculate statistics
        const durationSeconds = (endTime - startTime) / 1000;
        const completionTokens = data.usage?.completion_tokens || 0;
        const promptTokens = data.usage?.prompt_tokens || 0;

        let stats: PromptStats | null = null;
        if (completionTokens > 0 && durationSeconds > 0) {
            stats = {
                promptTokens,
                completionTokens,
                durationSeconds: Number(durationSeconds.toFixed(1)),
                tokensPerSecond: Number((completionTokens / durationSeconds).toFixed(1))
            };

            if (outputChannel) {
                outputChannel.appendLine(`[LLMService] Response received in ${stats.durationSeconds.toFixed(2)}s`);
                outputChannel.appendLine(`[LLMService] Speed: ${stats.tokensPerSecond} TPS`);
            }
        }

        return { content, stats };
    }
}