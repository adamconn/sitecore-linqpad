using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class DefaultValuesTests
    {
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void DoesValueMatchListAllParametersNull()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList(null, null, null);
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void DoesValueMatchListNullStringAndEmptyList()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList(null, null, new HashSet<string>());
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void EmptyStringAndEmptyList()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList(string.Empty, null, new HashSet<string>());
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void EmptyStringAndNullList()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList(string.Empty, null, null);
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithNoDelimiter1()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a", null, new HashSet<string>() { "a" });
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithDelimiter()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a", "|", new HashSet<string>() { "a" });
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithNoDelimiter2()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b", null, new HashSet<string>() { "a", "b" });
            Assert.IsFalse(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithWrongDelimiter()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b", ",", new HashSet<string>() { "a", "b" });
            Assert.IsFalse(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithCorrectDelimiter1()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b", "|", new HashSet<string>() { "a", "b" });
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListWithCorrectDelimiter2()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a\nb", "\n", new HashSet<string>() { "a", "b" });
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueStringAndMatchingListInDifferentOrder()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b|c", "|", new HashSet<string>() { "b", "a", "c" });
            Assert.IsTrue(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueContainsListMinus1()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b|c", "|", new HashSet<string>() { "b", "a", "c", "d" });
            Assert.IsFalse(result);
        }
        [TestMethod, TestCategory("DefaultValuesForCxSettings class")]
        public void ValueContainsListPlus1()
        {
            var result = DefaultValuesForCxSettings.Current.DoesValueMatchList("a|b|c|d", "|", new HashSet<string>() { "b", "a", "c" });
            Assert.IsFalse(result);
        }
    }
}
