﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class EventInfo : CodeInfoBase
    {
        public string AddOnInfo { get; set; }
        public string RemoveOnInfo { get; set; }
        public EventInfo(string name, string sysInfo, string attrs, string addOn, string removeOn) : base(name, sysInfo, attrs)
        {
            if(string.IsNullOrWhiteSpace(addOn) || string.IsNullOrWhiteSpace(removeOn))
            {
                throw new ArgumentException("Event must have both remove and add methods");
            }
            AddOnInfo = addOn;
            RemoveOnInfo = removeOn;
        }
    }
}