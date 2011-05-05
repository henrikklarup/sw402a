using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MASClassLibrary;

namespace XMLawesome
{
    public static class XMLhelp
    {
        private static int childCount = 0;
        public static int Cdepth = -1;
        public static int Cint = 0;
        public static List<aXML> XmlList = new List<aXML>();
        public static int Depth = 0;
        private static bool single = true;

        public static void Root(String Tag, String Value)
        {
            //Return to root
            Depth = 0;
            aXML temp = new aXML(Depth, Tag, Value, false,null,0);
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
            aXML temp = new aXML(-1, null, null, false, null,0);
            XmlList.Add(temp);
        }

        public static void Child(String Tag, String Value)
        {
            //Make child
            childCount++;
            Depth++;
            aXML temp = new aXML(Depth, Tag, Value, false, null,0);
            XmlList.Add(temp);
            Depth++;
        }

        public static void Parent()
        {
            //Make parent
            Depth--;
            Depth--;
            //XML temp = new XML(Depth, null, null, false, null);
           // XmlList.Add(temp);
        }

        public static void Node(String Tag, String Value)
        {
            aXML temp = new aXML(Depth, Tag, Value, true, null,0);
            XmlList.Add(temp);
        }

        public static void LastNode(String Tag, String Value)
        {
            aXML temp = new aXML(Depth, Tag, Value, true, null, childCount);
            XmlList.Add(temp);
            Depth = Depth - childCount;
            childCount = 0;
        }
    }
}
