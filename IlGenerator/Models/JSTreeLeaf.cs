using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class JSTreeLeaf
    {
        public string text;
        public string data;
        public string type;
        public JSTreeLeaf(string text, string sysInfo, string bodyCode, IlTypes type)
        {
            this.text = text;
            this.type = FormatIlType(type);
            data = bodyCode;
        }

        private static string FormatIlType(IlTypes type)
        {
            switch (type)
            {
                case IlTypes.MethodInstance | IlTypes.MethodGeneric:
                    return "method-instance-generic";
                case IlTypes.MethodStatic | IlTypes.MethodGeneric:
                    return "method-static-generic";
                default:
                    return FormatIlType(type.GetType().GetEnumName(type));
            }
        }
        private static string FormatIlType(string typename)
        {
            //if name contains other uppercase letters (e.g. ClassGeneric)
            if(typename.Substring(1).Any(x => char.IsUpper(x)))
            {
                var i = typename.IndexOf(typename.Substring(1).First(x => char.IsUpper(x)));
                return typename.Substring(0, i).ToLower() + "-" + typename.Substring(i).ToLower();
            }
            return typename.ToLower();
        }
    }
}