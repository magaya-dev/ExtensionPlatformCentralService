using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Application.Exception
{
    public abstract class BaseException : System.Exception
    {

        public BaseException(string message) : base(message)
        {

        }
    }
}
