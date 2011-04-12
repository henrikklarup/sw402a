using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListToXML
{
    public class XmlType
    {

        public String Tag;
        public String Value;
        public String Type;
        public int Order;

        public XmlType(String Tag, String Value, String Type, int Order)
        {
            this.Tag = Tag.Replace("<","");
            this.Tag = this.Tag.Replace(">", "");
            this.Value = Value;
            this.Type = Type;
            this.Order = Order;
        }
    }
}
