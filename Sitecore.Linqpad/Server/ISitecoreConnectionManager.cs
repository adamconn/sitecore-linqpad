using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sitecore.Linqpad.Server
{
    public interface ISitecoreConnectionManager
    {
        ServerResponse<XElement> GetSitecoreConfig();
        ServerResponse<Version> GetSitecoreVersion();
        ServerResponse<bool> TestConnection();
    }
}
