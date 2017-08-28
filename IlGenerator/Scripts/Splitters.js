var codeSplitter = $("#content").split({
    orientation: 'horizontal',
    limit: 100,
    onDrag: function (event) {
        editor.resize();
        resultEditor.resize();
    }
})

var infoSplitter = $("#treeContainer").split({
    orientation: 'vertical',
    limit: 100,
    onDrag: function (event) {
        resultEditor.resize();
    }
})
