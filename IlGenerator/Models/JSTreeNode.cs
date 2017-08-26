using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class JSTreeNode
    {
        public string text;
        public string data;
        public string type;
        public object[] children = new object[0];
        public Dictionary<string, string> li_attr = new Dictionary<string, string>()
        {
            {"class", "smaller-margins" }
        };
        public JSTreeNode(string text, string sysInfo, string bodyCode, IlTypes type)
        {
            this.text = text;
            this.type = FormatType(type);
            data = $"{sysInfo}{Environment.NewLine}" +
                $"{{{Environment.NewLine}" +
                $"{WithOffset(bodyCode)}" +
                $"{Environment.NewLine}}}";
        }

        private string FormatType(IlTypes type)
        {
            switch (type)
            {
                case IlTypes.MethodInstance | IlTypes.MethodGeneric:
                    return "method-instance-generic";
                case IlTypes.MethodStatic | IlTypes.MethodGeneric:
                    return "method-static-generic";
                default:
                    return FormatType(type.GetType().GetEnumName(type));
            }
        }
        //making ClassGeneric => class-generic
        private string FormatType(string typename)
        {
            //if name contains other uppercase letters (e.g. ClassGeneric)
            if(typename.Substring(1).Any(x => char.IsUpper(x)))
            {
                var i = typename.IndexOf(typename.Substring(1).First(x => char.IsUpper(x)));
                return typename.Substring(0, i).ToLower() + "-" + typename.Substring(i).ToLower();
            }
            return typename.ToLower();
        }

        private string WithOffset(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var offsetLines = lines.Select(x => "\t" + x);
            return string.Join(Environment.NewLine, offsetLines);
        } 
    }
}