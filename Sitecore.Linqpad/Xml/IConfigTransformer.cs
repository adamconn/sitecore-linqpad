using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sitecore.Linqpad.Xml
{
    public interface IConfigTransformer
    {
        bool Transform(XDocument document);
    }
}
