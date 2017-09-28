using Microsoft.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
namespace IlGenerator.Models
{
    public static class SourceCodeFormatter
    {
        #region CODE info

        //------------Types and members CODE info providers------------

        public static TypeInfo GetTypeInfo(TypeDefinition td)
        {
            var name = td.Name;
            if (td.HasGenericParameters)
            {
                name += $"<{string.Join(", ", td.GenericParameters)}>";
            }
            TypeInfo currType = new TypeInfo(name, GetSystemInfo(td), GetCustomAttributes(td));
            foreach (FieldDefinition fd in td.Fields)
            {
                currType.Fields.Add(GetFieldInfo(fd));
            }
            foreach (PropertyDefinition pd in td.Properties)
            {
                currType.Properties.Add(GetPropertyInfo(pd));
            }
            foreach (EventDefinition ed in td.Events)
            {
                currType.Events.Add(GetEventInfo(ed));
            }
            foreach (MethodDefinition md in td.Methods)
            {
                currType.Methods.Add(GetMethodInfo(md));
            }
            return currType;
        }

        public static FieldInfo GetFieldInfo(FieldDefinition fd)
        {
            string name = $"{GetTypeAlias(fd.FieldType)} {fd.Name}";
            string sysinfo = GetSystemInfo(fd);

            return new FieldInfo(name, sysinfo, GetCustomAttributes(fd));
        }

        public static PropertyInfo GetPropertyInfo(PropertyDefinition pd)
        {
            string name = $"{GetTypeAlias(pd.PropertyType)} {pd.Name}";
            string sysinfo = GetSystemInfo(pd);
            string getter = GetSystemInfo(pd.GetMethod);
            string setter = GetSystemInfo(pd.SetMethod);

            return new PropertyInfo(name, sysinfo, GetCustomAttributes(pd), getter, setter);
        }

        public static EventInfo GetEventInfo(EventDefinition ed)
        {
            string name = $"{GetTypeAlias(ed.EventType)} {ed.Name}";
            string sysinfo = GetSystemInfo(ed);
            string addOn = GetSystemInfo(ed.AddMethod);
            string removeOn = GetSystemInfo(ed.RemoveMethod);

            return new EventInfo(name, sysinfo, GetCustomAttributes(ed), addOn, removeOn);
        }

        public static MethodInfo GetMethodInfo(MethodDefinition md)
        {
            string name = $"{md.Name}";
            if (md.HasGenericParameters)
            {
                name += $"<{string.Join(", ", md.GenericParameters)}>";
            }
            name += $" : {GetTypeAlias(md.ReturnType)}" +
                $"({string.Join(", ", md.Parameters.Select(x => GetTypeAlias(x.ParameterType)))})";
            
            string sysinfo = GetSystemInfo(md);
            var instructions = md.Body.Instructions;
            StringBuilder body = new StringBuilder(256);

            body.Append(string.Join(Environment.NewLine, instructions));

            string withHandlers = InsertExceptionHandlers(body.ToString(), md.Body.ExceptionHandlers);

            //----Additional info----

            string fullBodyCode = $".maxstack {md.Body.MaxStackSize}{Environment.NewLine}{withHandlers}";
            if (md.Body.HasVariables)
            {
                fullBodyCode = $".locals init ({string.Join(", ", md.Body.Variables.Select(x => $"[{x.Index}] {GetTypeAlias(x.VariableType)}{(string.IsNullOrWhiteSpace(x.Name) ? "" : " " + x.Name)}"))}){Environment.NewLine}" + fullBodyCode;
            }
            fullBodyCode = $"// Code size (0x{string.Format("{0:X}", md.Body.CodeSize)}){Environment.NewLine}" + fullBodyCode;
            return new MethodInfo(name, sysinfo, GetCustomAttributes(md), fullBodyCode);
        }

        #endregion

        #region SYSTEM info

        //------------Types and members SYSTEM info providers (attrs, params, etc.)------------

        private static string GetSystemInfo(TypeDefinition td)
        {
            var sysinfo = (td.IsClass ? ".class" : "")
                + (td.IsPublic ? " public" : " private")
                + (td.IsAutoClass ? " auto" : "")
                + (td.IsAnsiClass ? " ansi" : "")
                + (td.IsBeforeFieldInit ? " beforefieldinit" : "")
                + ($" ofname {td.Name}");

            if (!string.Equals(td.BaseType.Name, "Object", StringComparison.InvariantCultureIgnoreCase))
            {
                sysinfo += Environment.NewLine + "\textends " + td.BaseType.FullName;
            }

            if (td.IsValueType)
            {
                sysinfo += " valuetype";
            }
            return sysinfo;
        }

