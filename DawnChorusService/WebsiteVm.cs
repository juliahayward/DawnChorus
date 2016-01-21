using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DawnChorusService
{
    public class WebsiteVm
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int HitCount { get; set; }
        public int FilesInFTP { get; set; }
        public bool SupportsTwitter { get; set; }
    }
}
