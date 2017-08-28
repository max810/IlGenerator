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

            CompilerResults compiled = SourceCodeGenerator.CompileDefaultAssembly(sourceCodeDecoded);

            var assembly = AssemblyDefinition.ReadAssembly(compiled.PathToAssembly);

            var resultCodeInfo = SourceCodeGenerator.GenerateIlCode(assembly);

            var tree = JsTreeFormatter.ToJSTree(resultCodeInfo);

            var allErrors = compiled.Errors.Cast<CompilerError>().Select(x => new ErrorInfo(x));

            return Json(new
            {
                Tree = tree,
                Errors = allErrors.Where(x => !x.IsWarning),
                Warnings = allErrors.Where(x => x.IsWarning)
            });
        }
    }
}