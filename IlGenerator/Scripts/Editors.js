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

var editor = ace.edit("editor");
editor.setTheme("ace/theme/monokai");
editor.getSession().setMode("ace/mode/csharp");
var sourceCode = $('textarea[name="sourceCode"]');
editor.$blockScrolling = Infinity;

editor.session.setOption("useWorker", false);

editor.getSession().on("change", function () {
    sourceCode.val(encode(editor.getSession().getValue()));
    $("#inputForm").submit();
});


var resultEditor = ace.edit("resultEditor");
editor.setTheme("ace/theme/monokai");
resultEditor.getSession().setMode("ace/mode/csharp-il");
resultEditor.session.setOption("useWorker", false);
resultEditor.$blockScrolling = Infinity;
resultEditor.setReadOnly(true);