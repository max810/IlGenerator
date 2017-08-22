function encode(string) {
    var encoded = string.replace(/[\u00A0-\u9999<>\&]/gim, function (i) {
        return '&#' + i.charCodeAt(0) + ';';
    });
    return encoded;
}

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

//function testError() {
//    editor.getSession().setAnnotations([
//        {
//            row: 1,
//            column: 0,
//            text: "Strange error",
//            type: "error"
//        }]);
//}

editor.getSession().on("change", function () {
    sourceCode.val(encode(editor.getSession().getValue()));
    $("#inputForm").submit();
});

$("div.hsplitter").on('mousedown', function (event) {
    alert("dick");
    editor.resize();
});