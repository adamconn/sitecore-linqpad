using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sitecore.Linqpad.Xml
{
    public class RemoveElementTransformer : IConfigTransformer
    {
        public RemoveElementTransformer(string xpath, bool removeAll)
        {
            this.Xpath = xpath;
            this.RemoveAll = removeAll;
        }
        public string Xpath { get; private set; }
        public bool RemoveAll { get; private set; }
        public virtual bool Transform(XDocument document)
        {
            if (document == null) { return false; }
            var source = document.XPathSelectElements(this.Xpath).ToArray<XElement>();
            if (source.Length == 0)
            {
                return false;
            }
            var changeMade = false;
            if (this.RemoveAll)
            {
                source.Remove<XElement>();
                changeMade = true;
            }
            else
            {
                var element = source.FirstOrDefault<XElement>();
                if (element != null)
                {
                    element.Remove();
                    changeMade = true;
                }
            }
            return changeMade;
        }
    }
}
