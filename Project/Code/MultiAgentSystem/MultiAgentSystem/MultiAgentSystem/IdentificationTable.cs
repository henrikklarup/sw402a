using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Attributes
    {
        public int scope;
        public string ident;
        public int kind;
    }

    class IdentificationTable
    {
        public List<Attributes> identificationTable;
        private int scope;

        public IdentificationTable()
        {
            identificationTable = new List<Attributes>();
            scope = 0;
        }

        public void enter(int kind, string ident)
        {
            Attributes attr = new Attributes();

            attr.scope = scope;
            attr.kind = kind;
            attr.ident = ident;

            identificationTable.Add(attr);
        }

        public int retrieve(string ident)
        {
            List<Attributes> attributes;
            Attributes attr;

            attributes = identificationTable.FindAll(
                delegate(Attributes att)
                {
                    return att.ident == ident;
                });
            attr = attributes.Find(
                delegate(Attributes att)
                {
                    return att.scope <= scope;
                });
            if (attr != null) return attr.kind;
            else Console.WriteLine("{0} has not been declared.", token.spelling);

            return (int)Token.keywords.ERROR;
        }

        public void openScope()
        {
            scope++;
        }

        public void closeScope()
        {
            scope--;
        }
    }
}
