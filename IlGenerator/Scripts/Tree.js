function updateTree(dataObject) {
            var warnings = dataObject.Warnings;
            var errors = dataObject.Errors;
            editor.getSession().setAnnotations(warnings.map(x => toAnnotation(x, "warning")));
            if (errors.length != 0) {
                $("#tree").html(errors.map(x => x.TextMessage).join("<br>"));
                editor.getSession().setAnnotations(errors.map(x => toAnnotation(x, "error")));
                resultEditor.setValue("");
            }
            else {
                createTree(dataObject);
            }
        }

        function toAnnotation(errorInfo, type) {
            return ({
                row: errorInfo.Row - 1,
                column: errorInfo.Column,
                text: errorInfo.HighlightMessage,
                type: type
            });
        }

        function createTree(dataObject) {
            $("#tree").jstree('destroy');
            var tree = $("#tree").jstree(
                {
                    'types': {
                        "field-instance": {
                            "icon": "/Content/IlIcons/field-instance.png"
                        },
                        "field-static": {
                            "icon": "/Content/IlIcons/field-static.png"
                        },
                        "property": {
                            "icon": "/Content/IlIcons/property.png"
                        },
                        "method-instance": {
                            "icon": "/Content/IlIcons/method-instance.png"
                        },
                        "method-instance-generic": {
                            "icon": "/Content/IlIcons/method-instance-generic.png"
                        },
                        "method-static": {
                            "icon": "/Content/IlIcons/method-static.png"
                        },
                        "method-static-generic": {
                            "icon": "/Content/IlIcons/method-static-generic.png"
                        },
                        "interface": {
                            "icon": "/Content/IlIcons/interface.png"
                        },
                        "interface-generic": {
                            "icon": "/Content/IlIcons/interface-generic.png"
                        },
                        "event": {
                            "icon": "/Content/IlIcons/event.png"
                        },
                        "class": {
                            "icon": "/Content/IlIcons/class.png"
                        },
                        "class-generic": {
                            "icon": "/Content/IlIcons/class-generic.png"
                        },
                        "struct": {
                            "icon": "/Content/IlIcons/struct.png"
                        },
                        "struct-generic": {
                            "icon": "/Content/IlIcons/struct-generic.png"
                        },
                        "enum": {
                            "icon": "/Content/IlIcons/enum.png"
                        },
                        "folder-field": {
                            "icon": "/Content/IlIcons/folder-field.png"
                        },
                        "folder-property": {
                            "icon": "/Content/IlIcons/folder-property.png"
                        },
                        "folder-event": {
                            "icon": "/Content/IlIcons/folder-event.png"
                        },
                        "folder-method": {
                            "icon": "/Content/IlIcons/folder-method.png"
                        }
                    },
                    'core': {
                        'multiple': false,
                        'data': jsonEncode(dataObject.Tree)
                    },
                    'plugins': ["types"]

                });
            tree.on('changed.jstree', function (e, data) {
                var curr = data.node;
                resultEditor.setValue(decode(curr.data));
                resultEditor.session.selection.clearSelection();
            });
            resultEditor.setValue("");
        }