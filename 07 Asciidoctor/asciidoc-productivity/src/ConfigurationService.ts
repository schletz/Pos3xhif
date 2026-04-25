import * as vscode from 'vscode';

export default class ConfigurationService {
    constructor() {
    }
    public getIncludeExtensions(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');
        const includeExtensions = config.get<string>(
            'includeExtensions',
            "cs|csproj|java|rb|json|js|ts|jsx|tsx|py|txt|xml|adoc|md|cmd|sh|sql|yaml|puml");
        return includeExtensions.replace(/\s+/g, '');
    }
    public getExcludedDirectories(): string[] {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string[]>(
            'excludeDirectories', ['bin', 'obj', 'node_modules'])
            .map(dir => dir.toLowerCase());
    }
    public getExcludedFiles(): string[] {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string[]>(
            'excludeFiles', ["package-lock.json"])
            .map(file => file.toLowerCase());
    }
    public getCompletionsUrl(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'completionsUrl', "http://localhost:1234/v1/chat/completions");
    }
    public getLlm(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'llm', "lmstudio-community/Qwen3.6-35B-A3B-GGUF");
    }
    public getMaxOutputTokens(): number {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<number>(
            'maxOutputTokens', 4096);
    }
    public getDefaultTargetLanguage(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'defaultLanguage', "en-US");
    }
    public getCheckSpellingPrompt(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'checkSpellingPrompt',
            "You are an expert editor for asciidoc documents. Correct spelling, grammar, and punctuation of the provided text.\nMaintain the original tone and all formatting (especially AsciiDoc syntax and source blocks).\nOutput ONLY the corrected text. Do not add explanations or comments."
        );
    }
    public getSimplifyTextPrompt(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'simplifyTextPrompt',
            "You are an expert editor for asciidoc documents. Rewrite the provided German text to be clear and easily understandable for non-native students with a B2 language level.\nKeep all technical terms intact, but resolve overly nested sentences (Schachtelsätze) and avoid unnecessary passive voice.\nMaintain the original tone and all formatting (especially AsciiDoc syntax and source blocks).\nOutput ONLY the rewritten text. Do not add explanations or comments."
        );
    }
    public getApiKey(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');
        return config.get<string>('apiKey', "");
    }
}
