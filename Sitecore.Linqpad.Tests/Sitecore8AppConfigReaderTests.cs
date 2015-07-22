using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.Linqpad.AppConfigReaders;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Server;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class Sitecore8AppConfigReaderTests
    {
        public TestContext TestContext { get; set; } //this needs to be named TestContext

        [TestMethod, TestCategory("Sitecore8AppConfigReader class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\Sitecore8AppConfigReaderData.xml", "version", DataAccessMethod.Sequential)]
        public void IsSupportedVersion()
        {
            var version = new Version((string) this.TestContext.DataRow["number"]);
            var isSupported = Boolean.Parse((string)this.TestContext.DataRow["supported"]);
            var reader = new Sitecore8AppConfigReader();
            Assert.AreEqual(isSupported, reader.IsSupportedVersion(version));
        }

        [TestMethod, TestCategory("Sitecore8AppConfigReader class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\Sitecore8AppConfigReaderData.xml", "webConFig", DataAccessMethod.Sequential)]
        public void Read()
        {
            var tmpCxStringsFile = Path.GetTempFileName();
            var tmpCxStringsDoc = XDocument.Parse("<aaa></aaa>");
            tmpCxStringsDoc.Save(tmpCxStringsFile);

            var tmpWebConfigFile = Path.GetTempFileName();
            var tmpWebConfigDoc = XDocument.Parse((string) this.TestContext.DataRow["fileSystem"]);
            var csElement = tmpWebConfigDoc.XPathSelectElement("/configuration/connectionStrings");
            var attr = csElement.Attribute("configSource");
            if (attr != null)
            {
                attr.Value = tmpCxStringsFile;
            }
            tmpWebConfigDoc.Save(tmpWebConfigFile);

            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.WebRootPath).Returns(Path.GetDirectoryName(tmpWebConfigFile));

            var driverSettings = new SitecoreDriverSettings() {CxSettings = mockCxSettings.Object};
            var mockManager = new Mock<ISitecoreConnectionManager>();
            var response = new ServerResponse<XElement>();
            response.Data = XElement.Parse((string)this.TestContext.DataRow["server"]);
            mockManager.Setup(m => m.GetSitecoreConfig()).Returns(response);
            var reader = new Sitecore8AppConfigReader();
            var doc = reader.Read(mockManager.Object, driverSettings, tmpWebConfigFile, false);
            //
            //Test by re-running the transformers. No changes should be reported.
            var transformers = reader.GetTransformers(driverSettings);
            foreach (var transformer in transformers)
            {
                Assert.IsFalse(transformer.Transform(doc));
            }
        }
    }
}
