using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.AssemblyLoading
{
    public class AssemblyLoadingContext : IDisposable
    {
        public IEnumerable<string> AssemblyPaths { get; private set; }
        public AssemblyLoadingContext(IEnumerable<string> paths)
        {
            this.AssemblyPaths = paths;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (this.AssemblyPaths == null || ! this.AssemblyPaths.Any()) { return null; }
            var name = new AssemblyName(args.Name);
            foreach (var path in this.AssemblyPaths)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }
                var filePath = Path.Combine(path, name.Name + ".dll");
                if (File.Exists(filePath))
                {
                    return Assembly.LoadFile(filePath);
                }
            }
            return null;
        }
    }
}
