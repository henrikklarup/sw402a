﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace XMLawesome
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

            try
            {
                StreamReader streamReader = new StreamReader(file);
                String XmlFile = streamReader.ReadToEnd();
            if (XmlFile.Contains("<?"))
            {
                XmlFile = XmlFile.Replace("?>", "?>@");
                String[] tempArray = XmlFile.Split(Split);
                String encoding = tempArray[0];
                XmlFile = tempArray[1];
            }
            XmlFile = XmlFile.Replace("<", "@<");
            XmlFile = XmlFile.Replace(">", ">@");
            XmlFile = XmlFile.Replace(">@@<", ">@<");
            temp = XmlFile.Split(Split);
            streamReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read XML file: " + file);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            int order = 0;

            for (int i = 0; i < temp.Length - 1; i++)
            {       //Check for </> <> set order

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



        List<XmlType> OrderStack = new List<XmlType>();

        public List<XmlType> ReturnOrderStack()
        {
            return OrderStack;
        }

        public void TreeLists()
        {
            int number = 0;
            for (int i = 1; i < XML.Count; i++)
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
                        number++;
                    }
                    else if (atm.Order > prev.Order)
                    {
                        if (atm.Order == next.Order)
                        {
                            XmlType XmlOrderType = new XmlType(atm.Tag, next.Tag, "Standalone", number);
                            OrderStack.Add(XmlOrderType);
                            i = i + 2;
                        }
                        else
                        {
                            XmlType XmlOrderType = new XmlType(atm.Tag, null, "nest", number);
                            OrderStack.Add(XmlOrderType);
                            number++;
                        }
                    }
                    else if (atm.Order < prev.Order)
                    {
                        number--;
                    }
                    else if (atm.Order == prev.Order)
                    {
                        if (atm.Order > next.Order)
                        {
                            XmlType XmlOrderType = new XmlType(atm.Tag, atm.Tag, "Standalone", number);
                            OrderStack.Add(XmlOrderType);
                        }
                    }
                }
                catch { };

            }
        }
        List<XmlList> RootStack = new List<XmlList>();
        List<XmlList> ListStack = new List<XmlList>();
        List<XmlType> ReturnList = new List<XmlType>();
        public List<XmlType> XmlSearch(String searchterm)
        {
            int limit = 0;
            int search = 0;
            String[] KeySearch = { null };

            if (searchterm.Contains(">"))
            {
                KeySearch = searchterm.Split('>');
                limit = KeySearch.Length;
                search = 0;
                return Search(limit, search, KeySearch);
            }
            else if (searchterm == "")
            {
                return OrderStack;
            }
            else
            {
                KeySearch[0] = searchterm;
                limit = 1;
                search = 0;
                return Search(limit, search, KeySearch);
            }
        }
            private List<XmlType> Search(int limit, int search, String[] KeySearch)
            {
                for (int i = 0; i < limit; i++)
                {
                    int c = 0;
                    List<XmlType> temp = OrderStack.FindAll(x => x.Tag == KeySearch[c]);
                    foreach (XmlType item in temp)
                    {
                        if (item.Order == search)
                        {
                            search++;
                        }
                    }
                    c++;
                }

                if (search == limit)
                {
                    //search++;
                    int index = OrderStack.FindIndex(x => x.Order == search);
                    for (int u = index; u < OrderStack.Count; u++)
                    {
                        if (search <= OrderStack[u].Order)
                        {
                            ReturnList.Add(OrderStack[u]);
                        }
                    }
                }

                return ReturnList;
        }

        public void finalList()
        {
            try
            {
                for (int i = 0; i < OrderStack.Count; i++)
                {
                    if (OrderStack[i].Type == "root")
                    {
                        List<XmlList> empty = new List<XmlList>();
                        XmlList root = new XmlList(OrderStack[i].Tag, OrderStack[i].Value, empty, null);
                        RootStack.Add(root);
                    }
                    else if (OrderStack[i].Type == "nest")
                    {
                        List<XmlList> empty = new List<XmlList>();
                        XmlList root = new XmlList(OrderStack[i].Tag, OrderStack[i].Value, empty, null);
                        RootStack.Add(root);
                    }
                    else if (OrderStack[i].Type == "Standalone")
                    {
                        bool Check = true;
                        int k = 0;
                        while (Check == true)
                        {

                            if (OrderStack[i + k].Type == "Standalone")
                            {
                                List<XmlList> empty = new List<XmlList>();
                                XmlList stand = new XmlList(OrderStack[i + k].Tag, OrderStack[i + k].Value, empty, null);
                                RootStack[RootStack.Count - 1].ListofXml.Add(stand);
                                k++;
                            }
                            i = i + k;
                        }
                    }

                    
                }
            }
            catch { }
        }

        public void Attributes()
        {
            for (int i = 0; i < OrderStack.Count; i++)
            {
                if (OrderStack[i].Tag.Contains("=\""))
                {
                    //Parse tag
                    String tempTag = OrderStack[i].Tag;
                    int index = tempTag.IndexOf(" ");
                    String tempSub = tempTag.Substring(0, index);
                    OrderStack[i].Tag = tempSub;
                    tempTag = tempTag.Replace(tempSub, "");
                    Console.WriteLine("CHECK THIS OUT" + tempSub);
                    String[] temp = tempTag.Split('"');
                    List<Attributes> ListAttr = new List<Attributes>();
                    int u = temp.Length;
                    for(int j = 0; j < u-1; j=+2)
                    {
                        Attributes Attri = new Attributes(temp[j].Replace("=",""),temp[j+1]);
                        ListAttr.Add(Attri);
                    }
                    OrderStack[i].Atr = ListAttr;
                }
            }
        }

        public List<XmlList> GetToDoStack()
        {
            return RootStack;
        }


        public void Mount()
        {
            TreeLists();
            //finalList();
            Attributes();
            Console.WriteLine("HEHAHAHA");
        }







    }
}