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
        public char[] Split = { '@' };

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
            XmlFile = XmlFile.Replace(">@@<", ">@<");
            temp = XmlFile.Split(Split);
            streamReader.Close();
            int order = 0;

            for (int i = 0; i < temp.Length - 1; i++)
            {       //Check for </> <> sæt order

                if (temp[i].Contains("</"))
                {
                    order--;
                }
                else if (temp[i].Contains("<"))
                {
                    order++;
                }
                XmlOrder XmlO = new XmlOrder(temp[i], order);
                XML.Add(XmlO);
            }
        }

        public void XmlListGenerate()
        {

        }

        public String[] GetNode(String command)
        {

            String[] teemp = { "" };
            return teemp;
        }

        List<XmlType> OrderStack = new List<XmlType>();

        public List<XmlType> ReturnOrderStack()
        {
            return OrderStack;
        }

        public void TreeLists()
        {
            int number = 0;
            for (int i = 0; i < XML.Count; i++)
            {
                try
                {
                XmlOrder atm = XML[i];
                
                    XmlOrder prev = XML[i - 1];
                    XmlOrder next = XML[i + 1];
                

                if (i == 1)
                {
                    XmlType XmlOrderType = new XmlType(atm.Tag, null, "root", number);
                    OrderStack.Add(XmlOrderType);
                }
                else if (atm.Order > prev.Order)
                {
                    number++;
                    XmlType XmlOrderType = new XmlType(atm.Tag, null, "nest", number);
                    OrderStack.Add(XmlOrderType);
                }
                else if (atm.Order < prev.Order)
                {
                    number--;
                }
                else if (atm.Order == prev.Order)
                {
                    if (atm.Order > next.Order)
                    {
                        XmlType XmlOrderType = new XmlType(prev.Tag, atm.Tag, "Standalone", number);
                        OrderStack.Add(XmlOrderType);
                    }
                }
                }
                catch { };

            }
        }
        List<XmlList> ToDoStack = new List<XmlList>();
        List<XmlList> ListStack = new List<XmlList>();

        public void finalList()
        {
            try
            {
                //public XmlList(String TagName, String Value, List<XmlList> ListofXml, List<Attributes> Attributes)
                for (int i = 0; i < OrderStack.Count; i++)
                {
                    if (i == 1)
                    {
                        List<XmlList> Empty = new List<XmlList>();
                        XmlList DoList = new XmlList(OrderStack[i].Tag, null, Empty, null);
                        ToDoStack.Add(DoList);

                    }
                    else if (OrderStack[i].Order < OrderStack[i + 1].Order)
                    {
                        List<XmlList> Empty = new List<XmlList>();
                        XmlList DoList = new XmlList(OrderStack[i].Tag, null, Empty, null);
                        ToDoStack.Add(DoList);
                    }
                    else if (OrderStack[i].Type == "Standalone")
                    {
                        bool IsNextStandalone = true;
                        int k = 1;
                        XmlList Stand = new XmlList(OrderStack[i].Tag, OrderStack[i].Value, null, null);
                        ToDoStack[ToDoStack.Count - 1].ListofXml.Add(Stand);
                        while (IsNextStandalone == true)
                        {
                            if (OrderStack[i + k].Type == "Standalone")
                            {
                                XmlList XStand = new XmlList(OrderStack[i + k].Tag, OrderStack[i + k].Value, null, null);
                                ToDoStack[ToDoStack.Count - 1].ListofXml.Add(XStand);
                                k++;
                            }
                            else
                            {
                                IsNextStandalone = false;
                            }
                            i = i + k;
                        }
                    }
                    else if (OrderStack[i].Order < OrderStack[i - 1].Order)
                    {
                        XmlList XStand = new XmlList(OrderStack[i].Tag, OrderStack[i].Value, null, null);
                        ToDoStack[ToDoStack.Count - 1].ListofXml.Add(XStand);
                    }
                }
            }
            catch { }
        }

        public List<XmlList> GetToDoStack()
        {
            return ToDoStack;
        }










    }
}