using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class TypeInfo : CodeInfoBase
    {
        public ICollection<FieldInfo> Fields { get; set; }
        public ICollection<PropertyInfo> Properties { get; set; }
        public ICollection<EventInfo> Events { get; set; }
        public ICollection<MethodInfo> Methods { get; set; }
        public TypeInfo(string name, string sysInfo, string attrs) : base(name, sysInfo, attrs)
        {
            Fields = new List<FieldInfo>();
            Properties = new List<PropertyInfo>();
            Events = new List<EventInfo>();
            Methods = new List<MethodInfo>();
        }
    }
}