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
        public ActionResult Index(bool? mobile)
        {
            if (mobile == null)
                mobile = false;
            ViewBag.Mobile = mobile;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        //param name is important = 'sourceCode' !
        public JsonResult ResultCode(string sourceCode)
        {
            string sourceCodeDecoded = WebUtility.HtmlDecode(sourceCode);

            string assemblyName = Guid.NewGuid().ToString();

            CompilerResults compiled = SourceCodeGenerator.CompileDefaultAssembly(sourceCodeDecoded, assemblyName);

            var allErrors = compiled.Errors.Cast<CompilerError>().Select(x => new ErrorInfo(x));
            var errors = allErrors.Where(x => !x.IsWarning);
            var warnings = allErrors.Where(x => x.IsWarning);

            if (errors.Any())
            {
                return Json(new
                {
                    Errors = errors,
                    Warnings = warnings
                });
            }

            var assembly = AssemblyDefinition.ReadAssembly(compiled.PathToAssembly);

            var resultCodeInfo = SourceCodeGenerator.GenerateIlCode(assembly);

            var tree = JsTreeFormatter.ToJSTree(resultCodeInfo);

            string assemblyFullName = Path.Combine(Path.GetTempPath(), assemblyName + ".dll");
            if(System.IO.File.Exists(assemblyFullName))
                System.IO.File.Delete(assemblyFullName);

            return Json(new
            {
                Tree = tree,
                Errors = errors,
                Warnings = warnings
            });
        }
    }
}