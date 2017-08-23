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

            var parameters = new CompilerParameters(dlls, @"C:\Users\maxbe\Desktop\MyTempFiles\foo.dll", false)
            {
                GenerateExecutable = false
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
            TypeInfo currType = new TypeInfo(td.Name, GetTypeSystemInfo(td));
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
        private static string GetTypeSystemInfo(TypeDefinition td)
        {
            return td.IsClass ? " .class " : ""
                + (td.IsValueType ? "valueType " : "");
        }


        private static MethodInfo GetMethodInfo(MethodDefinition md)
        {
            string name = $"{GetAlias(md.ReturnType.ToString())} {md.Name}";
            string sysinfo = GetMethodSystemInfo(md);
            var instructions = md.Body.Instructions;
            StringBuilder body = new StringBuilder(256);
            foreach (var instr in instructions)
            {
                body.Append(instr.ToString() + Environment.NewLine);
            }

            return new MethodInfo(name, sysinfo, body.ToString());
        }
        private static string GetMethodSystemInfo(MethodDefinition md)
        {
            string sysInfo = GetAccessibility(md)
                 + (md.IsAbstract ? " abstract" : "")
                 + (md.IsAddOn ? " addon" : "")
                 + (md.IsRemoveOn ? " removeon" : "")
                 + (md.IsGetter ? " getter" : "")
                 + (md.IsSetter ? "setter" : "")
                 + (md.IsStatic ? " static" : " instance")//!!!
                 + (md.IsHideBySig ? " hidebysig" : "")
                 + (md.IsPreserveSig ? " preservesig" : "")
                 + (md.IsFinal ? " final" : "")
                 + (md.IsNewSlot ? " newslot" : "")
                 + (md.IsVirtual ? " virtual" : "")
                 + (md.IsSpecialName ? " specialname" : "")
                 + (md.IsRuntimeSpecialName ? " rtspecialname" : "")
                 + (md.IsManaged ? " cil managed" : " umnanaged")
                 + $"{Environment.NewLine}return: ({md.ReturnType})"
                 + $"{Environment.NewLine}params: ({string.Join(", ", md.Parameters)})";//!!!

            return sysInfo;
        }


        private static EventInfo GetEventInfo(EventDefinition ed)
        {
            string name = $"{GetAlias(ed.EventType.ToString())} {ed.Name}";
            string sysinfo = GetEventSystemInfo(ed);
            string addOn = GetMethodSystemInfo(ed.AddMethod);
            string removeOn = GetMethodSystemInfo(ed.RemoveMethod);

            return new EventInfo(name, sysinfo, addOn, removeOn);
        }
        private static string GetEventSystemInfo(EventDefinition ed)
        {
            string sysinfo = $"{ed.EventType}";

            return sysinfo;
        }


        private static PropertyInfo GetPropertyInfo(PropertyDefinition pd)
        {
            string name = $"{GetAlias(pd.PropertyType.ToString())} {pd.Name}";
            string sysinfo = GetPropertySystemInfo(pd);
            string getter = GetMethodSystemInfo(pd.GetMethod);
            string setter = GetMethodSystemInfo(pd.SetMethod);

            return new PropertyInfo(name, sysinfo, getter, setter);
        }
        private static string GetPropertySystemInfo(PropertyDefinition pd)
        {
            string sysinfo = pd.PropertyType.ToString();

            return sysinfo;
        }


        private static FieldInfo GetFieldInfo(FieldDefinition fd)
        {
            string name = $"{GetAlias(fd.FieldType.ToString())} {fd.Name}";
            string sysinfo = GetFieldSystemInfo(fd);

            return new FieldInfo(name, sysinfo);
        }
        private static string GetFieldSystemInfo(FieldDefinition fd)
        {
            string sysinfo = GetAccessibility(fd)
                + (fd.IsStatic ? " static" : " instance")
                + (fd.IsInitOnly ? " initonly" : "")
                + " " + fd.FieldType;
            return sysinfo;

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

        private static string GetAlias(string type)
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
                    return "int8";
                case "System.SByte":
                    return "uint8";
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

        public static object ToTree(IEnumerable<TypeInfo> types)
        {
            var asm = new
            {
                text = "Assembly",
                state = "opened",
                children = types.Select(type =>
                    new
                    {
                        text = type.Name,
                        children = new []
                        {
                            new
                            {
                                text = "Fields",
                                children = type.Fields.Select(x => new
                                {
                                    text = x.Name,
                                    sysinfo = x.SystemInfo,
                                    data = "",
                                    type = "demo"
                                })
                            },
                            new
                            {
                                text = "Properties",
                                children = type.Properties.Select(x => new
                                {
                                    text = x.Name,
                                    sysinfo = x.SystemInfo,
                                    data = x.GetterInfo + Environment.NewLine + x.SetterInfo,
                                    type = "demo"

                                })
                            },
                            new
                            {
                                text = "Events",
                                children = type.Events.Select(x => new
                                {
                                    text = x.Name,
                                    sysinfo = x.SystemInfo,
                                    data = x.AddOnInfo + Environment.NewLine + x.RemoveOnInfo,
                                    type = "demo"
                                })
                            },
                            new
                            {
                                text = "Methods",
                                children = type.Methods.Select(x => new
                                {
                                    text = x.Name,
                                    sysinfo = x.SystemInfo,
                                    data = x.MethodBody,
                                    type = "demo"
                                })
                            }
                        }
                    })
            };


            return asm;
        }
    }
}
