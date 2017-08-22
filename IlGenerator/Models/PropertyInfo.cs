using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class PropertyInfo : CodeInfo
    {
        public string GetterInfo { get; set; }
        public string SetterInfo { get; set; }
        public PropertyInfo(string name, string sysInfo, string getter, string setter) : base(name, sysInfo)
        {
            if(string.IsNullOrWhiteSpace(getter) && string.IsNullOrWhiteSpace(setter))
            {
                throw new ArgumentException("Property should have at least one accessor");
            }
            GetterInfo = getter;
            SetterInfo = setter;
        }
    }
}