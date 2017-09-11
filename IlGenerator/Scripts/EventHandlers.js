var curr = null;
var down = false;
$(document).ready(function () {
    $('.tp-r').tooltip({
        trigger: 'hover',
        placement: 'right'
    });

    $('.tp-r').hover(function (event) {
        $(this).css('opacity', '0.9');
    }, function (event) {
        if (!down || !$(this).is(curr))
            $(this).css('opacity', '0.6');
        $(this).tooltip();
    });

    $('.tp-r').mousedown(function () {
        if ($(this).hasClass('btn-dark')) {
            $(this).css('background-color', '#F0F0F0');
        }
        else {
            $(this).css('background-color', '#424242');
        }
        curr = $(this);
        down = true;
    });

    $('.tp-r').mouseup(function () {
        if (curr != null) {
            down = false;
            if (!$(this).is(curr)) {
                curr.mouseout();
                curr.mouseup();
            } else {
                $(this).css('background-color', 'transparent');
                $(this).mouseout();
                curr = null;
            }
        }
    });
    $(document).mouseup(function () {
        if (curr != null) {
            curr.mouseout();
            curr.mouseup();
        }
    });

    editor.refresh();
    var prevCode = getCookie('sourceCode');
    if (prevCode != '') {
        editor.setValue(decodeURIComponent(prevCode));
    }
    $("#inputForm").submit();
    changeFont(0);
});
var clipboard = new Clipboard('.tp-r');

clipboard.on('error', function (e) {
    setTemporaryTooltip(e.trigger, 'Not supported in your browser(', 2000);
});

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
    var res = searchErrorBoundaries(CodeMirror.Pos(line,ch));
    return ({
        message: errorInfo.HighlightMessage,
        severity: severity,
        from: CodeMirror.Pos(line, res.start),
        to: CodeMirror.Pos(line, res.end),
        col: errorInfo.Column
    });
}

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
