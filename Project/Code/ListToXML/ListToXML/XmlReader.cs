using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListToXML
{
    public class XmlReader
    {

        public String[] XML;
        public char[] Split = {'@'};

        public String[] getXML()
        {
            return XML;
        }

        public XmlReader(String file)
        {
            StreamReader streamReader = new StreamReader(@"C:\WarGame.txt");
            string XmlFile = streamReader.ReadToEnd();

            Console.WriteLine("\r\nXml Start\r\n");
            Console.WriteLine(XmlFile);
            Console.WriteLine("\r\nXml slut\r\n");

            XmlFile.Replace("<", "@<");
            XmlFile.Replace(">", "@>");
            XML = XmlFile.Split(Split);
            streamReader.Close();
        }
    }
}
