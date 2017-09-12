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

function changeFont(mod) {
    font += mod;
    if (font <= 24) {
        treeFont = font;
    } else {
        treeFont = 24;
    }
    $('.CodeMirror').css('font-size', font + 'px');
    changeTreeFont(treeFont);
    $('#fontSizeInput').val(font);
    editor.refresh();
    resultEditor.refresh();
}


function changeTreeFont(newFontSize) {
    $('.jstree-node').css('font-size', newFontSize + 'px');
}

function selectAll(someEditor) {
    var end = someEditor.lastLine();
    var endLength = someEditor.getLine(end).length;
    someEditor.setSelection(CodeMirror.Pos(0, 0), CodeMirror.Pos(end, endLength));
    someEditor.focus();
}

function updateResultInfo(dataObject) {
    var warnings = dataObject.Warnings;
    var errors = dataObject.Errors;

    ERRORS = warnings.map(x => toError(x, 'warning')).concat(errors.map(x => toError(x, 'error')));

    editor.performLint();

    if (errors.length != 0) {
        $('#tree').html(errors.map(x => x.TextMessage).join('<br>'));
    }
    else if (!dataObject.Tree.children.length) {
        $('#tree').html('Nothing to disassemble. Insert your code in the upper editor.');
    }
    else {
        setCookie('sourceCode', encodeURIComponent(editor.getValue()), 7);
        createTree(dataObject.Tree);
    }
    resultEditor.setValue('');
}

function getCookie(name) {
    var results = document.cookie.split('; ').map(x => x.split('=')).filter(x => x[0] == name);
    if (results.length) {
        return decodeURIComponent(results[0][1]);
    }
    return '';
}

function setCookie(name, value, expireDays) {
    var d = new Date();
    d.setTime(d.getTime() + (expireDays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + "; " + expires + "; path=/";
}

function deleteCookie(name) {
    document.cookie = name + '=; ' + 'expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
}

function toError(errorInfo, severity) {
    var line = errorInfo.Line - 1;
    var ch = errorInfo.Column;
    var res = searchErrorBoundaries(CodeMirror.Pos(line, ch));
    return ({
        message: errorInfo.HighlightMessage,
        severity: severity,
        from: CodeMirror.Pos(line, res.start),
        to: CodeMirror.Pos(line, res.end),
        col: errorInfo.Column
    });


    function searchErrorBoundaries(position) {
        var charPos = position.ch;
        var str = editor.getLine(position.line);
        if (str) {
            var startIndex = charPos - 1;
            var endIndex = startIndex + 1;
            if (startIndex == 1) {
                startIndex = 0;
            }
            var stringRemaining = str.substring(startIndex + 1);
            if (stringRemaining == '' && startIndex != 0) {
                startIndex--;
            }
            var searchResult = stringRemaining.search(/[^\w]/);
            if (searchResult > -1) {
                endIndex += searchResult;
            }
            return {
                start: startIndex,
                end: endIndex
            };
        }
        else {
            return {
                start: 0,
                end: 0
            }
        }
    }


}