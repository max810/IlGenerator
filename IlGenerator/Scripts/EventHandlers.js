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

    $('#editor').resize();
});
var clipboard = new Clipboard('#editorCopyAllButton');

clipboard.on('error', function (e) {
    setTemporaryTooltip(e.trigger, 'Not supported in your browser(', 2000);
});

function editorSelectAll() {
    editor.selection.selectAll();
}
function editorUndo() {
    editor.undo();
}
function editorRedo() {
    editor.redo();
}