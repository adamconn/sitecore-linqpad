using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Sitecore.Linqpad.Xml;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Server;

namespace Sitecore.Linqpad.AppConfigReaders
{
    public class Sitecore7AppConfigReader : IAppConfigReader
    {
        public virtual bool IsSupportedVersion(Version version)
        {
            if (version == null) { return false;}
            var min = new Version("7.0");
            var max = new Version("8.0");
            return (version.CompareTo(min) != -1 && version.CompareTo(max) == -1);
        }
        public virtual IEnumerable<IConfigTransformer> GetTransformers(SitecoreDriverSettings driverSettings)
        {
            var transformers = new List<IConfigTransformer>()
                {
                    new MergeSettingsFromFileTransformer("/configuration/connectionStrings", "configSource", driverSettings.CxSettings.WebRootPath),
                    new RemoveElementTransformer("/configuration/sitecore/pipelines/indexing.filterIndex.outbound//processor[@type='Sitecore.ContentSearch.Pipelines.IndexingFilters.ApplyOutboundSecurityFilter, Sitecore.ContentSearch']", false),
                    new RemoveElementTransformer("/configuration/log4net//encoding", true),
                    new AttributeValueTransformer("/configuration/sitecore/configStores/*[starts-with(@arg0, '/App_Config/Security/')]", "arg0", driverSettings.CxSettings.WebRootPath, AttributeValueChangeType.Prepend)
                };
            return transformers;
        }

        #region Read
        public virtual XDocument Read(ISitecoreConnectionManager cxManager, SitecoreDriverSettings driverSettings, string fileName, bool save)
        {
            if (cxManager == null) { throw new ArgumentNullException("cxManager"); }
            if (driverSettings == null) { throw new ArgumentNullException("driverSettings"); }
            if (driverSettings.CxSettings == null) { throw new NullReferenceException("CxSettings cannot be null"); }
            var config = this.ReadConfigFromSitecore(cxManager, driverSettings, fileName);
            this.Transform(config, driverSettings);
            if (save)
            {
                this.Save(config, driverSettings);
            }
            return config;
        }
        protected virtual XDocument ReadConfigFromSitecore(ISitecoreConnectionManager cxManager, SitecoreDriverSettings driverSettings, string fileName)
        {
            var fsConfig = this.ReadFileFromFileSystem(driverSettings, fileName);
            var response = cxManager.GetSitecoreConfig();
            if (response == null)
            {
                throw new Exception("Connection manager returned a null response.");
            }
            var webScRootElement = response.Data;
            if (webScRootElement == null)
            {
                throw new Exception("Connection manager returned a null Sitecore config.");
            }
            var fsScRootElement = fsConfig.XPathSelectElement("/configuration/sitecore");
            if (fsScRootElement == null)
            {
                throw new Exception("Unable to locate the element /configuration/sitecore in web.config");
            }
            fsScRootElement.ReplaceWith(webScRootElement);
            return fsConfig;
        }
        protected virtual XDocument ReadFileFromFileSystem(SitecoreDriverSettings driverSettings, string fileName)
        {
            var webRootPath = driverSettings.CxSettings.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                throw new Exception("No webroot path was specified, or the specified path is invalid");
            }
            var path = Path.Combine(webRootPath, fileName);
            if (!File.Exists(path))
            {
                throw new Exception(string.Format("The {0} file could not be located at {1}", fileName, path));
            }
            return XDocument.Load(path);
        }
        protected virtual void Transform(XDocument document, SitecoreDriverSettings driverSettings)
        {
            if (document == null) { return; }
            var transformers = GetTransformers(driverSettings);
            if (transformers == null) { return; }
            foreach (var transformer in transformers)
            {
                transformer.Transform(document);
            }
        }
        #endregion

        #region Save
        public virtual void Save(XDocument document, SitecoreDriverSettings driverSettings)
        {
            if (driverSettings == null) { throw new ArgumentNullException("driverSettings"); }
            var cxInfo = driverSettings.CxInfo;
            if (cxInfo == null) { throw new NullReferenceException("CxInfo cannot be null"); }
            if (!File.Exists(cxInfo.AppConfigPath))
            {
                cxInfo.AppConfigPath = Path.GetTempFileName();
            }
            document.Save(cxInfo.AppConfigPath);
        }
        #endregion
    }
}
