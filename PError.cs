using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    /// <summary>
    /// Ett fel i programmet som ska skrivas ut till användaren.
    /// </summary>
    [Serializable]
    public class PError : Exception
    {
        public PError() { }
        public PError(string message) : base(message) { }
        public PError(string message, Exception inner) : base(message, inner) { }
        protected PError(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
