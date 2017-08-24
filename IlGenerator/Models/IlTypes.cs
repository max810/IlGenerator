using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    [Flags]
    public enum IlTypes
    {
        None = 0,
        FieldInstance = 0x1,
        FieldStatic = 0x2,
        Property = 0x4,
        MethodInstance = 0x8,
        MethodStatic = 0x10,
        MethodGeneric = 0x20,
        Interface = 0x40,
        InterfaceGeneric = 0x80,
        Event = 0x100,
        Class = 0x200,
        ClassGeneric = 0x400,
        Struct = 0x800,
        StructGeneric = 0x1000,
        Enumeration = 0x2000
    }
}