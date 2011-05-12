using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MASSIVE
{
    [Serializable]
    class GrammarException : System.Exception
    {
        private string _failedString;
        private Token _failedToken;
        public List<GrammarException> containedExceptions = new List<GrammarException>();

        public GrammarException()
        { }

        public GrammarException(string message)
            : base(message)
        { }

        public GrammarException(Token token)
        {
            this._failedToken = token;
        }

        public GrammarException(string message, Token token)
            : base(message)
        {
            this._failedToken = token;
        }

        public GrammarException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public GrammarException(string message, Exception innerException, Token token)
            : base(message, innerException)
        {
            this._failedToken = token;
        }

        protected GrammarException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                this._failedString = info.GetString("_failedString");
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("_failedString", this._failedString);
            }
        }

        public string FailedString
        {
            get { return this._failedString; }
            set { this._failedString = value; }
        }

        public Token FailedToken
        {
            get { return this._failedToken; }
            set { this._failedToken = value; }
        }

        public void PrintExceptions()
        {
            Console.WriteLine("\n" + this.Message);
            foreach (GrammarException exc in this.containedExceptions)
            {
                Printer.Error(exc.Message + "\n");
            }
            Console.WriteLine();
        }
    }
}
