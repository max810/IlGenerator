var codeSplitter = $("#content").height('100%').split({
    orientation: 'horizontal',
    limit: 50,
    position: '50%',
    onDrag: function (event) {
        var parentHeight = $('#content').height();
        if (parentHeight > 150) {
            var newHeight = $('#editorContainer').height();
            var percent = newHeight / parentHeight * 100;
            if (percent < 75 && percent > 25) {
                $('#editorContainer').css('max-height', percent + 5 + '%');
                $('#editorContainer').css('min-height', percent - 5 + '%');
            }
            editor.refresh();
            CheckScrollbar();
        }
        //resultEditor.refresh();
    }
});

var infoSplitter = $("#resultInfoContainer").split({
    orientation: 'vertical',
    limit: 75,
    position: '48.25%',
    onDrag: function (event) {
        var parentWidth = $('#resultInfoContainer').width();
        if (parentWidth > 75) {
            var newWidth = $('#tree').width();
            var percent = newWidth / parentWidth * 100;
            if (percent < 75 && percent > 25) {
                $('#tree').css('max-width', percent + 5 + '%');
                $('#tree').css('min-width', percent - 5 + '%');
            }
        }

        //resultEditor.refresh();
    }
});
