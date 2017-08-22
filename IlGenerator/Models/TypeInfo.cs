using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class TypeInfo : CodeInfo
    {
        public ICollection<FieldInfo> Fields { get; set; }
        public ICollection<PropertyInfo> Properties { get; set; }
        public ICollection<EventInfo> Events { get; set; }
        public ICollection<MethodInfo> Methods { get; set; }
        public TypeInfo(string name, string sysInfo) : base(name, sysInfo)
        {
            Fields = new List<FieldInfo>();
            Properties = new List<PropertyInfo>();
            Events = new List<EventInfo>();
            Methods = new List<MethodInfo>();
        }
    }
}