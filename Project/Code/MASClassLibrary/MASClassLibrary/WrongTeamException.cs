using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MASClassLibrary
{
        [Serializable]
        // public, because it can't be catched in the ActionInterpeter, if its private.
        public class WrongTeamException : System.Exception
        {
            private string _failedString;
            public List<WrongTeamException> containedExceptions = new List<WrongTeamException>();

            public WrongTeamException()
            { }

            public WrongTeamException(string message)
                : base(message)
            { }

            public WrongTeamException(string message, Exception innerException)
                : base(message, innerException)
            { }

            protected WrongTeamException(SerializationInfo info, StreamingContext context)
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

            public List<string> PrintExceptions()
            {
                List<string> output = new List<string>();
                output.Add(this.Message);
                foreach (WrongTeamException exc in this.containedExceptions)
                {
                    output.Add(exc.Message);
                }

                return output;
            }
        }
}
