using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public interface ISitecoreConnectionSettings
    {
        string ClientUrl { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string WebRootPath { get; set; }
        string ContextDatabaseName { get; set; }
        //
        //
        HashSet<string> NamespacesToAdd { get; set; }
        ISelectedType SearchResultType { get; set; }
        ISelectedType AppConfigReaderType { get; set; }
        ISelectedType SchemaBuilderType { get; set; }
        ISelectedType DriverInitializerType { get; set; }
    }
}
