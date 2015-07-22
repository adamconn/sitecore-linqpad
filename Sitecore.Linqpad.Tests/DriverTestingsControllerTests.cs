using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.Linqpad.Controllers;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class DriverTestingsControllerTests
    {
        [TestMethod, TestCategory("DriverSettingsController class"), ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullView()
        {
            var controller = new DriverSettingsController(null);
        }
        [TestMethod, TestCategory("DriverSettingsController class")]
        public void ConstructorWithEmptyView()
        {
            var mockView = new Mock<ISitecoreConnectionSettings>();
            mockView.SetupGet(view => view.ClientUrl).Returns((string)null);
            mockView.SetupGet(view => view.Username).Returns((string)null);
            mockView.SetupGet(view => view.Password).Returns((string)null);
            mockView.SetupGet(view => view.WebRootPath).Returns((string)null);
            mockView.SetupGet(view => view.ContextDatabaseName).Returns((string)null);
            mockView.SetupGet(view => view.NamespacesToAdd).Returns((HashSet<string>)null);
            mockView.SetupGet(view => view.SearchResultType).Returns((SelectedType)null);
            mockView.SetupGet(view => view.AppConfigReaderType).Returns((SelectedType)null);
            mockView.SetupGet(view => view.SchemaBuilderType).Returns((SelectedType)null);
            mockView.SetupGet(view => view.DriverInitializerType).Returns((SelectedType)null);
            var controller = new DriverSettingsController(mockView.Object);
            Assert.AreEqual(mockView.Object, controller.View);
        }
    }
}
