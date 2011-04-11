using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListToXML
{
    public class XmlList
    {
        private String TagName = "";
        private List<XmlList> ListofXml = new List<XmlList>();
        private List<Attributes> Attributes = new List<Attributes>();

        public XmlList(String TagName, List<XmlList> ListofXml, List<Attributes> Attributes)
        {
            this.TagName = TagName;
            this.ListofXml = ListofXml;
            this.Attributes = Attributes;
        }
    }
}
