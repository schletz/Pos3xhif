import * as vscode from 'vscode';
import * as path from 'path';
import { Uri } from 'vscode';

export default class EditorService {
    private editor: vscode.TextEditor;

    constructor() {
        const activeEditor = vscode.window.activeTextEditor;
        if (!activeEditor) {
            throw new Error('Kein Editor geöffnet.');
        }
        this.editor = activeEditor;
    }

    public performEditorOperations<T>(operations: (editor: vscode.TextEditor) => T): T {
        return operations(this.editor);
    }

    public async insertAtCurrentPosition(block: string): Promise<boolean> {
        return this.editor.edit(editBuilder => {
            editBuilder.insert(this.editor.selection.active, block);
        });
    }

    public getDocumentPath(useWorkspaceAsDefault: boolean = true): Uri | undefined {
        if (this.editor.document.uri.scheme === 'file') {
            return vscode.Uri.file(path.dirname(this.editor.document.uri.fsPath));
        }
        return useWorkspaceAsDefault 
            ? vscode.workspace.workspaceFolders?.[0]?.uri 
            : undefined;
    }

    public async showOpenDialog(options?: vscode.OpenDialogOptions): Promise<string | undefined> {
        const defaultOptions: vscode.OpenDialogOptions = { defaultUri: this.getDocumentPath() };
        const fileUris = await vscode.window.showOpenDialog({ ...defaultOptions, ...options });
        return fileUris && fileUris.length > 0 ? fileUris[0].fsPath : undefined;
    }

    public async showSaveDialog(options?: vscode.SaveDialogOptions): Promise<string | undefined> {
        const defaultOptions: vscode.SaveDialogOptions = { defaultUri: this.getDocumentPath() };
        const saveUri = await vscode.window.showSaveDialog({ ...defaultOptions, ...options });
        return saveUri ? saveUri.fsPath : undefined;
    }

    public isDocumentSaved(): boolean {
        return this.editor.document.uri.scheme === 'file';
    }

    public getRelativeAsciiDocPath(targetPath: string): string {
        const docPath = path.dirname(this.editor.document.uri.fsPath);
        return path.relative(docPath, targetPath).replace(/\\/g, '/');
    }
}
