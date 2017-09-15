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

    $('#content').height('100%');
    $('#tree').css('width', '50%');

    $('.topNavbarButton').mouseup(function () {
        $(this).blur();
    });

    $('#fontSizeInput').change(function () {
        var newVal = $('#fontSizeInput').val();
        if (newVal >= 4 && newVal <= 36)
            changeFont($('#fontSizeInput').val() - font);
        else {
            $('#fontSizeInput').val(font);
        }
    });
    $('#fontSizeInput').keydown(function (e) {
        if (e.key == 'Enter') {
            $(this).blur();
        }
    });

    editor.refresh();
    var prevCode = getCookie('sourceCode');
    if (prevCode != '') {
        editor.setValue(decodeURIComponent(prevCode));
    }
    else {
        $("#inputForm").submit();
    }

    $(window).resize(function () {
        codeSplitter.position(hSplitterRatioPercent + '%');
        infoSplitter.position(vSplitterRatioPercent + '%');
        contentHeight = $('#content').height();
        contentWidth = $('#content').width();
    });

    //set 12px as a default
    changeFont(0);
});


var clipboard = new Clipboard('.tp-r');
clipboard.on('error', function (e) {
    setTemporaryTooltip(e.trigger, 'Not supported in your browser(', 2000);
});