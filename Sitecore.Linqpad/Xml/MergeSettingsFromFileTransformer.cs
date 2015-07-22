using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sitecore.Linqpad.Xml
{
    /// <summary>
    /// For example, this transformer can be used to merge the connection string settings from App_Config/ConnectionStrings.config
    /// </summary>
    public class MergeSettingsFromFileTransformer : IConfigTransformer
    {
        public MergeSettingsFromFileTransformer(string xpath, string attributeName, string filePath)
        {
            this.Xpath = xpath;
            this.AttributeName = attributeName;
            this.FilePath = filePath;
        }
        public string Xpath { get; private set; } 
        public string AttributeName { get; private set; }
        public string FilePath { get; private set; } 

        public virtual bool Transform(XDocument document)
        {
            if (document == null) { return false; }
            var element = document.XPathSelectElement(this.Xpath);
            if (element == null)
            {
                return false;
            }
            var attribute = element.Attribute(this.AttributeName);
            if (attribute == null)
            {
                return false;
            }
            var path = Path.Combine(this.FilePath, attribute.Value);
            if (!File.Exists(path))
            {
                throw new Exception(string.Format("Cannot find {0}", path));
            }
            var root = XDocument.Load(path).Root;
            if (!element.ToString().Equals(root.ToString()))
            {
                element.ReplaceWith(root);
                return true;
            }
            return false;
        }
    }
}
