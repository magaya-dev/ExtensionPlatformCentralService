using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExtensionsPlatform.Application.ExtensionActiveAction.Dto
{
    public class ExtensionActive
    {
        [Required]
        public string NetworkId { get; set; }
        [Required]
        public string ExtensionName { get; set; }
        [Required]
        public bool Active { get; set; }
    }

    public class RetrieveIfExtActive
    {
        [Required]
        public string NetworkId { get; set; }
        [Required]
        public string ExtensionName { get; set; }
    }
}
