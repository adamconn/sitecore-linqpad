using System;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Server;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class SitecoreConnectionManagerTests
    {
        [TestMethod, TestCategory("SitecoreConnectionManager class"), ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullCxSettings()
        {
            var manager = new SitecoreConnectionManager(null);
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class"), ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorEmptyCxSettings()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class"), ExpectedException(typeof(UriFormatException))]
        public void ConstructorClientUrlMalformed()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("xxx");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void ConstructorClientUrlHttp()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void ConstructorClientUrlHttps()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("https://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
        }
        
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetVersionClientUrlHostNotFound()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://localhost/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreVersion();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any());
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetVersionSitecore80()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreVersion();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(8,response.Data.Major);
            Assert.AreEqual(0, response.Data.Minor);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsFalse(response.GetExceptions().Any());
        }

        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigClientUrlHostNotFound()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://localhost/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(WebException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigUsernameMissing()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns((string)null);
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigUsernameIncorrect()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns("xxxx");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigPasswordMissing()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns((string)null);
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigSitecore80()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsFalse(response.GetExceptions().Any());
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void GetSitecoreConfigPasswordIncorrect()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("xxxx");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.GetSitecoreConfig();
            Assert.IsNotNull(response);
            Assert.IsNull(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }

        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionClientUrlHostNotFound()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://localhost/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(WebException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionUsernameMissing()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns((string)null);
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionUsernameIncorrect()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns("xxxx");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionPasswordMissing()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns((string)null);
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionPasswordIncorrect()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("xxxx");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsTrue(response.GetExceptions().Any(ex => ex.GetType() == typeof(AuthenticationException)));
        }
        [TestMethod, TestCategory("SitecoreConnectionManager class")]
        public void TestConnectionSitecore80()
        {
            var mockCxSettings = new Mock<ISitecoreConnectionSettings>();
            mockCxSettings.SetupGet(cxSettings => cxSettings.Username).Returns(@"sitecore\admin");
            mockCxSettings.SetupGet(cxSettings => cxSettings.Password).Returns("b");
            mockCxSettings.SetupGet(cxSettings => cxSettings.ClientUrl).Returns("http://sc80/sitecore");
            var manager = new SitecoreConnectionManager(mockCxSettings.Object);
            var response = manager.TestConnection();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Data);
            Assert.IsNotNull(response.GetExceptions());
            Assert.IsFalse(response.GetExceptions().Any());
        }
    }
}
