using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public class SitecoreConnectionSettings : ISitecoreConnectionSettings
    {
        public string ClientUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string WebRootPath { get; set; }
        public string ContextDatabaseName { get; set; }
        public HashSet<string> NamespacesToAdd { get; set; }

        public ISelectedType SearchResultType { get; set; }
        public ISelectedType AppConfigReaderType { get; set; }
        public ISelectedType SchemaBuilderType { get; set; }
        public ISelectedType DriverInitializerType { get; set; }
    }
}
