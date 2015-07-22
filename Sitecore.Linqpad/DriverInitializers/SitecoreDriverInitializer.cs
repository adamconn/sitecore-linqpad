using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQPad.Extensibility.DataContext;

namespace Sitecore.Linqpad.DriverInitializers
{
    public class SitecoreDriverInitializer : IDriverInitializer
    {
        protected virtual IEnumerable<MethodRunner> GetMethodRunners(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            var runners = new List<MethodRunner>();
            runners.Add(new MethodRunner("Sitecore.ContentSearch.Hooks.Initializer, Sitecore.ContentSearch", "Initialize"));
            return runners;
        }
        public virtual void Run(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            var runners = GetMethodRunners(cxInfo, context, executionManager);
            if (runners == null)
            {
                return;
            }
            foreach (var runner in runners)
            {
                runner.Run();
            }
        }
    }
}
