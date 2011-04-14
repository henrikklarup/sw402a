using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XMLawesome
{
    public static class XMLhelp
    {
        public static List<XML> XmlList = new List<XML>();
        public static int Depth = 0;

        public static void Root(String Tag, String Value)
        {
            //Return to root
            Depth = 0;
            XML temp = new XML(Depth, Tag, Value, false,null);
            XmlList.Add(temp);
        }
        
        public static void Attribute(String tag, String value)
        {
            String atr = " " + tag.ToLower() + "=\"" + value + "\"";
            XmlList.Last()._attr += atr;
        }

        public static void Attribute(List<Attributes> input)
        {
            String atr = "";
            foreach (Attributes attri in input)
            {
                atr += " " + attri.Atr.ToLower() + "=\"" + attri.Value + "\"";
            }
            XmlList.Last()._attr += atr;
        }

        public static void End()
        {
            //Return to root
            Depth = 0;
            XML temp = new XML(-1, null, null, false, null);
            XmlList.Add(temp);
        }

        public static void Child(String Tag, String Value)
        {
            //Make child
            Depth++;
            XML temp = new XML(Depth, Tag, Value, false, null);
            XmlList.Add(temp);
            Depth++;
        }

        public static void Parent()
        {
            //Make parent
            Depth--;
            Depth--;
            Depth--;
            XML temp = new XML(Depth, null, null, false, null);
            XmlList.Add(temp);
        }

        public static void Node(String Tag, String Value)
        {
            XML temp = new XML(Depth, Tag, Value, true, null);
            XmlList.Add(temp);
        }
    }
}
