//encode html entities
function encode(text) {
    var encoded = text.replace(/[\u00A0-\u9999<>\&]/gim, function (i) {
        return '&#' + i.charCodeAt(0) + ';';
    });
    return encoded;
}

//decode html entities
function decode(text) {
    //inserting text into a div, forcing jquery to decode it, 
    //and then pulling the new text back
    var decoded = $('<div/>').html(text).text();
    return decoded;
}

//encode html entities in the whole object
function jsonEncode(jsonObject) {
    var str = JSON.stringify(jsonObject);
    var newstr = encode(str);
    return JSON.parse(newstr);
}

var editor = CodeMirror.fromTextArea(document.getElementById('editor'),
    {
        lineNumbers: true,
        mode: 'text/x-csharp',
        //mode: "javascript",
        gutters: ["CodeMirror-lint-markers"],
        lint: {
            lintOnChange: false,
        },
        theme: 'ambiance'
    });

editor.on('change', function (cm) {
    cm.save();
    $("#inputForm").submit();
});

CodeMirror.registerHelper('lint', 'text/x-csharp', function (editor, text) { return ERRORS; });
var resultEditor = CodeMirror.fromTextArea(document.getElementById('resultEditor'),
    {
        lineNumbers: true,
        mode: 'text/x-csharp',
        readOnly: 'nocursor'
    });