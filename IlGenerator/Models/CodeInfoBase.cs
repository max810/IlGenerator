using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public abstract class CodeInfoBase
    {
        public string Name { get; set; }
        public string SystemInfo { get; set; }
        public string CustomAttributes { get; set; }
        public CodeInfoBase(string name, string sysInfo, string attrs)
        {
            Name = name;
            SystemInfo = sysInfo;
            CustomAttributes = attrs;
        }
    }
}