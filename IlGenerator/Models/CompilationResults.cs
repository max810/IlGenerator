using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class CompilationResults
    {
        public object CompiledAssembly;
        public IEnumerable<string> Errors;
    }
}