try {
    const asciidoctor = window.Asciidoctor();
    window.AsciidoctorKroki.register(asciidoctor.Extensions);
    const contentDiv = document.getElementById('content');
    contentDiv.innerHTML = "<i>Preview ready. (Please type to render text)</i>";
    function renderAsciiDoc(text) {
        const html = asciidoctor.convert(text, {
            safe: 'safe',
            attributes: {
                showtitle: true,
                icons: 'font',
                'kroki-default-format': 'svg',
                'source-highlighter': 'highlight.js',
                stem: 'latexmath'
            }
        });
        if (printRawHtml) {
            contentDiv.innerHTML = "";
            const preElement = document.createElement("pre");
            preElement.innerText = html;
            contentDiv.appendChild(preElement);
            return;
        }
        contentDiv.innerHTML = html;

        if (typeof hljs !== 'undefined') {
            hljs.highlightAll();
        }

        if (typeof MathJax !== 'undefined' && MathJax.typesetPromise) {
            MathJax.typesetClear([contentDiv]);
            MathJax.typesetPromise([contentDiv]).catch(function (err) {
                console.error('MathJax rendering failed: ' + err.message);
            });
        }
    }

    window.addEventListener('message', event => {
        try {
            const message = event.data;
            if (message.command === 'update') {
                renderAsciiDoc(message.text);
            }
        }
        catch (e) {
            document.getElementById('content').innerHTML = '<div style="color:red;">' + e.message + '</div>';
        }
    });
}
catch (e) {
    document.getElementById('content').innerHTML = '<div style="color:red;">' + e.message + '</div>';
}
