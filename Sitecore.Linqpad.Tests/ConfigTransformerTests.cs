using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Linqpad.Xml;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class ConfigTransformerTests
    {
        public TestContext TestContext { get; set; } //this needs to be named TestContext

        [TestMethod, TestCategory("IConfigTransformer types")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\RemoveElementTransformerData.xml", "element", DataAccessMethod.Sequential)]
        public void RemoveElementFirst()
        {
            var xpath = (string)this.TestContext.DataRow["xpath"];
            var xml = (string)this.TestContext.DataRow["xml"];
            var document = XDocument.Parse(xml);
            var countBefore = document.XPathSelectElements(xpath).Count();
            var transformer = new RemoveElementTransformer(xpath, false);
            transformer.Transform(document);
            var countAfterExpected = (countBefore > 0) ? --countBefore : countBefore;
            Assert.AreEqual(countAfterExpected, document.XPathSelectElements(xpath).Count());
        }
        [TestMethod, TestCategory("IConfigTransformer types")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\RemoveElementTransformerData.xml", "element", DataAccessMethod.Sequential)]
        public void RemoveElementAll()
        {
            var xpath = (string)this.TestContext.DataRow["xpath"];
            var xml = (string)this.TestContext.DataRow["xml"];
            var document = XDocument.Parse(xml);
            var countBefore = document.XPathSelectElements(xpath).Count();
            var transformer = new RemoveElementTransformer(xpath, true);
            transformer.Transform(document);
            Assert.AreEqual(0, document.XPathSelectElements(xpath).Count());
        }
        [TestMethod, TestCategory("IConfigTransformer types")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\AttributeValueTransformerData.xml", "element", DataAccessMethod.Sequential)]
        public void AttributeValueReplace()
        {
            var xpath = (string)this.TestContext.DataRow["xpath"];
            var xml = (string)this.TestContext.DataRow["xml"];
            var attributeName = (string)this.TestContext.DataRow["attribute"];
            var newValue = (string)this.TestContext.DataRow["value"];
            var document = XDocument.Parse(xml);

            var elements = document.XPathSelectElements(xpath);
            var transformer = new AttributeValueTransformer(xpath, attributeName, newValue, AttributeValueChangeType.Replace);
            transformer.Transform(document);
            var anyElementsUnchanged = elements.Where(e => e.Attribute(attributeName) != null)
                .Any(e => ! string.Equals(e.Attribute(attributeName).Value, newValue));
            Assert.IsFalse(anyElementsUnchanged);
        }
        [TestMethod, TestCategory("IConfigTransformer types")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\AttributeValueTransformerData.xml", "element", DataAccessMethod.Sequential)]
        public void AttributeValuePrepend()
        {
            var xpath = (string)this.TestContext.DataRow["xpath"];
            var xml = (string)this.TestContext.DataRow["xml"];
            var attributeName = (string)this.TestContext.DataRow["attribute"];
            var newValue = (string)this.TestContext.DataRow["value"];
            var document = XDocument.Parse(xml);

            var elements = document.XPathSelectElements(xpath);
            var transformer = new AttributeValueTransformer(xpath, attributeName, newValue, AttributeValueChangeType.Replace);
            transformer.Transform(document);
            var anyElementsUnchanged = elements.Where(e => e.Attribute(attributeName) != null)
                .Any(e => ! e.Attribute(attributeName).Value.StartsWith(newValue));
            Assert.IsFalse(anyElementsUnchanged);
        }
        [TestMethod, TestCategory("IConfigTransformer types")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Data\\AttributeValueTransformerData.xml", "element", DataAccessMethod.Sequential)]
        public void AttributeValueAppend()
        {
            var xpath = (string)this.TestContext.DataRow["xpath"];
            var xml = (string)this.TestContext.DataRow["xml"];
            var attributeName = (string)this.TestContext.DataRow["attribute"];
            var newValue = (string)this.TestContext.DataRow["value"];
            var document = XDocument.Parse(xml);

            var elements = document.XPathSelectElements(xpath);
            var transformer = new AttributeValueTransformer(xpath, attributeName, newValue, AttributeValueChangeType.Replace);
            transformer.Transform(document);
            var anyElementsUnchanged = elements.Where(e => e.Attribute(attributeName) != null)
                .Any(e => !e.Attribute(attributeName).Value.EndsWith(newValue));
            Assert.IsFalse(anyElementsUnchanged);
        }
    
    }
}
