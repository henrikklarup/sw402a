using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASSIVE
{
    public class Type
    {
        public int kind;    // Which kind (e.g. Strings in an expression)
        public int type;    // Which type (e.g. Numeric expression)

        public enum types
        { 
            NULL,
            NUM,
            BOOL,
            STRING,
            AGENT,
            TEAM,
            SQUAD,
            ACTIONPATTERN,
        }

        /// <summary>
        /// Creates a new type with, kind declared.
        /// </summary>
        /// <param name="kind">Denote the kind</param>
        public Type(Type.types kind)
        {
            this.kind = (int)kind;   
        }

        /// <summary>
        /// Creates a new type with, kind and type declared.
        /// </summary>
        /// <param name="kind">Denotes the kind</param>
        /// <param name="type">Denotes the type</param>
        public Type(Type.types kind, Type.types type)
            : this(kind)
        {
            this.type = (int)type;
        }
    }
}
