using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListToXML
{
    public class XmlReader
    {

        private String[] temp;
        private List<String> XML = new List<String>();
        public char[] Split = {'@'};

        public List<String> getXML()
        {
            return XML;
        }

        public XmlReader(String file)
        {
            StreamReader streamReader = new StreamReader(file);
            string XmlFile = streamReader.ReadToEnd();

            
            Console.WriteLine(XmlFile);

            XmlFile = XmlFile.Replace("<", "@<");
            XmlFile = XmlFile.Replace(">", ">@");
            temp = XmlFile.Split(Split);
            streamReader.Close();

            foreach (String item in temp)
            {
                if (item != null)
                {
                    XML.Add(item);
                }
            }
        }
        
        public String[] GetNode(String command)
        {
            
            String[] teemp = {""};
            return teemp;
        }

        public void GenerateList()
        { 
        
        }


    }
}