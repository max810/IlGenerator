using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public static class JsTreeFormatter
    {
        public static object ToJSTree(IEnumerable<TypeInfo> types)
        {
            var tree = new
            {
                text = "Assembly",
                children = types.Select(type =>
                    new JSTreeNode(type.Name, type.SystemInfo, type.CustomAttributes, SourceCodeFormatter.ResolveType(type))
                    {
                        children = new []
                        {
                            new
                            {
                                type = "folder-field",
                                text = "Fields",
                                li_attr = new Dictionary<string, string>()
                                {
                                    {"class", "smaller-margins" }
                                },
                                children = type.Fields.Select( x =>
                                new JSTreeNode(
                                    x.Name,
                                    x.SystemInfo,
                                    AttributesOrEmptyString(x.CustomAttributes),
                                    SourceCodeFormatter.ResolveType(x)))
                            },
                            new
                            {
                                type = "folder-property",
                                text = "Properties",
                                li_attr = new Dictionary<string, string>()
                                {
                                    {"class", "smaller-margins" }
                                },children = type.Properties.Select( x =>
                                new JSTreeNode(
                                    x.Name,
                                    x.SystemInfo,
                                    AttributesOrEmptyString(x.CustomAttributes)
                                        + x.GetterInfo
                                        + Environment.NewLine + x.SetterInfo,
                                    SourceCodeFormatter.ResolveType(x)))
                            },
                            new
                            {
                                type = "folder-event",
                                text = "Events",
                                li_attr = new Dictionary<string, string>()
                                {
                                    {"class", "smaller-margins" }
                                },children = type.Events.Select( x =>
                                new JSTreeNode(
                                    x.Name,
                                    x.SystemInfo,
                                    AttributesOrEmptyString(x.CustomAttributes)
                                        + x.AddOnInfo
                                        + Environment.NewLine + x.RemoveOnInfo,
                                    SourceCodeFormatter.ResolveType(x)))
                            },
                            new
                            {
                                type = "folder-method",
                                text = "Methods",
                                li_attr = new Dictionary<string, string>()
                                {
                                    {"class", "smaller-margins" }
                                },children = type.Methods.Select( x =>
                                new JSTreeNode(
                                    x.Name,
                                    x.SystemInfo,
                                    AttributesOrEmptyString(x.CustomAttributes)
                                        + x.MethodBody,
                                    SourceCodeFormatter.ResolveType(x)))
                            },
                        }
                    })
            };

            return tree;
        }

        private static string AttributesOrEmptyString(string attrs)
        {
            if (string.IsNullOrWhiteSpace(attrs))
                return "";
            return attrs + Environment.NewLine;
        }
    }
}