        private static string GetSystemInfo(FieldDefinition fd)
        {
            string sysinfo = ".field " + GetAccessibility(fd)
                + (fd.IsStatic ? " static" : " instance")
                + (fd.IsInitOnly ? " initonly" : "")
                + " " + fd.FieldType
                + $" ofname {fd.Name}";
            return sysinfo;
        }

        private static string GetSystemInfo(PropertyDefinition pd)
        {

            string sysinfo = $".property {pd.PropertyType}"
                + (pd.IsSpecialName ? " specialname" : "")
                + (pd.IsRuntimeSpecialName ? " rtspecialname" : "")
                + $" ofname {pd.Name}";

            return sysinfo;
        }

        private static string GetSystemInfo(EventDefinition ed)
        {
            string sysinfo = $".event {ed.EventType}"
                + (ed.IsSpecialName ? " specialname" : "")
                + (ed.IsRuntimeSpecialName ? " rtspecialname" : "")
                + $" ofname {ed.Name}";

            return sysinfo;
        }

        private static string GetSystemInfo(MethodDefinition md)
        {
            string sysInfo = ".method " + GetAccessibility(md)
                 + (md.IsAbstract ? " abstract" : "")
                 + (md.IsAddOn ? " addon" : "")
                 + (md.IsRemoveOn ? " removeon" : "")
                 + (md.IsGetter ? " getter" : "")
                 + (md.IsSetter ? " setter" : "")
                 + (md.IsStatic ? " static" : " instance")//!!!
                 + (md.IsHideBySig ? " hidebysig" : "")
                 + (md.IsPreserveSig ? " preservesig" : "")
                 + (md.IsFinal ? " final" : "")
                 + (md.IsNewSlot ? " newslot" : "")
                 + (md.IsVirtual ? " virtual" : "")
                 + (md.IsSpecialName ? " specialname" : "")
                 + (md.IsRuntimeSpecialName ? " rtspecialname" : "")
                 + (md.IsManaged ? " cil managed" : " umnanaged")
                 + ($" ofname {md.Name}")
                 + $"{Environment.NewLine}\treturn: ({GetTypeAlias(md.ReturnType)})"
                 + $"{Environment.NewLine}\tparams: ({GetParameters(md, true)})";

            return sysInfo;
        }

        #endregion

        #region Exception Handlers

        private static string InsertExceptionHandlers(string bodyCode, IEnumerable<ExceptionHandler> handlers)
        {
            List<string> lines = bodyCode.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            foreach(var group in handlers.GroupBy(x => x.TryStart.ToString()))
            {
                var finallyHandler = group.Where(x => x.HandlerType == ExceptionHandlerType.Finally)
                    .FirstOrDefault();
                if(finallyHandler != null)
                {
                    InsertTry(lines, finallyHandler);
                    InsertHandler(lines, finallyHandler);

                    var catchers = group.Where(x => x.HandlerType == ExceptionHandlerType.Catch);
                    if(catchers.Any())
                    {
                        InsertTry(lines, catchers.First());
                        foreach(var handler in catchers)
                        {
                            InsertHandler(lines, handler);
                        }
                    }
                }
                else
                {
                    InsertTry(lines, group.First());
                    foreach(var handler in group)
                    {
                        InsertHandler(lines, handler);
                    }
                }
            }
            return string.Join(Environment.NewLine, lines);
        }

        private static void InsertHandler(List<string> lines, ExceptionHandler handler)
        {
            int handlerStart = GetInstructionIndex(lines, handler.HandlerStart.ToString());
            int handlerEnd = GetInstructionIndex(lines, handler.HandlerEnd.ToString());
            int numberOfLines = handlerEnd - handlerStart;
            var innerCode = lines.GetRange(handlerStart, numberOfLines);
            lines.RemoveRange(handlerStart, numberOfLines);

            string name = handler.HandlerType.ToString().ToLowerInvariant();
            if(handler.HandlerType == ExceptionHandlerType.Catch)
            {
                name += $" ({handler.CatchType})";
            }

            lines.InsertRange(handlerStart, WrapWithHandler(innerCode, name));
        }

        private static void InsertTry(List<string> lines, ExceptionHandler handler)
        {
            int tryStart = GetInstructionIndex(lines, handler.TryStart.ToString());
            int tryEnd = GetInstructionIndex(lines, handler.TryEnd.ToString());
            int numberOfLines = tryEnd - tryStart;
            var innnerCode = lines.GetRange(tryStart, numberOfLines);
            lines.RemoveRange(tryStart, numberOfLines);

            lines.InsertRange(tryStart, WrapWithHandler(innnerCode, ".try"));
        }

