using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    public class TevianException : Exception
    {
        public TevianException()
        {
        }

        public TevianException(string message) : base(message)
        {
        }

        public TevianException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}
