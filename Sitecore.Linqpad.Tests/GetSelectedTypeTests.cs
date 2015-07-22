using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class GetSelectedTypeTests
    {
        [TestMethod, TestCategory("SelectedType class")]
        public void DefaultConstructor()
        {
            var model = new SelectedType();
            Assert.IsNull(model.GetSelectedType());
        }
        [TestMethod, TestCategory("SelectedType class")]
        public void NullTypeConstructor()
        {
            var model = new SelectedType((Type)null);
            Assert.IsNull(model.GetSelectedType());
        }
        [TestMethod, TestCategory("SelectedType class")]
        public void SpecifiedTypeConstructor()
        {
            var model = new SelectedType(typeof(string));
            Assert.AreSame(model.GetSelectedType(), typeof(string));
        }
        [TestMethod, TestCategory("SelectedType class")]
        public void TypeNameFromGacConstructor()
        {
            var model = new SelectedType(typeof(string));
            Assert.AreSame(typeof(string), model.GetSelectedType());
        }
        [TestMethod, TestCategory("SelectedType class")]
        public void NonExistentTypeNameConstructor()
        {
            var model = new SelectedType((Type)null);
            var type = model.GetSelectedType();
            Assert.IsNull(type);
        }
    }
}
