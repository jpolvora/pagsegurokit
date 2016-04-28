using System;

namespace PagSeguroKit
{
    public class PagSeguroException : Exception
    {
        public PagSeguroException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}