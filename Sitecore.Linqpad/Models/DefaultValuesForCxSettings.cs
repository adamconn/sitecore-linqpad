using System.IO;
using Sitecore.Linqpad.AppConfigReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sitecore.Linqpad.AssemblyLoading;
using Sitecore.Linqpad.DriverInitializers;
using Sitecore.Linqpad.SchemaBuilders;

namespace Sitecore.Linqpad.Models
{
    public class DefaultValuesForCxSettings
    {
        public readonly string Delimiter = ";";
        public readonly string ContextDatabaseName = "master";

        private static DefaultValuesForCxSettings _current = null;
        public static DefaultValuesForCxSettings Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DefaultValuesForCxSettings();
                }
                return _current;
            }
            set { _current = value; }
        }

        private const string SearchResultItemTypeName = "Sitecore.ContentSearch.SearchTypes.SearchResultItem, Sitecore.ContentSearch";
        public virtual Type SearchResultItemType
        {
            get { return Type.GetType(SearchResultItemTypeName); }
        }
        public virtual IEnumerable<Type> GetValidTypesForSearchResultItem(Assembly assembly)
        {
            if (assembly == null) { return null; }
            var paths = new List<string>();
            using (var context = new AssemblyLoadingContext(paths))
            {
                return (from t in assembly.GetTypes()
                    where 
                        (t.IsPublic && !t.IsAbstract) && (t.GetConstructor(System.Type.EmptyTypes) != null)
                    orderby t.AssemblyQualifiedName
                    select t);
            }
        }

        public virtual Type AppConfigReaderType { get { return typeof(Sitecore8AppConfigReader); } }
        public virtual IEnumerable<Type> GetValidTypesForAppConfigReaderType(Assembly assembly)
        {
            if (assembly == null) { return null; }
            var paths = new List<string>();
            using (var context = new AssemblyLoadingContext(paths))
            {
                return (from t in assembly.GetTypes()
                    where
                        ((t.IsPublic && !t.IsAbstract) && (t.GetConstructor(System.Type.EmptyTypes) != null)) &&
                        typeof (IAppConfigReader).IsAssignableFrom(t)
                    orderby t.AssemblyQualifiedName
                    select t);
            }
        }

        public virtual Type SchemaBuilderType { get { return typeof(SitecoreSchemaBuilder); } }
        public virtual IEnumerable<Type> GetValidTypesForSchemaBuilderType(Assembly assembly)
        {
            if (assembly == null) { return null; }
            var paths = new List<string>();
            using (var context = new AssemblyLoadingContext(paths))
            {
                return (from t in assembly.GetTypes()
                    where
                        ((t.IsPublic && !t.IsAbstract) && (t.GetConstructor(System.Type.EmptyTypes) != null)) &&
                        typeof (ISchemaBuilder).IsAssignableFrom(t)
                    orderby t.AssemblyQualifiedName
                    select t);
            }
        }

        public virtual Type DriverInitializerType { get { return typeof(SitecoreDriverInitializer); } }
        public virtual IEnumerable<Type> GetValidTypesForDriverInitializerType(Assembly assembly)
        {
            if (assembly == null) { return null; }
            var paths = new List<string>();
            using (var context = new AssemblyLoadingContext(paths))
            {
                return (from t in assembly.GetTypes()
                    where
                        ((t.IsPublic && !t.IsAbstract) && (t.GetConstructor(System.Type.EmptyTypes) != null)) &&
                        typeof (IDriverInitializer).IsAssignableFrom(t)
                    orderby t.AssemblyQualifiedName
                    select t);
            }
        }

        private const string NamespacesToAddAsString = 
            "System;" + 
            "System.Linq;" + 
            "Sitecore.ContentSearch;" + 
            "Sitecore.ContentSearch.SearchTypes;" + 
            "Sitecore.ContentSearch.Linq;" + 
            "Sitecore.Data";

        public virtual HashSet<string> NamespacesToAdd
        {
            get { return new HashSet<string>(NamespacesToAddAsString.Split(';').ToList()); }
        }

        public virtual bool DoesValueMatchList(string value, string delimiter, HashSet<string> list)
        {
            if (string.IsNullOrEmpty(value))
            {
                return (list == null || list.Count == 0);
            }
            if (list == null || list.Count == 0)
            {
                return false;
            }
            var valueAsArray = value.Split(new string[] {delimiter}, StringSplitOptions.RemoveEmptyEntries);
            if (valueAsArray.Length != list.Count)
            {
                return false;
            }
            return (valueAsArray.All(list.Contains));
        }
    }
}
