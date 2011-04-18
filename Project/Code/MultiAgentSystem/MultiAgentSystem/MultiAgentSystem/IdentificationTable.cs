using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    public class Attributes
    {
        public int scope;
        public string ident;
        public int kind;
    }

    public static class IdentificationTable
    {
        private static List<Attributes> identificationTable = new List<Attributes>();
        private static int scope = 0;

        public static void enter(int kind, string ident)
        {
            Attributes attr = new Attributes();

            attr.scope = scope;
            attr.kind = kind;
            attr.ident = ident;

            identificationTable.Add(attr);
        }

        public static int retrieve(Token token)
        {
            List<Attributes> attributes;
            Attributes attr;

            string ident = token.spelling;

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
            return (int)Token.keywords.ERROR;
        }

        public static void openScope()
        {
            scope++;
        }

        public static void closeScope()
        {
            scope--;
        }
    }
}
