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

            CompilerResults compiled = SourceCodeProcessor.CompileDefaultAssembly(sourceCodeDecoded);

            var assembly = AssemblyDefinition.ReadAssembly(compiled.PathToAssembly);

            var resultCode = SourceCodeProcessor.GenerateIlCode(assembly);

            var tree = SourceCodeProcessor.ToTree(resultCode);

            return Json(new
            {
                Tree = tree,
                Errors = compiled.Errors
            });
        }
    }
}