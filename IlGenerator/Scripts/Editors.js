var editor = CodeMirror.fromTextArea(document.getElementById('editor'),
    {
        lineNumbers: true,
        mode: 'text/x-csharp',
        gutters: ["CodeMirror-lint-markers"],
        lint: {
            lintOnChange: false,
        },
        theme: 'ambiance',
        scrollbarStyle: "overlay"
    });

editor.on('change', function (cm) {
    cm.save();
    $("#inputForm").submit();
});

CodeMirror.registerHelper('lint', 'text/x-csharp', function (editor, text) { return ERRORS; });

var resultEditor = CodeMirror.fromTextArea(document.getElementById('resultEditor'),
    {
        lineNumbers: true,
        mode: 'text/x-il',
        readOnly: 'nocursor',
        scrollbarStyle: "overlay",
        theme: 'neat'
    });