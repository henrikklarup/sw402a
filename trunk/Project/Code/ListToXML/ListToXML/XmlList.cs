﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListToXML
{
    public class XmlList
    {
        public String TagName;
        public String Value;
        public List<XmlList> ListofXml = new List<XmlList>();
        public List<Attributes> Attributes = new List<Attributes>();

        public XmlList(String TagName, String Value, List<XmlList> ListofXml, List<Attributes> Attributes)
        {
            this.TagName = TagName;
            this.Value = Value;
            this.ListofXml = ListofXml;
            this.Attributes = Attributes;
        }
    }
}