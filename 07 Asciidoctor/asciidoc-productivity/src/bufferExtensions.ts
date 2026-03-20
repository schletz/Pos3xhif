export {}; // Zwingt TypeScript, diese Datei als Modul zu behandeln

declare global {
    interface Buffer {
        getStringWithEncodingDetection(): string;
    }
}

Buffer.prototype.getStringWithEncodingDetection = function(): string {
    const buffer = this;
    let fileContent = '';

    // 1. Ist die Datei UTF-16 LE? (BOM: FF FE)
    if (buffer.length >= 2 && buffer[0] === 0xFF && buffer[1] === 0xFE) {
        fileContent = buffer.toString('utf16le');
    } 
    // 2. Ist die Datei UTF-16 BE? (BOM: FE FF)
    else if (buffer.length >= 2 && buffer[0] === 0xFE && buffer[1] === 0xFF) {
        if (buffer.length % 2 === 0) {
            fileContent = buffer.swap16().toString('utf16le');
        } else {
            fileContent = buffer.toString('utf8');
        }
    } 
    // 3. Fallback: UTF-8 (mit oder ohne BOM)
    else {
        fileContent = buffer.toString('utf8');
    }

    // 4. Eventuellen BOM am Anfang des fertigen Strings entfernen
    return fileContent.replace(/^\uFEFF/, '');
};
