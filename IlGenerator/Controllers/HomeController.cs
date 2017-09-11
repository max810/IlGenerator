using IlGenerator.Models;
using Microsoft.CSharp;
using Mono.Cecil;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace IlGenerator.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        //param name is important
        public JsonResult ResultCode(string sourceCode)
        {
            string sourceCodeDecoded = WebUtility.HtmlDecode(sourceCode);

            string assemblyName = Guid.NewGuid().ToString();

            CompilerResults compiled = SourceCodeGenerator.CompileDefaultAssembly(sourceCodeDecoded, assemblyName);

            var assembly = AssemblyDefinition.ReadAssembly(compiled.PathToAssembly);

            var resultCodeInfo = SourceCodeGenerator.GenerateIlCode(assembly);

            var tree = JsTreeFormatter.ToJSTree(resultCodeInfo);

            var allErrors = compiled.Errors.Cast<CompilerError>().Select(x => new ErrorInfo(x));

            string dirPath = Request.MapPath("~/App_Data/TempAssemblies");
            string assemblyFullName = Path.Combine(dirPath, assemblyName + ".dll");
            string debugFullName = Path.Combine(dirPath, assemblyName + ".pdb");

            if(System.IO.File.Exists(assemblyFullName))
                System.IO.File.Delete(assemblyFullName);
            if (System.IO.File.Exists(assemblyFullName))
                System.IO.File.Delete(debugFullName);

            return Json(new
            {
                Tree = tree,
                Errors = allErrors.Where(x => !x.IsWarning),
                Warnings = allErrors.Where(x => x.IsWarning),
            });
        }
    }
}