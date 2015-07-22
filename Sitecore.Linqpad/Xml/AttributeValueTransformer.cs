using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sitecore.Linqpad.Xml
{
    public class AttributeValueChangeType
    {
        public static readonly AttributeValueChangeType Replace = new AttributeValueChangeType();
        public static readonly AttributeValueChangeType Append = new AttributeValueChangeType();
        public static readonly AttributeValueChangeType Prepend = new AttributeValueChangeType();
    }
    public class AttributeValueTransformer : IConfigTransformer
    {
        public AttributeValueTransformer(string xpath, string attributeName, string newValue, AttributeValueChangeType changeType = null)
        {
            this.Xpath = xpath;
            this.AttributeName = attributeName;
            this.NewValue = newValue;
            this.ChangeType = changeType ?? AttributeValueChangeType.Replace;
        }
        public string Xpath { get; private set; }
        public string AttributeName { get; private set; }
        public string NewValue { get; private set; }
        public AttributeValueChangeType ChangeType { get; private set; }

        public virtual bool Transform(XDocument document)
        {
            if (document == null) { return false; }
            var elementArray = document.XPathSelectElements(this.Xpath).ToArray<XElement>();
            if (elementArray.Length == 0) { return false; }
            var changeMade = false;
            foreach (var element in elementArray)
            {
                var attribute = element.Attribute(this.AttributeName);
                if (attribute == null) { continue; }
                string newValue = null;
                if (this.ChangeType == AttributeValueChangeType.Replace)
                {
                    newValue = this.NewValue;
                }
                else if (this.ChangeType == AttributeValueChangeType.Prepend)
                {
                    newValue = this.NewValue + attribute.Value;
                }
                else if (this.ChangeType == AttributeValueChangeType.Append)
                {
                    newValue = attribute.Value + this.NewValue;
                }
                if (!string.IsNullOrEmpty(newValue) && ! newValue.Equals(attribute.Value))
                {
                    attribute.Value = newValue;
                    changeMade = true;
                }
            }
            return changeMade;
        }
    }
}
