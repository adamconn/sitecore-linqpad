using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Server;
using Sitecore.Linqpad.Xml;

namespace Sitecore.Linqpad.AppConfigReaders
{
    /// <summary>
    /// Reads the web.config file from the Sitecore server and 
    /// creates an appropriate app.config file for LINQPad.
    /// </summary>
    public interface IAppConfigReader
    {
        bool IsSupportedVersion(Version version);
        XDocument Read(ISitecoreConnectionManager cxManager, SitecoreDriverSettings driverSettings, string fileName, bool save);
        IEnumerable<IConfigTransformer> GetTransformers(SitecoreDriverSettings driverSettings); 
        void Save(XDocument document, SitecoreDriverSettings driverSettings);
    }
}