        private static List<string> WrapWithHandler(List<string> innerCode, string handlerName)
        {
            int offsetNumber = innerCode.First().Length - innerCode.First().TrimStart().Length;
            innerCode = innerCode.Select(x => "\t" + x).ToList();
            string offset = new string('\t', offsetNumber);
            innerCode.Insert(0, $"{offset}{handlerName}");
            innerCode.Insert(1, offset + "{");
            innerCode.Add($"{offset}}} // end {handlerName.Split(' ').First()}");
            return innerCode;
        }

        private static int GetInstructionIndex(List<string> instructions, string instruction)
        {
            return instructions.FindIndex(x => Regex.IsMatch(x, @"^\s*" + instruction + @"\s*$"));
        }

        #endregion

        #region system methods (attrs, accessibility, etc.)

        //------------System Methods------------

        private static string GetCustomAttributes(ICustomAttributeProvider attributeProvider)
        {
            if (attributeProvider.HasCustomAttributes)
                return string.Join(Environment.NewLine, attributeProvider.CustomAttributes.Select(GetCustomAttributeInfo));
            return "";
        }

        private static string GetCustomAttributeInfo(CustomAttribute attr)
        {
            var ctor = attr.Constructor;
            string info = ".custom instance"
                + " " + GetTypeAlias(ctor.ReturnType)
                + " " + GetTypeAlias(attr.AttributeType)
                + "::" + ctor.Name
                + $"({string.Join(", ", ctor.Parameters.Select(x => GetTypeAlias(x.ParameterType)))})"
                + $" = ( {string.Join(" ", attr.GetBlob().Select(x => string.Format("0x{0:X2}", x)))} )";
            return info;
        }

        private static string GetAccessibility(MethodDefinition md)
        {
            return
                md.IsPrivate ? "private" :
                md.IsFamilyOrAssembly ? "famorassem" :
                md.IsFamily ? "family" :
                md.IsAssembly ? "assembly" :
                "public";
        }

        private static string GetAccessibility(FieldDefinition fd)
        {
            return
                fd.IsPrivate ? "private" :
                fd.IsFamilyOrAssembly ? "famorassem" :
                fd.IsFamily ? "family" :
                fd.IsAssembly ? "assembly" :
                "public";
        }

        private static string GetParameters(MethodDefinition md, bool useAlias = false)
        {
            return string.Join(", ", md.Parameters.Select(x =>
            (useAlias
            ? $"{GetTypeAlias(x.ParameterType)}"
            : x.ParameterType.ToString())
            + $" {x.Name}"));
        }

        private static string GetTypeAlias(TypeReference type)
        {
            return GetTypeAlias(type.ToString());
        }

        private static string GetTypeAlias(string type)
        {
            if (Regex.IsMatch(type, @"^System.U?Int\d{2}(\[\])?$")
                || Regex.IsMatch(type, @"^System.Object(\[\])?$")
                || Regex.IsMatch(type, @"^System.Boolean(\[\])?$")
                || Regex.IsMatch(type, @"^System.String(\[\])?$"))
            {
                return type.Substring(type.IndexOf('.') + 1).ToLower();
            }
            switch (type)
            {
                case "System.Byte":
                    return "uint8";
                case "System.SByte":
                    return "int8";
                case "System.Single":
                    return "float32";
                case "System.Double":
                    return "float64";
                case "System.Void":
                    return "void";
                default:
                    return type;
            }
        }

        public static IlTypes ResolveType(TypeInfo type)
        {
            return Regex.IsMatch(type.Name, @"\w+<\s*\w+\s*>") ? IlTypes.ClassGeneric : IlTypes.Class;
        }
        public static IlTypes ResolveType(FieldInfo field)
        {
            return field.SystemInfo.Contains(" static ") ? IlTypes.FieldStatic : IlTypes.FieldInstance;
        }
        public static IlTypes ResolveType(PropertyInfo prop)
        {
            return IlTypes.Property;
        }
        public static IlTypes ResolveType(EventInfo @event)
        {
            return IlTypes.Event;
        }
        public static IlTypes ResolveType(MethodInfo method)
        {
            return (method.SystemInfo.Contains(" static ") ? IlTypes.MethodStatic : IlTypes.MethodInstance)
                | (Regex.IsMatch(method.Name, @"\w+<\s*\w+\s*>") ? IlTypes.MethodGeneric : IlTypes.None);
        }

        #endregion
    }
}