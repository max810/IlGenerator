using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class FieldInfo : CodeInfoBase
    {
        public FieldInfo(string name, string sysInfo, string attrs) : base(name, sysInfo, attrs)
        {
        }
    }
}