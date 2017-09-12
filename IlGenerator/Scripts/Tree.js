function createTree(tree) {
    $('#tree').jstree('destroy');
    var tree = $('#tree').jstree(
        {
            'types': {
                'field-instance': {
                    'icon': '/Content/IlIcons/field-instance.png'
                },
                'field-static': {
                    'icon': '/Content/IlIcons/field-static.png'
                },
                'property': {
                    'icon': '/Content/IlIcons/property.png'
                },
                'method-instance': {
                    'icon': '/Content/IlIcons/method-instance.png'
                },
                'method-instance-generic': {
                    'icon': '/Content/IlIcons/method-instance-generic.png'
                },
                'method-static': {
                    'icon': '/Content/IlIcons/method-static.png'
                },
                'method-static-generic': {
                    'icon': '/Content/IlIcons/method-static-generic.png'
                },
                'interface': {
                    'icon': '/Content/IlIcons/interface.png'
                },
                'interface-generic': {
                    'icon': '/Content/IlIcons/interface-generic.png'
                },
                'event': {
                    'icon': '/Content/IlIcons/event.png'
                },
                'class': {
                    'icon': '/Content/IlIcons/class.png'
                },
                'class-generic': {
                    'icon': '/Content/IlIcons/class-generic.png'
                },
                'struct': {
                    'icon': '/Content/IlIcons/struct.png'
                },
                'struct-generic': {
                    'icon': '/Content/IlIcons/struct-generic.png'
                },
                'enum': {
                    'icon': '/Content/IlIcons/enum.png'
                },
                'folder-field': {
                    'icon': '/Content/IlIcons/folder-field.png'
                },
                'folder-property': {
                    'icon': '/Content/IlIcons/folder-property.png'
                },
                'folder-event': {
                    'icon': '/Content/IlIcons/folder-event.png'
                },
                'folder-method': {
                    'icon': '/Content/IlIcons/folder-method.png'
                }
            },
            'core': {
                'multiple': false,
                'data': jsonEncode(tree)
            },
            'plugins': ['types']

        });
    tree.on('changed.jstree', function (e, data) {
        var curr = data.node;
        var decodedString = decode(curr.data);
        resultEditor.setValue(decodedString);
    });
    tree.on('open_node.jstree', function () { changeTreeFont(treeFont) });
    $('#IlCode').val('');
    resultEditor.setValue('');
}