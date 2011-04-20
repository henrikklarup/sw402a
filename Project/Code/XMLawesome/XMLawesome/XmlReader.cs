using System;
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
                return TSearch(limit, search, KeySearch);
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
                return TSearch(limit, search, KeySearch);
            }
        }
        private List<XmlType> TempSearch(int limit, int search, String[] KeySearch)
        {   
            bool SearchTerm = true;
            for(int i = 0; i < KeySearch.Length; i++)
            {
                List<XmlType> status = OrderStack.FindAll(x => x.Tag == KeySearch[i] && x.Order == i);
                
                if(status.Count == 0)
                {
                    SearchTerm = false;
                }
            }

            if (SearchTerm == true)
            {
                Console.WriteLine("SUCCESS");
            }


            //Find search
            

            return ReturnList;
        }
        //Select how much you want
        private List<XmlType> TSearch(int limit, int search, String[] KeySearch)
        {
            List<XmlType> SearchTempList = new List<XmlType>();
            SearchTempList = OrderStack;

            for (int f = 0; f < KeySearch.Length; f++)
            {
                for (int b = 0; b < SearchTempList.Count; b++)
                {
                    int addB = 0;
                    if (SearchTempList[b].Tag != KeySearch[f] && SearchTempList[b].Order == f)
                    {
                        for (int a = b + 1; a < SearchTempList.Count; a++)
                        {
                            if (SearchTempList[b].Order < SearchTempList[a].Order)
                            {
                                SearchTempList[a].Order = -1;

                                addB++;
                            }
                            else
                            {
                                a = SearchTempList.Count;
                            }


                        }
                        b = b + addB;
                    }
                }
            }
           
            ReturnList = SearchTempList.FindAll(x => x.Order >= KeySearch.Length);
            //return OrderStack;
            return ReturnList;
        }
            private List<XmlType> Search(int limit, int search, String[] KeySearch)
            {
                int c = 0;
                int index = 0;
                for (int i = 0; i < limit; i++)
                {
                    List<XmlType> temp = OrderStack.FindAll(x => x.Tag == KeySearch[c]);
                    for(int g = 0; g < temp.Count; g++)
                    {
                        if (temp[g].Order == search && temp[g].Order <= limit)
                        {
                            search++;
                            index = g;
                        }
                    }
                    c++;
                }

                if (search == limit)
                {
                    //search++;
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
                    String[] temp = tempTag.Split('"');
                    List<Attributes> ListAttr = new List<Attributes>();
                    int u = temp.Length;
                    for (int j = 0; j < u - 1; j += 2)
                    {
                        Attributes Attri = new Attributes(temp[j].Replace("=", ""), temp[j + 1]);
                        ListAttr.Add(Attri);
                    }
                    OrderStack[i].Atr = ListAttr;
                }
                else
                {
                    List<Attributes> ListAttr = new List<Attributes>();
                    Attributes Attri = new Attributes(null, null);
                    ListAttr.Add(Attri);
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
            finalList();
            Attributes();
        }







    }
}