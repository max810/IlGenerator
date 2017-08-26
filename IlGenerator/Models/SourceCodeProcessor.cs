using Microsoft.CSharp;
using Mono.Cecil;
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
    public static class SourceCodeProcessor
    {
        public static CompilerResults CompileDefaultAssembly(string sourceCode)
        {
            var csc = new CSharpCodeProvider();
            string[] dlls = Properties.Settings.Default.CompilerDllReferences.Split(' ');

            var parameters = new CompilerParameters(dlls, @"C:\Users\maxbe\Desktop\MyTempFiles\foo.dll", true)
            {
                GenerateExecutable = false,
                WarningLevel = 4
            };

            CompilerResults compiled = csc.CompileAssemblyFromSource(parameters, sourceCode);
            return compiled;
        }

        public static IEnumerable<TypeInfo> GenerateIlCode(AssemblyDefinition asm)
        {
            List<TypeInfo> types = new List<TypeInfo>();
            //!!!
            //Skip(1) because the <module> definition is somehow there, before any of the classes
            //It doesn't contain anything - no class members
            //It shouldn't be there because we are iterating through MainModule's types
            //Perhaps a bug in the library
            //!!!
            foreach (TypeDefinition td in asm.MainModule.Types.Skip(1))
            {
                var tinf = GetTypeInfo(td);
                types.Add(tinf);
            }
            return types;
        }
        private static TypeInfo GetTypeInfo(TypeDefinition td)
        {
            var name = td.Name;
            if (td.HasGenericParameters) {
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
        private static string GetSystemInfo(TypeDefinition td)
        {
            var sysinfo = (td.IsClass ? ".class" : "")
                + (td.IsPublic ? " public" : " private")
                + (td.IsAutoClass ? " auto" : "")
                + (td.IsAnsiClass ? " ansi" : "")
                + (td.IsBeforeFieldInit ? " beforefieldinit" : "")
                + ($" ofname {td.Name}");

            if(!string.Equals(td.BaseType.Name, "Object", StringComparison.InvariantCultureIgnoreCase))
            {
                sysinfo += Environment.NewLine + "\textends " + td.BaseType.FullName;
            }
            
            if (td.IsValueType)
            {
                sysinfo += " valuetype";
            }
            return sysinfo;
        }

        private static FieldInfo GetFieldInfo(FieldDefinition fd)
        {
            string name = $"{GetTypeAlias(fd.FieldType)} {fd.Name}";
            string sysinfo = GetSystemInfo(fd);

            return new FieldInfo(name, sysinfo, GetCustomAttributes(fd));
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

        private static PropertyInfo GetPropertyInfo(PropertyDefinition pd)
        {
            string name = $"{GetTypeAlias(pd.PropertyType)} {pd.Name}";
            string sysinfo = GetSystemInfo(pd);
            string getter = GetSystemInfo(pd.GetMethod);
            string setter = GetSystemInfo(pd.SetMethod);

            return new PropertyInfo(name, sysinfo, GetCustomAttributes(pd), getter, setter);
        }
        private static string GetSystemInfo(PropertyDefinition pd)
        {
            
            string sysinfo = $".property {pd.PropertyType}"
                + (pd.IsSpecialName ? " specialname" : "")
                + (pd.IsRuntimeSpecialName ? " rtspecialname" : "")
                + $" ofname {pd.Name}";

            return sysinfo;
        }

        private static EventInfo GetEventInfo(EventDefinition ed)
        {
            string name = $"{GetTypeAlias(ed.EventType)} {ed.Name}";
            string sysinfo = GetSystemInfo(ed);
            string addOn = GetSystemInfo(ed.AddMethod);
            string removeOn = GetSystemInfo(ed.RemoveMethod);

            return new EventInfo(name, sysinfo, GetCustomAttributes(ed), addOn, removeOn);
        }
        private static string GetSystemInfo(EventDefinition ed)
        {
            string sysinfo = $".event {ed.EventType}"
                + (ed.IsSpecialName ? " specialname" : "")
                + (ed.IsRuntimeSpecialName ? " rtspecialname" : "")
                + $" ofname {ed.Name}";

            return sysinfo;
        }

        private static MethodInfo GetMethodInfo(MethodDefinition md)
        {
            string name = $"{md.Name} : {GetTypeAlias(md.ReturnType)}" +
                $"({string.Join(", ", md.Parameters.Select(x => GetTypeAlias(x.ParameterType)))})";
            if (md.HasGenericParameters)
            {
                name += $"<{string.Join(", ", md.GenericParameters)}>";
            }

            string sysinfo = GetSystemInfo(md);
            var instructions = md.Body.Instructions;
            StringBuilder body = new StringBuilder(256);

            body.Append(string.Join(Environment.NewLine, instructions));
            string fullBodyCode = $".maxstack {md.Body.MaxStackSize}{Environment.NewLine}{body.ToString()}";
            //if (md.Body.HasVariables)
            //{
            //    fullBodyCode = $".locals init ({string.Join(", ", md.Body.Variables.Select(x => $"[{x.Index}] {GetTypeAlias(x.VariableType)}"))}{Environment.NewLine})" + fullBodyCode;
            //}
            return new MethodInfo(name, sysinfo, GetCustomAttributes(md), fullBodyCode);
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

        //------------System Methods------------

        private static string GetCustomAttributes(ICustomAttributeProvider attributeProvider)
        {
            if(attributeProvider.HasCustomAttributes)
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
                + $" = ( {string.Join(" ", attr.GetBlob())} )";
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
            useAlias
            ? $"{GetTypeAlias(x.ParameterType)}"
            : x.ParameterType.ToString()
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

        private static IlTypes ResolveType(TypeInfo type)
        {
            return Regex.IsMatch(type.Name, @"\w+<\s*\w+\s*>") ? IlTypes.ClassGeneric : IlTypes.Class;
        }

        private static IlTypes ResolveType(FieldInfo field)
        {
            return field.SystemInfo.Contains(" static ") ? IlTypes.FieldStatic : IlTypes.FieldInstance;
        }
        private static IlTypes ResolveType(PropertyInfo prop)
        {
            return IlTypes.Property;
        }
        private static IlTypes ResolveType(EventInfo @event)
        {
            return IlTypes.Event;
        }
        private static IlTypes ResolveType(MethodInfo method)
        {
            return (method.SystemInfo.Contains(" static ") ? IlTypes.MethodStatic : IlTypes.MethodInstance)
                | (Regex.IsMatch(method.Name, @"\w+<\s*\w+\s*>") ? IlTypes.MethodGeneric : IlTypes.None);
        }

        //------------To jstree (probably temporary method)------------

        public static object ToJSTree(IEnumerable<TypeInfo> types)
        {
            var tree = new
            {
                text = "Assembly",
                children = types.Select(type =>
                    new JSTreeNode(type.Name, type.SystemInfo, type.CustomAttributes, ResolveType(type))
                    {
                        children = new object[]
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
                                    x.CustomAttributes,
                                    ResolveType(x)))
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
                                    (string.IsNullOrWhiteSpace(x.CustomAttributes) ? "" : (x.CustomAttributes + Environment.NewLine))
                                        + x.GetterInfo
                                        + Environment.NewLine + x.SetterInfo,
                                    ResolveType(x)))
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
                                    (string.IsNullOrWhiteSpace(x.CustomAttributes) ? "" : (x.CustomAttributes + Environment.NewLine))
                                        + x.AddOnInfo
                                        + Environment.NewLine + x.RemoveOnInfo,
                                    ResolveType(x)))
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
                                    (string.IsNullOrWhiteSpace(x.CustomAttributes) ? "" : (x.CustomAttributes + Environment.NewLine))
                                        + x.MethodBody,
                                    ResolveType(x)))
                            },
                        }
                    })
            };

            return tree;
        }
    }
}

//TODO
//2. Error messages instead of tree
//3. Errors highlight
//4. Move tree making from c# to js 
//5. Not ugly icons
