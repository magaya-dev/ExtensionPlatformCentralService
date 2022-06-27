using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsPlatform.Application.ExtensionCompany.Dto
{
    public class ExtCompanyVersionInfo
    {
        public string NetworkId { get; set; }
        public string ExtensionName { get; set; }

        public string CurrentVersion { get; set; }
        public bool UpdateAvailable { get; set; }
    }
}
