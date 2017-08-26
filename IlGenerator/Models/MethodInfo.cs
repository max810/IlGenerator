using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class MethodInfo : CodeInfoBase
    {
        public string MethodBody { get; set; }
        public MethodInfo(string name, string sysInfo, string attrs, string methodBody) : base(name, sysInfo, attrs)
        {
            MethodBody = methodBody;
        }
    }
}