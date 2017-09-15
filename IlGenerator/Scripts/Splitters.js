var codeSplitter = $("#content").split({
    orientation: 'horizontal',
    limit: 50,
    position: '50%',
    onDrag: function () {
        var newHeight = codeSplitter.position();
        var percent = newHeight / contentHeight * 100;
        if (percent >= 25 && percent <= 75) {
            hSplitterRatioPercent = percent;
        }
        editor.setSize();
        resultEditor.setSize();
    }
});

var infoSplitter = $("#resultInfoContainer").split({ 
    orientation: 'vertical',
    limit: 75,
    position: '48.25%',
    onDrag: function () {
        var newWidth = infoSplitter.position();
        var percent = newWidth / contentWidth * 100;
        if (percent >= 25 && percent <= 75) {
            vSplitterRatioPercent = percent;
        }
        resultEditor.setSize();
    }
});
