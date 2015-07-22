using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.Driver;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class DriverTests
    {
        [TestMethod, TestCategory("SitecoreDriver class")]
        public void DriverAuthorIsAdamConn()
        {
            var driver = new SitecoreDriver();
            Assert.AreEqual("Adam Conn", driver.Author);
        }
        [TestMethod, TestCategory("SitecoreDriver class")]
        public void DriverNameIsSitecore()
        {
            var driver = new SitecoreDriver();
            Assert.AreEqual("Sitecore", driver.Name);
        }
    }
}
