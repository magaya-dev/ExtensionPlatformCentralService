using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Application.Exception
{
    public class ExistsExtensionCompanyException : BaseException
    {
        public ExistsExtensionCompanyException() : base("Extension Company data already exist")
        {
        }
    }
}
