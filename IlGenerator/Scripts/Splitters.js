var codeSplitter = $("#content").split({
    orientation: 'horizontal',
    limit: 50,
    position: '50%',
    onDrag: function (event) {
        var parentHeight = $('#content').height();
        var newHeight = $('#editorContainer').height();
        var percent = newHeight / parentHeight * 100;
        if (percent > 30) {
            $('#editorContainer').css('min-height', percent - 10 + '%');
        }
        editor.refresh();
        resultEditor.refresh();
    }
});

var infoSplitter = $("#resultInfoContainer").split({
    orientation: 'vertical',
    limit: 75,
    position: '48.25%',
    onDrag: function (event) {
        var parentWidth = $('#resultInfoContainer').width();
        var newWidth = $('#tree').width();
        var percent = newWidth / parentWidth * 100;
        if (percent > 30) {
            $('#tree').css('min-width', percent - 10 + '%');
        }
        resultEditor.refresh();
    }
});
