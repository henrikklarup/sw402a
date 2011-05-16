using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASSIVE
{
    // Class used to hold all the data, stored about the identifier.
    // Which scope it was declared in, how its recognized, and which kind it is.
    public class Attributes
    {
        public int scope;
        public string ident;
        public int kind;
        public bool used;
        public int line;
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
            attr.used = false;

            identificationTable.Add(attr);
        }

        public static void enter(int kind, string ident, int line)
        {
            Attributes attr = new Attributes();

            attr.scope = scope;
            attr.kind = kind;
            attr.ident = ident;
            attr.used = false;
            attr.line = line;

            identificationTable.Add(attr);
        }

        // Method to search for an identifier in the identification table, returns an error if it doesn't exists.
        public static int retrieve(string ident)
        {
            Attributes attr;

            // Find the first identifier in the identification table,
            // which matches the identifier.
            attr = identificationTable.Find(
                delegate(Attributes att)
                {
                    return att.ident == ident;
                });
            if (attr != null) return attr.kind;
            return (int)Token.keywords.ERROR;
        }

        public static void use(string ident)
        {
            foreach (Attributes a in identificationTable)
            {
                if (a.ident == ident)
                {
                    a.used = true;
                    break;
                }
            }
        }

        // When a new scope is identified, count the scopecounter 1 up.
        public static void openScope()
        {
            scope++;
        }

        // When a scope has ended, count the scope counter 1 down, 
        // and delete all items in the identification table which were in that scope.
        public static void closeScope()
        {
            scope--;
            identificationTable.RemoveAll(item => item.scope > scope);
        }

        public static List<GrammarException> varCloseScope()
        {
            scope--;
            List<GrammarException> gExceptions = new List<GrammarException>();
            foreach (Attributes a in identificationTable)
            {
                if (a.scope > scope && !a.used)
                {
                    gExceptions.Add(new GrammarException("(Line " + a.line + ") Variable " + a.ident + 
                        " was declared, but never used.", false));
                }
            }
            identificationTable.RemoveAll(item => item.scope > scope);
            return gExceptions;
        }
    }
}
