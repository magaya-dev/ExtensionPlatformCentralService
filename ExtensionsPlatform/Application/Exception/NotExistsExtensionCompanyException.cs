using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Application.Exception
{
    public class NotExistsExtensionCompanyException : BaseException
    {
        public NotExistsExtensionCompanyException() : base("Extension Company data does not exist")
        {
        }
    }
}
