using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class MethodInfo : CodeInfo
    {
        public string MethodBody { get; set; }
        public MethodInfo(string name, string sysInfo, string methodBody) : base(name, sysInfo)
        {
            MethodBody = methodBody;
        }
    }
}