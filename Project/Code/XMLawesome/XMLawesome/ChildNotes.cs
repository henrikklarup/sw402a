using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace XMLawesome
{
    public class ChildNotes
    {
        public String tag;
        public String value;

        public ChildNotes(String Tag, String Value)
        {
            this.tag = Tag;
            this.value = Value;
        }
    }
}
