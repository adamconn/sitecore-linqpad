using System;
using System.Collections.Generic;
using LINQPad.Extensibility.DataContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.Linqpad.Controllers;
using Sitecore.Linqpad.Dialogs;
using Sitecore.Linqpad.Models;
using System.Xml.Linq;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class ConnectionDialogTests
    {
        public TestContext TestContext { get; set; } //this needs to be named TestContext

        [TestMethod, TestCategory("UI - connection dialog")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "requiredSettings", DataAccessMethod.Sequential)]
        public void SetCxSettingsNull()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);

            var view = new ConnectionDialog();
            view.InitializeComponent();
            var driverSettings = new SitecoreDriverSettings() { CxInfo = mockCxInfo.Object, CxSettings = cxSettings, SettingsMapper = new DriverDataCxSettingsMapper() };
            view.Model = driverSettings;
            var controller = new DriverSettingsController(view);
            controller.LoadView(driverSettings);
            view.SaveViewToModelCallback = controller.SaveView;
            //
            //basic settings
            view.ClientUrl = null;
            view.Username = null;
            view.Password = null;
            view.WebRootPath = null;
            view.ContextDatabaseName = null;
            //
            //advanced settings
            view.NamespacesToAdd = null;
            view.SearchResultType = null;
            view.AppConfigReaderType = null;
            view.SchemaBuilderType = null;
            view.DriverInitializerType = null;
        }

        [TestMethod, TestCategory("UI - connection dialog")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "requiredSettings", DataAccessMethod.Sequential)]
        public void SetCxSettingsWithEmptyObjectsForAdvancedSettings()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);

            var view = new ConnectionDialog();
            view.InitializeComponent();
            var driverSettings = new SitecoreDriverSettings() { CxInfo = mockCxInfo.Object, CxSettings = cxSettings, SettingsMapper = new DriverDataCxSettingsMapper() };
            view.Model = driverSettings;
            var controller = new DriverSettingsController(view);
            controller.LoadView(driverSettings);
            view.SaveViewToModelCallback = controller.SaveView;
            //
            //basic settings
            view.ClientUrl = "http://localhost";
            view.Username = "username";
            view.Password = "password";
            view.WebRootPath = @"C:\Windows\Temp";
            view.ContextDatabaseName = "master";
            //
            //advanced settings
            view.NamespacesToAdd = new HashSet<string>();
            view.SearchResultType = new SelectedType();
            view.AppConfigReaderType = new SelectedType();
            view.SchemaBuilderType = new SelectedType();
            view.DriverInitializerType = new SelectedType();
        }

    }
}
