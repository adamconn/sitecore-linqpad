using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQPad.Extensibility.DataContext;

namespace Sitecore.Linqpad.DriverInitializers
{
    public interface IDriverInitializer
    {
        void Run(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager);
    }
}
