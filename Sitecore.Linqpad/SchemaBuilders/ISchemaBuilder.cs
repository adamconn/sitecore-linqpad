using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LINQPad.Extensibility.DataContext;

namespace Sitecore.Linqpad.SchemaBuilders
{
    public interface ISchemaBuilder
    {
        List<ExplorerItem> BuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName);
    }
}
