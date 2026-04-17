// =================================================================================================
// showPreview.ts
// Renders a preview for AsciiDoc documents in VS Code.
// This file uses a customized version of asciidoctor.js:
//     git clone https://github.com/asciidoctor/asciidoctor.js.git
//     cd asciidoctor.js
//     npm install
//     cd packages/core
//     npm install
//     
//     In packages/core/src/template-asciidoctor-browser.js change 
//     export default function Asciidoctor(moduleConfig)
//     to
//     window.Asciidoctor = function Asciidoctor(moduleConfig)
//
// Download asciidoctor-kroki for PlantUML Rendering from
// https://github.com/asciidoctor/asciidoctor-kroki/blob/master/dist/browser/asciidoctor-kroki.js
//
// Download highlight.js from https://highlightjs.org/download and copy min.js and theme to lib.
// Download MathJax from https://cdn.jsdelivr.net/npm/mathjax@4/tex-mml-chtml.js to lib.
// =================================================================================================
const printRawHtml = false;  // for debugging

import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

let currentPanel: vscode.WebviewPanel | undefined = undefined;
let changeDocumentSubscription: vscode.Disposable | undefined = undefined;

// --- HELPER FUNCTION: Resolves includes in the Node.js backend ---
function resolveLocalIncludes(text: string, documentUri: vscode.Uri): string {
    if (documentUri.scheme !== 'file') {
        return text;
    }

    const baseDir = path.dirname(documentUri.fsPath);
    const includeRegex = /^include::([^\[]+)\[(.*?)\]/gm;

    return text.replace(includeRegex, (match, filePath) => {
        try {
            const fullPath = path.join(baseDir, filePath);
            if (fs.existsSync(fullPath)) {
                return fs.readFileSync(fullPath, 'utf8');
            } else {
                return `\n// ERROR: File not found: ${filePath}\n`;
            }
        } catch (e: any) {
            return `\n// ERROR: Could not load ${filePath} (${e.message})\n`;
        }
    });
}

export function showPreview(context: vscode.ExtensionContext) {
    const editor = vscode.window.activeTextEditor;

    if (!editor || editor.document.languageId !== 'asciidoc') {
        vscode.window.showInformationMessage('Please change the language of the current file to AsciiDoc to use the preview.');
        return;
    }

    const column = vscode.ViewColumn.Beside;
    const documentDir = vscode.Uri.file(path.dirname(editor.document.uri.fsPath));
    const localResourceRoots = [vscode.Uri.joinPath(context.extensionUri, 'lib')];
    if (vscode.workspace.workspaceFolders) {
        localResourceRoots.push(...vscode.workspace.workspaceFolders.map(f => f.uri));
    } else {
        localResourceRoots.push(documentDir);
    }

    currentPanel = vscode.window.createWebviewPanel(
        'asciidocPreview',
        'AsciiDoc Preview',
        column,
        {
            enableScripts: true,
            retainContextWhenHidden: true,
            localResourceRoots: localResourceRoots
        }
    );

    const libUri = currentPanel.webview.asWebviewUri(vscode.Uri.joinPath(context.extensionUri, 'lib'));
    const baseUri = currentPanel.webview.asWebviewUri(documentDir);
    
    currentPanel.webview.html = getWebviewContent(libUri, baseUri, printRawHtml);
    
    setTimeout(() => {
        if (currentPanel) {
            const resolvedText = resolveLocalIncludes(editor.document.getText(), editor.document.uri);
            currentPanel.webview.postMessage({ command: 'update', text: resolvedText });
        }
    }, 500);

    changeDocumentSubscription = vscode.workspace.onDidChangeTextDocument(event => {
        if (currentPanel && event.document === vscode.window.activeTextEditor?.document) {
            const resolvedText = resolveLocalIncludes(event.document.getText(), event.document.uri);
            currentPanel.webview.postMessage({ command: 'update', text: resolvedText });
        }
    });

    currentPanel.onDidDispose(() => {
        currentPanel = undefined;
        if (changeDocumentSubscription) {
            changeDocumentSubscription.dispose();
            changeDocumentSubscription = undefined;
        }
    }, null);
}

function getWebviewContent(libUri: vscode.Uri, baseUri: vscode.Uri, printRawHtml: boolean) {
    // baseUri erhält am Ende einen Slash, damit der Browser ihn als Basis-Ordner erkennt!
    const baseHref = baseUri.toString().endsWith('/') ? baseUri.toString() : baseUri.toString() + '/';

    return `
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <base href="${baseHref}">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <meta http-equiv="Content-Security-Policy" content="default-src 'none'; font-src ${libUri.scheme}: vscode-resource: vscode-webview-resource: data:; img-src ${libUri.scheme}: vscode-resource: vscode-webview-resource: https: http: data:; style-src 'unsafe-inline' ${libUri.scheme}: vscode-resource: vscode-webview-resource:; script-src 'unsafe-inline' ${libUri.scheme}: vscode-resource: vscode-webview-resource:; worker-src blob:;">
            <title>AsciiDoc Preview</title>
            <link rel="stylesheet" id="hljs-theme" href="">
            <link rel="stylesheet" href="${libUri}/preview.css">
            <script>
                const lightThemeUri = "${libUri}/vs.min.css";
                const darkThemeUri = "${libUri}/vs2015.min.css";

                function updateHljsTheme() {
                    const isDark = document.body.classList.contains('vscode-dark') || 
                                document.body.classList.contains('vscode-high-contrast');
                    document.getElementById('hljs-theme').href = isDark ? darkThemeUri : lightThemeUri;
                }
                window.addEventListener('DOMContentLoaded', () => {
                    updateHljsTheme();
                    const observer = new MutationObserver(updateHljsTheme);
                    observer.observe(document.body, { attributes: true, attributeFilter: ['class'] });
                });
            </script>            
            <script>
                const printRawHtml = ${printRawHtml};

                window.MathJax = {
                    tex: {
                        inlineMath: [['\\\\(', '\\\\)'], ['\\\\$', '\\\\$']],
                        displayMath: [['\\\\[', '\\\\]']],
                        processEscapes: true
                    },
                    startup: {
                        typeset: false
                    },
                    options: {
                        enableMenu: false,
                        enableSpeech: false,
                        enableBraille: false,
                        enableEnrichment: false,
                        menuOptions: {
                            settings: {
                                enrich: false,         // true to enable semantic-enrichment
                                collapsible: false,   // true to enable collapsible math
                                speech: false,         // true to enable speech generation
                                braille: false,        // true to enable Braille generation
                                assistiveMml: false,  // true to enable assistive MathML
                            }
                        }                        
                    }                    
                };                
            </script>
            <script src="${libUri}/asciidoctor.min.js"></script>
            <script src="${libUri}/asciidoctor-kroki.js"></script>
            <script src="${libUri}/highlight.min.js"></script>
            <script src="${libUri}/tex-mml-chtml.js"></script>
            <script src="${libUri}/preview.js" defer></script>
        </head>
        <body>
            <div id="content">Loading Asciidoctor preview...</div>
        </body>
        </html>
    `;
}
