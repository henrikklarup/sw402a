using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XMLawesome
{
    public class XML
    {
        public int _depth;
        public String _input;
        public String _tag;
        public bool _standalone;
        public String _attr;
        public XML(int depth, String tag, String input, bool standalone, String Attr)
        {
            this._input = input;
            this._depth = depth;
            this._tag = tag;
            this._standalone = standalone;
            this._attr = Attr;
        }

        public static void GenerateThisShizzle(String XmlName, String Encoding)
        {
            XMLhelp.End();
            if (Encoding == null)
            {
                Encoding = "ISO-8859-1";
                Encoding = "<?xml version=\"1.0\" encoding=\"" + Encoding + "\"?>";
            }
            else
            {
                Encoding = "<?xml version=\"1.0\" encoding=\"" + Encoding + "\"?>";
            }
            String a = "<";
            String b = ">";
            String c = "</";
            String XML = a + XmlName + b;
            //Generate the xml string
            List<XML> stack = new List<XML>();

            for(int i = 0; i < XMLhelp.XmlList.Count; i++)
            {
                try
                {
                    ////If i is the last element or _depth equals 0 and i is not 0 pop stack
                    if (i + 1 == XMLhelp.XmlList.Count || (XMLhelp.XmlList[i]._depth == 0 && i != 0))
                    {
                        for (int j = stack.Count - 1; j >= 0; j--)
                        {
                            if (stack[j]._tag != null)
                            {
                                XML += c + stack[j]._tag + b;
                            }
                        }
                        stack.Clear();
                        if (XMLhelp.XmlList[i]._tag != null)
                        {
                            XML += a + XMLhelp.XmlList[i]._tag + XMLhelp.XmlList[i]._attr + b;
                            stack.Add(XMLhelp.XmlList[i]);
                        }
                    } 
                        //If the depth is 0 we print first tag and insert second tag into our stack
                    else
                        if (XMLhelp.XmlList[i]._depth == 0)
                        {
                            if (XMLhelp.XmlList[i]._tag != null)
                            {
                                XML += a + XMLhelp.XmlList[i]._tag + XMLhelp.XmlList[i]._attr + b;

                                stack.Add(XMLhelp.XmlList[i]);
                            }
                        }
                            //If i is standalone it will print first tag, value and second tag
                        else
                            if (XMLhelp.XmlList[i]._standalone == true)
                            {

                                XML += a + XMLhelp.XmlList[i]._tag + XMLhelp.XmlList[i]._attr + b;
                                XML += XMLhelp.XmlList[i]._input;
                                XML += c + XMLhelp.XmlList[i]._tag + b;
                            }
                            else
                            {
                                if (XMLhelp.XmlList[i]._tag != null)
                                {
                                    XML += a + XMLhelp.XmlList[i]._tag + XMLhelp.XmlList[i]._attr + b;
                                    stack.Add(XMLhelp.XmlList[i]);
                                }
                            }


                    }
                catch{}
            }
            XML += c + XmlName + b;

            using (StreamWriter outfile = new StreamWriter(@"C:\Users\Kristian\Desktop\XML\WarGame.xml"))
            {
                outfile.Write(Encoding+XML);
            }
        }

    }
}
