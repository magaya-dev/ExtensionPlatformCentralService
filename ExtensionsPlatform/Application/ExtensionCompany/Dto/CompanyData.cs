using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExtensionsPlatform.Application.ExtensionCompany.Dto
{
    public class ExtensionCompanyData
    {
        [Required]
        public string NetworkId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string ExtensionName { get; set; }
        
        public string ExtId { get; set; }
    }

    public class UpdateExtensionCompanyData
    {
        [Required]
        public string NetworkId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string ExtensionName { get; set; }
        [Required]
        public string Version { get; set; }
    }
}
