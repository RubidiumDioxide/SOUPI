using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOUPI.Exceptions
{
    public class SoupiException : Exception
    {
        public SoupiException()
        {
        }

        public SoupiException(string message)
            : base(message)
        {
        }

        public SoupiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
