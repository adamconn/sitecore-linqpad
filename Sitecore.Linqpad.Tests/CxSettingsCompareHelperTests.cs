using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class CxSettingsCompareHelperTests
    {
        [TestMethod, TestCategory("CxSettingsCompareHelper class")]
        public void GetCurrent()
        {
            Assert.IsNotNull(CxSettingsCompareHelper.Current);
            CxSettingsCompareHelper.Current = null;
            Assert.IsNotNull(CxSettingsCompareHelper.Current);
        }
        [TestMethod, TestCategory("CxSettingsCompareHelper class")]
        public void SetCurrent()
        {
            var obj = new CxSettingsCompareHelper();
            Assert.AreNotEqual(obj, CxSettingsCompareHelper.Current);
            CxSettingsCompareHelper.Current = obj;
            Assert.AreEqual(obj, CxSettingsCompareHelper.Current);
        }
        [TestMethod, TestCategory("CxSettingsCompareHelper class")]
        public void AreEqualMethod()
        {
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(null, null));
            var cxSettings1 = new SitecoreConnectionSettings();
            var cxSettings2 = new SitecoreConnectionSettings();
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings1.ClientUrl = "aaa";
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.ClientUrl = "aaa";
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            
            cxSettings1.Username = "bbb";
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.Username = "bbb";
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            
            cxSettings1.Password = "ccc";
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.Password = "ccc";
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            
            cxSettings1.WebRootPath = "ddd";
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.WebRootPath = "ddd";
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            
            cxSettings1.ContextDatabaseName = "eee";
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.ContextDatabaseName = "eee";
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            
            cxSettings1.NamespacesToAdd = new HashSet<string>() {"fff", "ggg"};
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.NamespacesToAdd = new HashSet<string>() { "fff", "ggg" };
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));

            cxSettings1.SearchResultType = new SelectedType(typeof(string));
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.SearchResultType = new SelectedType(typeof(string));
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));

            cxSettings1.AppConfigReaderType = new SelectedType(typeof(string));
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.AppConfigReaderType = new SelectedType(typeof(string));
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));

            cxSettings1.SchemaBuilderType = new SelectedType(typeof(string));
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.SchemaBuilderType = new SelectedType(typeof(string));
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));

            cxSettings1.DriverInitializerType = new SelectedType(typeof(string));
            Assert.IsFalse(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
            cxSettings2.DriverInitializerType = new SelectedType(typeof(string));
            Assert.IsTrue(CxSettingsCompareHelper.Current.AreEqual(cxSettings1, cxSettings2));
        }
    }
}
