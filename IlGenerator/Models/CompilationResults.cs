using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class CompilationResults
    {
        object CompiledAssembly;
        IEnumerable<string> Errors;
    }
}