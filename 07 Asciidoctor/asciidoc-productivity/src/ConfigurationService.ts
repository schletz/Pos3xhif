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
}
