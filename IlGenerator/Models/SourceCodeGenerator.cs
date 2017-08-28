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
    public static class SourceCodeGenerator
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
            //Skip(1) because the <Module> definition is somehow there, before any of the classes
            //It doesn't contain anything - no class members
            //It shouldn't be there because we are iterating through MainModule's types
            //Perhaps a bug in the library
            //!!!
            foreach (TypeDefinition td in asm.MainModule.Types.Skip(1))
            {
                var tinf = SourceCodeFormatter.GetTypeInfo(td);
                types.Add(tinf);
            }
            return types;
        }
    }
}

//TODO
//2. Error messages instead of tree
//3. Errors highlight
//4. Move tree making from c# to js 
//5. Not ugly icons
