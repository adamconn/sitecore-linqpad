using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public class CxSettingsPathsHelper
    {
        private static CxSettingsPathsHelper _current = null;
        public static CxSettingsPathsHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new CxSettingsPathsHelper();
                }
                return _current;
            }
            set { _current = value; }
        }

        protected virtual void RemoveNonAssemblyLocations(HashSet<string> locations)
        {
            var locationsToRemove = new List<string>();
            foreach (var location in locations.ToList())
            {
                if (!IsAssembly(location))
                {
                    locationsToRemove.Add(location);
                }
            }
            foreach (var location in locationsToRemove)
            {
                locations.Remove(location);
            }
        }
        protected virtual bool IsAssembly(string path)
        {
            try
            {
                var testAssembly = AssemblyName.GetAssemblyName(path);
            }
            catch (BadImageFormatException)
            {
                return false;
            }
            return true;
        }

        public virtual HashSet<string> GetAssemblyLocations(ISitecoreConnectionSettings settings)
        {
            var locations = new HashSet<string>();
            var directories = GetAssemblyDirectories(settings);
            foreach (var directory in directories)
            {
                locations.UnionWith(Directory.GetFiles(directory, "*.dll"));
            }
            RemoveNonAssemblyLocations(locations);
            return locations;
        }
        public virtual HashSet<string> GetAssemblyDirectories(ISitecoreConnectionSettings settings)
        {
            var paths = new HashSet<string>();
            if (!string.IsNullOrEmpty(settings.WebRootPath))
            {
                paths.Add(Path.Combine(settings.WebRootPath, "bin"));
            }
            AddAssemblyPath(settings.SearchResultType, paths);
            AddAssemblyPath(settings.AppConfigReaderType, paths);
            AddAssemblyPath(settings.SchemaBuilderType, paths);
            AddAssemblyPath(settings.DriverInitializerType, paths);
            return paths;
        }
        private void AddAssemblyPath(ISelectedType selectedType, HashSet<string> paths)
        {
            if (selectedType == null || string.IsNullOrEmpty(selectedType.AssemblyLocation) || paths == null)
            {
                return;
            }
            var path = Path.GetDirectoryName(selectedType.AssemblyLocation);
            if (!string.IsNullOrEmpty(path) && !paths.Contains(path))
            {
                paths.Add(path);
            }
        }

    }
}
