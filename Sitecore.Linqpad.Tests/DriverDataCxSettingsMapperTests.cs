using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.DriverInitializers;
using Sitecore.Linqpad.Models;
using Moq;
using LINQPad.Extensibility.DataContext;
using Sitecore.Linqpad.AppConfigReaders;
using Sitecore.Linqpad.SchemaBuilders;
using System.Collections.Generic;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class DriverDataCxSettingsMapperTests
    {
        public TestContext TestContext { get; set; } //this needs to be named TestContext

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class"), ExpectedException(typeof(NullReferenceException))]
        public void ReadDriverDataNull()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
        }
        
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "empty", DataAccessMethod.Sequential)] 
        public void ReadDriverDataEmpty()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            Assert.IsNull(cxSettings.ClientUrl, "ClientUrl should be null");
            Assert.IsNull(cxSettings.Username, "Username should be null");
            Assert.IsNull(cxSettings.Password, "Password should be null");
            Assert.IsNull(cxSettings.WebRootPath, "WebRootPath should be null");
            Assert.AreEqual("master", cxSettings.ContextDatabaseName);

            var defaultValues = DefaultValuesForCxSettings.Current;
            var list1 = cxSettings.NamespacesToAdd;
            var list2 = defaultValues.NamespacesToAdd;
            Assert.IsTrue(((list1 == list2) || ((list1.Count == list2.Count) && !list1.Except(list2).Any())));

            Assert.AreEqual(cxSettings.SearchResultType.GetSelectedType(), defaultValues.SearchResultItemType);
            Assert.AreEqual(defaultValues.AppConfigReaderType, cxSettings.AppConfigReaderType.GetSelectedType());
            Assert.AreEqual(defaultValues.SchemaBuilderType, cxSettings.SchemaBuilderType.GetSelectedType());
            Assert.AreEqual(defaultValues.DriverInitializerType, cxSettings.DriverInitializerType.GetSelectedType());
        }

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "requiredSettings", DataAccessMethod.Sequential)]
        public void ReadSettingsRequired()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var clientUrl = (string)this.TestContext.DataRow["clientUrl"];
            var username = (string)this.TestContext.DataRow["username"];
            var password = (string)this.TestContext.DataRow["password"];
            var webRootPath = (string)this.TestContext.DataRow["webRootPath"];
            Assert.AreEqual(clientUrl, cxSettings.ClientUrl);
            Assert.AreEqual(username, cxSettings.Username);
            Assert.AreEqual(password, cxSettings.Password);
            Assert.AreEqual(webRootPath, cxSettings.WebRootPath);
        }

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "empty", DataAccessMethod.Sequential)]
        public void ReadSettingsOptional()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var defaultValues = DefaultValuesForCxSettings.Current;
            Assert.AreEqual(cxSettings.ContextDatabaseName, defaultValues.ContextDatabaseName, "context database name should not be '{0}'", cxSettings.ContextDatabaseName);
            Assert.IsTrue(cxSettings.NamespacesToAdd.SetEquals(defaultValues.NamespacesToAdd), "namespaces loaded by the mapper do not match the default namespaces");
            DoMatch(defaultValues.SearchResultItemType, cxSettings.SearchResultType);
            DoMatch(defaultValues.AppConfigReaderType, cxSettings.AppConfigReaderType);
            DoMatch(defaultValues.SchemaBuilderType, cxSettings.SchemaBuilderType);
            DoMatch(defaultValues.DriverInitializerType, cxSettings.DriverInitializerType);
        }
        private void DoMatch(Type expectedType, ISelectedType selectedType)
        {
            Assert.IsNotNull(selectedType);
            var actualType = selectedType.GetSelectedType();
            Assert.AreEqual(expectedType, actualType);
        }

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\DriverData.xml", "nsSettings", DataAccessMethod.Sequential)]
        public void ReadDriverDataNamespaces()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse((string)this.TestContext.DataRow["driverData"]);
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var count = Int32.Parse((string)this.TestContext.DataRow["count"]);
            Assert.AreEqual(count, cxSettings.NamespacesToAdd.Count);
        }

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadSearchResultTypeEmptyTag()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(@"<DriverData><searchResultType></searchResultType></DriverData>");
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            Assert.AreEqual(DefaultValuesForCxSettings.Current.SearchResultItemType, cxSettings.SearchResultType.GetSelectedType());
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class"), ExpectedException(typeof(TypeLoadException))]
        public void ReadSearchResultTypeNoWebRootPathSpecified()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' /></DriverData>", DriverDataCxSettingsMapper.KEY_SEARCH_RESULT_TYPE, "xxx"));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings); //will throw an exception because the default type cannot be resolved without a valid webRootPath
        }

        [TestMethod, TestCategory("DriverDataCxSettingsMapper class"), ExpectedException(typeof(TypeLoadException))]
        public void ReadSearchResultTypeNotFound()
        {
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData webRootPath='{0}'><{1} type='{2}' /></DriverData>", @"c:\Sitecore\sc80\Website", DriverDataCxSettingsMapper.KEY_SEARCH_RESULT_TYPE, "xxx"));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            Assert.AreEqual(DefaultValuesForCxSettings.Current.SearchResultItemType, cxSettings.SearchResultType.GetSelectedType());
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadSearchResultTypeInvalidType()
        {
            var type = typeof (string);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' /></DriverData>", DriverDataCxSettingsMapper.KEY_SEARCH_RESULT_TYPE, type.AssemblyQualifiedName));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.SearchResultType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsFalse(cxSettings.SearchResultType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadSearchResultType()
        {
            var type = this.GetType();
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData webRootPath='{0}'><{1} type='{2}' location='{3}' /></DriverData>", @"c:\Sitecore\sc80\Website", DriverDataCxSettingsMapper.KEY_SEARCH_RESULT_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.SearchResultType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsTrue(cxSettings.SearchResultType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadAppConfigReaderTypeInvalidType()
        {
            var type = typeof(string);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' location='{2}' /></DriverData>", DriverDataCxSettingsMapper.KEY_APP_CONFIG_READER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.AppConfigReaderType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsFalse(cxSettings.SearchResultType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadAppConfigReaderType()
        {
            var type = typeof(Sitecore7AppConfigReader);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData webRootPath='{0}'><{1} type='{2}' location='{3}' /></DriverData>", @"c:\Sitecore\sc80\Website" , DriverDataCxSettingsMapper.KEY_APP_CONFIG_READER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.AppConfigReaderType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsTrue(cxSettings.AppConfigReaderType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadSchemaBuilderTypeInvalidType()
        {
            var type = typeof(string);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' location='{2}' /></DriverData>", DriverDataCxSettingsMapper.KEY_SCHEMA_BUILDER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.SchemaBuilderType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsFalse(cxSettings.SchemaBuilderType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadSchemaBuilderType()
        {
            var type = typeof(SitecoreSchemaBuilder);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' location='{2}' /></DriverData>", DriverDataCxSettingsMapper.KEY_SCHEMA_BUILDER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.SchemaBuilderType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsTrue(cxSettings.SchemaBuilderType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadDriverInitializerTypeInvalidType()
        {
            var type = typeof(string);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' location='{2}' /></DriverData>", DriverDataCxSettingsMapper.KEY_DRIVER_INITIALIZER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.DriverInitializerType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsFalse(cxSettings.DriverInitializerType.IsValidType(selectedType));
        }
        [TestMethod, TestCategory("DriverDataCxSettingsMapper class")]
        public void ReadDriverInitializerType()
        {
            var type = typeof(SitecoreDriverInitializer);
            var mockCxInfo = new Mock<IConnectionInfo>();
            var element = XElement.Parse(string.Format(@"<DriverData><{0} type='{1}' location='{2}' /></DriverData>", DriverDataCxSettingsMapper.KEY_DRIVER_INITIALIZER_TYPE, type.AssemblyQualifiedName, type.Assembly.Location));
            mockCxInfo.SetupGet(cxInfo => cxInfo.DriverData).Returns(element);
            var cxSettings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(mockCxInfo.Object, cxSettings);
            var selectedType = cxSettings.DriverInitializerType.GetSelectedType();
            Assert.AreEqual(type, selectedType);
            Assert.IsTrue(cxSettings.DriverInitializerType.IsValidType(selectedType));
        }    
    }
}
