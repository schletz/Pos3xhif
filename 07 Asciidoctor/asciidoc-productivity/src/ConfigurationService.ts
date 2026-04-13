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
            'completionsUrl', "http://127.0.0.1:8000/v1/chat/completions");
    }
    public getLlm(): string {
        const config = vscode.workspace.getConfiguration('asciidoc-productivity');

        return config.get<string>(
            'llm', "LilaRest/gemma-4-31B-it-NVFP4-turbo");
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
}
