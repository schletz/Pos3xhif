import { commands, ExtensionContext, Uri } from 'vscode';
import './bufferExtensions';
import { insertSourceBlock } from './insertSourceBlock';
import { insertImageBlock } from './insertImageBlock';
import { insertTsvTable } from './insertTsvTable';
import { insertImageFromFile } from './insertImageFromFile';
import { insertImageFromClipboard } from './insertImageFromClipboard';
import { insertFileAsSourceBlock } from './insertFileAsSourceBlock';
import { copySourcesToClipboard } from './copySourcesToClipboard';
import ConfigurationService from './ConfigurationService';
import { copyAsTsv } from './copyAsTsv';

export function activate(context: ExtensionContext) {
    // --- BEFEHL 1: Source Block einfügen ---
    let insertSourceBlockCmd = commands.registerCommand(
        'asciidoc-productivity.insertSourceBlock', insertSourceBlock);

    // --- BEFEHL 2: Bild-URL herunterladen, speichern und einfügen ---
    let insertImageBlockCmd = commands.registerCommand(
        'asciidoc-productivity.insertImageBlock', insertImageBlock);

    // --- BEFEHL 3: TSV Tabelle einfügen ---
    let insertTsvTableCmd = commands.registerCommand(
        'asciidoc-productivity.insertTsvTable', insertTsvTable);

    // --- BEFEHL 4: Bild aus Datei einfügen ---
    let insertImageFromFileCmd = commands.registerCommand(
        'asciidoc-productivity.insertImageFromFile', insertImageFromFile);

    // --- BEFEHL 5: Bild aus Zwischenablage speichern und einfügen ---
    let insertImageFromClipboardCmd = commands.registerCommand(
        'asciidoc-productivity.insertImageFromClipboard', insertImageFromClipboard);

    // --- BEFEHL 6: Datei aus dem Explorer als Source Block einfügen ---
    // Wichtig: Beim Klick im Explorer übergibt VS Code die "clickedUri" als Argument!
    let insertFileAsSourceBlockCmd = commands.registerCommand(
        'asciidoc-productivity.insertFileAsSourceBlock',
        async (clickedUri: Uri) => await insertFileAsSourceBlock(clickedUri));

    // --- BEFEHL 7: Sourcecode aus einem Verzeichnis in die Zwischenablage kopieren
    // Wichtig: Beim Klick im Explorer übergibt VS Code die "clickedUri" als Argument!    
    let copySourcesToClipboardCmd = commands.registerCommand(
        'asciidoc-productivity.copySourcesToClipboard',
        async (clickedUri?: Uri) => await copySourcesToClipboard(clickedUri, new ConfigurationService()));

    // --- BEFEHL 8: Tabelle als TSV kopieren ---
    let copyAsTsvCmd = commands.registerCommand(
        'asciidoc-productivity.copyAsTsv', copyAsTsv);

    context.subscriptions.push(
        insertSourceBlockCmd,
        insertImageBlockCmd,
        insertTsvTableCmd,
        insertImageFromFileCmd,
        insertImageFromClipboardCmd,
        insertFileAsSourceBlockCmd,
        copySourcesToClipboardCmd,
        copyAsTsvCmd
    );
}

export function deactivate() { }
