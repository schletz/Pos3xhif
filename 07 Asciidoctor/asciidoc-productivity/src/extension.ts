import { commands, ExtensionContext, Uri } from 'vscode';
import './bufferExtensions';
import { insertSourceBlock } from './insertSourceBlock';
import { insertImageBlock } from './insertImageBlock';
import { insertTsvTable } from './insertTsvTable';
import { insertImageFromFile } from './insertImageFromFile';
import { insertImageFromClipboard } from './insertImageFromClipboard';
import { insertFileAsSourceBlock } from './insertFileAsSourceBlock';
import { copySourcesToClipboard } from './copySourcesToClipboard';
import { exportAsPdf } from './exportAsPdf';
import ConfigurationService from './ConfigurationService';
import { copyAsTsv } from './copyAsTsv';
import { translate } from './translate';
import LLMService from './LLMService';
import { checkSpelling } from './spellcheck';
import { simplifyText } from './simplifyText';
import { showPreview } from './showPreview';

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
    // Wichtig: Beim Klick im Explorer übergibt VS Code die "clickedUri" als 1. und alle "selectedUris" als 2. Argument!    
    let copySourcesToClipboardCmd = commands.registerCommand(
        'asciidoc-productivity.copySourcesToClipboard',
        async (clickedUri?: Uri, selectedUris?: Uri[]) => await copySourcesToClipboard(clickedUri, selectedUris, new ConfigurationService()));

    // --- BEFEHL 8: Tabelle als TSV kopieren ---
    let copyAsTsvCmd = commands.registerCommand(
        'asciidoc-productivity.copyAsTsv', copyAsTsv);

    // --- BEFEHL 9: Übersetzung ---
    let translateCmd = commands.registerCommand(
        'asciidoc-productivity.translate',
        async () => {
            const configurationService = new ConfigurationService();
            const llmService = new LLMService(configurationService);
            await translate(configurationService, llmService, undefined);
        }
    );

    // --- BEFEHL 10: Übersetzung (ganze Datei) ---
    let translateEntireFileCmd = commands.registerCommand(
        'asciidoc-productivity.translateEntireFile',
        async (clickedUri?: Uri) => { // clickedUri wird vom Explorer übergeben
            const configurationService = new ConfigurationService();
            const llmService = new LLMService(configurationService);
            await translate(configurationService, llmService, clickedUri);
        }
    );

    // --- BEFEHL 10: Rechtschreibprüfung ---
    let checkSpellingCmd = commands.registerCommand(
        'asciidoc-productivity.checkSpelling',
        async () => {
            const configurationService = new ConfigurationService();
            const llmService = new LLMService(configurationService);
            await checkSpelling(configurationService, llmService);
        }
    );
    // --- BEFEHL 11: Export als PDF ---
    let exportAsPdfCmd = commands.registerCommand(
        'asciidoc-productivity.exportAsPdf',
        async (clickedUri: Uri) => await exportAsPdf(clickedUri));

    let showPreviewCmd = commands.registerCommand(
        'asciidoc-productivity.showPreview',
        () => showPreview(context)
    );
    // --- BEFEHL 12: Einfachere Sprache (B2) ---
    let simplifyTextCmd = commands.registerCommand(
        'asciidoc-productivity.simplifyText',
        async () => {
            const configurationService = new ConfigurationService();
            const llmService = new LLMService(configurationService);
            await simplifyText(configurationService, llmService);
        }
    );    

    context.subscriptions.push(
        insertSourceBlockCmd,
        insertImageBlockCmd,
        insertTsvTableCmd,
        insertImageFromFileCmd,
        insertImageFromClipboardCmd,
        insertFileAsSourceBlockCmd,
        copySourcesToClipboardCmd,
        copyAsTsvCmd,
        translateCmd,
        translateEntireFileCmd,
        checkSpellingCmd,
        simplifyTextCmd,
        exportAsPdfCmd,
        showPreviewCmd
    );
}

export function deactivate() { }
