using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Application.Exception
{
    public class NotExistsExtensionActiveException : BaseException
    {
        public NotExistsExtensionActiveException() : base("Extension data does not exist")
        {
        }
    }
}
