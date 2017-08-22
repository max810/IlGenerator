using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public abstract class CodeInfo
    {
        public string Name { get; set; }
        public string SystemInfo { get; set; }
        public string Attributes { get; set; }
        public CodeInfo(string name, string sysInfo)
        {
            Name = name;
            SystemInfo = sysInfo;
        }
    }
}