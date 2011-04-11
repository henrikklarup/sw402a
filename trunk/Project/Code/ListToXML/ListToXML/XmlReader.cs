using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListToXML
{
    public class XmlReader
    {

        private List<List<Object>> List = new List<List<Object>>();
        private String[] temp;
        private List<XmlOrder> XML = new List<XmlOrder>();
        public char[] Split = {'@'};

        public List<XmlOrder> getXML()
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
            XmlFile = XmlFile.Replace(">@@<", "@");
            temp = XmlFile.Split(Split);
            streamReader.Close();
            int order = 0;

            for(int i = 1; i < temp.Length-1; i++)
            {       //Check for </> <> sæt order
                    XmlOrder XmlO = new XmlOrder(temp[i],order);
                    XML.Add(XmlO);
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