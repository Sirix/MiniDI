using System;
using System.Runtime.Serialization;
using System.Text;

namespace MiniDI
{
    [Serializable]
    public class ResolveException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ResolveException()
        {
        }
        public ResolveException(string messageFormat, params object[] args)
            : this(string.Format(messageFormat, args))
        {
        }

        public ResolveException(string message) : base(message)
        {
        }

        public ResolveException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ResolveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}