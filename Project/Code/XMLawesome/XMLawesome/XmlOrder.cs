using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace XMLawesome
{
    public class XmlOrder
    {
        private String _Tag = "";
        private int _Order;
        public XmlOrder(String Tag, int Order)
        {
            this._Tag = Tag;
            this._Order = Order;
        }

        public String Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;
            }
        }

        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
            }
        }
    }
}
