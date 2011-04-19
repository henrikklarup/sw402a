using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    // Class used to hold all the data, stored about the identifier.
    // Which scope it was declared in, how its recognized, and which kind it is.
    public class Attributes
    {
        public int scope;
        public string ident;
        public int kind;
    }

    // The IdentificationTable class which holds all the declared identifiers.
    public static class IdentificationTable
    {
        private static List<Attributes> identificationTable = new List<Attributes>();
        private static int scope = 0;

        // Method to insert an identifier in the identification table.
        public static void enter(int kind, string ident)
        {
            Attributes attr = new Attributes();

            attr.scope = scope;
            attr.kind = kind;
            attr.ident = ident;

            identificationTable.Add(attr);
        }

        // 
        public static int retrieve(Token token)
        {
            Attributes attr;

            string ident = token.spelling;

            attr = identificationTable.Find(
                delegate(Attributes att)
                {
                    return att.ident == ident;
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
            identificationTable.RemoveAll(item => item.scope > scope);
        }
    }
}
