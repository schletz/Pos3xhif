import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';
import Docker from 'dockerode';
import * as tar from 'tar-stream';
import * as stream from 'stream';

const outputChannel = vscode.window.createOutputChannel("AsciiDoc PDF Export");
const docker = new Docker(); // Connects automatically to the local Docker daemon

/**
 * Handles the prompt and preparation of the theme parameter.
 * Returns the arguments for Docker and optionally the path to the temporary file (for cleanup).
 */
async function getThemeParameter(dirPath: string, baseName: string): Promise<{ themeArgs: string[], tempThemePath?: string }> {
    let tempThemePath: string | undefined = undefined;
    let themeArgs: string[] = [];
    let useTheme = false;

    const defaultThemePath = path.join(dirPath, `${baseName}.yml`);

    // 1. Theme Check: Same name (.yml)
    if (fs.existsSync(defaultThemePath)) {
        const answer = await vscode.window.showInformationMessage(
            `Should I use ${baseName}.yml as theme file?`, 'Yes', 'No'
        );
        if (answer === 'Yes') {
            themeArgs = ['--theme', `${baseName}.yml`];
            useTheme = true;
        }
    }

    // 2. Theme Check: Load template?
    if (!useTheme) {
        const answer = await vscode.window.showInformationMessage(
            `Do you want to load a theme template?`, 'Yes', 'No'
        );
        if (answer === 'Yes') {
            const selectedFiles = await vscode.window.showOpenDialog({
                defaultUri: vscode.Uri.file(dirPath),
                openLabel: 'Select Theme',
                filters: { 'Theme Files': ['yml', 'yaml'] }
            });

            if (selectedFiles && selectedFiles.length > 0) {
                const selectedThemePath = selectedFiles[0].fsPath;

                // Check if the file is already inside the workspace (dirPath)
                if (selectedThemePath.toLowerCase().startsWith(dirPath.toLowerCase())) {
                    const relThemePath = path.relative(dirPath, selectedThemePath).replace(/\\/g, '/');
                    themeArgs = ['--theme', relThemePath];
                } else {
                    const tempFileName = `.temp-theme-${Date.now()}.yml`;
                    tempThemePath = path.join(dirPath, tempFileName);

                    fs.copyFileSync(selectedThemePath, tempThemePath);
                    themeArgs = ['--theme', tempFileName];
                    outputChannel.appendLine(`[INFO] External theme copied to temporary file: ${tempFileName}`);
                }
            }
        }
    }

    return { themeArgs, tempThemePath };
}

/**
 * Checks if the image exists, otherwise builds it in-memory.
 */
async function ensureDockerImage(): Promise<void> {
    try {
        await docker.getImage('asciidoctor-pandoc:latest').inspect();
        outputChannel.appendLine('[INFO] Docker image "asciidoctor-pandoc" already exists.');
        return; // Image exists, we can abort early
    } catch (e) {
        outputChannel.appendLine('[INFO] Docker image not found. Building image (in-memory)...');
    }

    const dockerfileContent = `
FROM asciidoctor/docker-asciidoctor
RUN apk add --no-cache pandoc
WORKDIR /documents
CMD ["sh"]
    `.trim();

    const pack = tar.pack();
    pack.entry({ name: 'Dockerfile' }, dockerfileContent);
    pack.finalize();

    const stream = await docker.buildImage(pack, { t: 'asciidoctor-pandoc' });

    await new Promise((resolve, reject) => {
        docker.modem.followProgress(
            stream,
            (err: any, res: any) => err ? reject(err) : resolve(res),
            (event: any) => {
                if (event.stream) { outputChannel.append(event.stream); }
                if (event.error) { outputChannel.appendLine(`[ERROR] ${event.error}`); }
            }
        );
    });
    outputChannel.appendLine('[INFO] Image successfully created.');
}

/**
 * Starts the container and streams the output.
 */
async function runAsciidoctorContainer(
    dirPath: string,
    fileName: string,
    targetPdf: string,
    themeParam: string[]
): Promise<void> {

    // Safely build the command array (prevent shell injection)
    const cmd = [
        'asciidoctor-pdf',
        ...themeParam,
        '-r', 'asciidoctor-mathematical',
        '-r', 'asciidoctor-diagram',
        '-a', 'allow-uri-read',
        '-a', 'stem',
        '-a', 'mathematical-format=svg',
        '-o', targetPdf,
        fileName
    ];

    outputChannel.appendLine(`[EXEC] asciidoctor-pdf parameters: ${cmd.join(' ')}`);

    // Configure container
    const container = await docker.createContainer({
        Image: 'asciidoctor-pandoc',
        Cmd: cmd,
        HostConfig: {
            AutoRemove: true, // Container will be removed after completion (--rm)
            Binds: [`${dirPath}:/documents`] // Mounts the workspace (-v)
        },
        WorkingDir: '/documents'
    });

    // Intercept output streams (stdout and stderr)
    const logStream = new stream.PassThrough();
    logStream.on('data', (chunk) => {
        outputChannel.append(chunk.toString('utf8'));
    });

    const streamAttach = await container.attach({ stream: true, stdout: true, stderr: true });
    container.modem.demuxStream(streamAttach, logStream, logStream);

    // Start container
    await container.start();

    // Wait until the process inside the container has finished
    const data = await container.wait();
    if (data.StatusCode !== 0) {
        throw new Error(`The conversion process failed (Exit Code: ${data.StatusCode}).`);
    }
}


export async function exportAsPdf(clickedUri: vscode.Uri) {
    if (!clickedUri) {
        vscode.window.showErrorMessage('Please call this command via the File Explorer.');
        return;
    }

    const inputPath = clickedUri.fsPath;
    const dirPath = path.dirname(inputPath);
    const fileName = path.basename(inputPath);
    const baseName = path.parse(fileName).name;
    const targetPdf = `${baseName}.pdf`;

    outputChannel.show();
    outputChannel.appendLine(`Starting PDF export for ${fileName}...`);

    // Fetch the theme parameters and optional temp file path
    const { themeArgs, tempThemePath } = await getThemeParameter(dirPath, baseName);

    // 3. Execution with progress notification
    await vscode.window.withProgress({
        location: vscode.ProgressLocation.Notification,
        title: `Converting ${fileName} to PDF...`,
        cancellable: false
    }, async () => {
        try {
            await docker.ping();
            await ensureDockerImage();

            // themeArgs are passed here
            await runAsciidoctorContainer(dirPath, fileName, targetPdf, themeArgs);

            const cacheFolder = path.join(dirPath, '.asciidoctor');
            if (fs.existsSync(cacheFolder)) {
                fs.rmSync(cacheFolder, { recursive: true, force: true });
            }

            vscode.window.showInformationMessage(`Export successful: ${targetPdf}`);
            outputChannel.appendLine('[INFO] Done!');

        } catch (error: any) {
            vscode.window.showErrorMessage(error.message || 'Error during PDF export. Check the output channel for details.');
            outputChannel.appendLine(`[ABORT] ${error.message}`);
        } finally {
            // tempThemePath is used here for cleanup
            if (tempThemePath && fs.existsSync(tempThemePath)) {
                try {
                    fs.unlinkSync(tempThemePath);
                    outputChannel.appendLine('[INFO] Temporary theme file cleaned up.');
                } catch (cleanupError) {
                    outputChannel.appendLine(`[WARNING] Could not delete temporary file: ${cleanupError}`);
                }
            }
        }
    });
}
