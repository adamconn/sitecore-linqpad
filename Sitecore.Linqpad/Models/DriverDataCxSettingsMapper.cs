using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using LINQPad.Extensibility.DataContext;
using Sitecore.Linqpad.AssemblyLoading;

namespace Sitecore.Linqpad.Models
{
    public class DriverDataCxSettingsMapper : ISitecoreConnectionSettingsMapper
    {
        /// <summary>
        /// This method reads default values for the following properties:
        /// * ContextDatabaseName
        /// * NamespacesToAdd
        /// * SearchResultType
        /// * AppConfigReaderType
        /// * SchemaBuilderType
        /// * DriverInitializerType
        /// </summary>
        /// <param name="cxInfo"></param>
        /// <param name="cxSettings"></param>
        public virtual void Read(IConnectionInfo cxInfo, ISitecoreConnectionSettings cxSettings)
        {
            if (cxInfo == null) { throw new ArgumentNullException("cxInfo"); }
            if (cxSettings == null) { throw new ArgumentNullException("cxSettings"); }
            var element = cxInfo.DriverData;
            if (element == null) { throw new NullReferenceException("DriverData on the IConnectionInfo object is null"); }
            cxSettings.ClientUrl = GetStringFromAttribute(element, KEY_CLIENT_URL);
            cxSettings.Username = GetStringFromAttribute(element, KEY_USERNAME);
            cxSettings.Password = GetStringFromAttribute(element, KEY_PASSWORD);
            cxSettings.WebRootPath = GetStringFromAttribute(element, KEY_WEB_ROOT_PATH);
            cxSettings.ContextDatabaseName = GetStringFromAttribute(element, KEY_CONTEXT_DB, DefaultValuesForCxSettings.Current.ContextDatabaseName);
            cxSettings.NamespacesToAdd = GetHashSetFromElement(element, KEY_NAMESPACES_TO_ADD, DefaultValuesForCxSettings.Current.NamespacesToAdd);

            var paths = GetHashSetFromElement(element, "paths");
            if (paths == null)
            {
                paths = new HashSet<string>();
            }
            var path = (string.IsNullOrEmpty(cxSettings.WebRootPath)) ? null : Path.Combine(cxSettings.WebRootPath, "bin");
            if (! paths.Contains(path))
            {
                paths.Add(path);
            }

            using (var context = new AssemblyLoadingContext(paths))
            {
                //
                cxSettings.SearchResultType = GetSelectedTypeFromElement(element, KEY_SEARCH_RESULT_TYPE, DefaultValuesForCxSettings.Current.SearchResultItemType);
                cxSettings.SearchResultType.GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSearchResultItem;
                //
                cxSettings.AppConfigReaderType = GetSelectedTypeFromElement(element, KEY_APP_CONFIG_READER_TYPE, DefaultValuesForCxSettings.Current.AppConfigReaderType);
                cxSettings.AppConfigReaderType.GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForAppConfigReaderType;
                //
                cxSettings.SchemaBuilderType = GetSelectedTypeFromElement(element, KEY_SCHEMA_BUILDER_TYPE, DefaultValuesForCxSettings.Current.SchemaBuilderType);
                cxSettings.SchemaBuilderType.GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSchemaBuilderType;
                //
                cxSettings.DriverInitializerType = GetSelectedTypeFromElement(element, KEY_DRIVER_INITIALIZER_TYPE, DefaultValuesForCxSettings.Current.DriverInitializerType);
                cxSettings.DriverInitializerType.GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForDriverInitializerType;
            }
        }
        /// <summary>
        /// This method sets the default values for the following properties:
        /// * ContextDatabaseName
        /// * NamespacesToAdd
        /// * SearchResultType
        /// * AppConfigReaderType
        /// * SchemaBuilderType
        /// * DriverInitializerType
        /// </summary>
        /// <param name="cxInfo"></param>
        /// <param name="cxSettings"></param>
        public virtual void Save(IConnectionInfo cxInfo, ISitecoreConnectionSettings cxSettings)
        {
            if (cxInfo == null) {  throw new ArgumentNullException("cxInfo"); }
            if (cxSettings == null) { throw new ArgumentNullException("cxSettings"); }
            var element = cxInfo.DriverData;
            if (element == null) { throw new NullReferenceException("DriverData on the IConnectionInfo object is null"); }
            SetAttribute(element, KEY_CLIENT_URL, cxSettings.ClientUrl);
            SetAttribute(element, KEY_USERNAME, cxSettings.Username);
            SetAttribute(element, KEY_PASSWORD, cxSettings.Password);
            SetAttribute(element, KEY_WEB_ROOT_PATH, cxSettings.WebRootPath);
            SetAttribute(element, KEY_CONTEXT_DB, cxSettings.ContextDatabaseName, DefaultValuesForCxSettings.Current.ContextDatabaseName);
            SetElement(element, KEY_NAMESPACES_TO_ADD, "namespace", cxSettings.NamespacesToAdd, DefaultValuesForCxSettings.Current.NamespacesToAdd);
            //
            //
            var paths = new List<string>();
            AddAssemblyPathToList(cxSettings.SearchResultType, paths);
            AddAssemblyPathToList(cxSettings.AppConfigReaderType, paths);
            AddAssemblyPathToList(cxSettings.SchemaBuilderType, paths);
            AddAssemblyPathToList(cxSettings.DriverInitializerType, paths);
            var thisAssemblyPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            if (paths.Contains(thisAssemblyPath))
            {
                paths.Remove(thisAssemblyPath);
            }
            var path = (string.IsNullOrEmpty(cxSettings.WebRootPath)) ? null : Path.Combine(cxSettings.WebRootPath, "bin");
            if (paths.Contains(path))
            {
                paths.Remove(path);
            }
            SetElement(element, "paths", "path", paths);
            //
            //
            SetElement(element, KEY_SEARCH_RESULT_TYPE, cxSettings.SearchResultType, DefaultValuesForCxSettings.Current.SearchResultItemType);
            SetElement(element, KEY_APP_CONFIG_READER_TYPE, cxSettings.AppConfigReaderType, DefaultValuesForCxSettings.Current.AppConfigReaderType);
            SetElement(element, KEY_SCHEMA_BUILDER_TYPE, cxSettings.SchemaBuilderType, DefaultValuesForCxSettings.Current.SchemaBuilderType);
            SetElement(element, KEY_DRIVER_INITIALIZER_TYPE, cxSettings.DriverInitializerType, DefaultValuesForCxSettings.Current.DriverInitializerType);
        }
        public static readonly string KEY_CLIENT_URL = "clientUrl";
        public static readonly string KEY_USERNAME = "username";
        public static readonly string KEY_PASSWORD = "password";
        public static readonly string KEY_WEB_ROOT_PATH = "webRootPath";
        public static readonly string KEY_CONTEXT_DB = "contextdb";
        public static readonly string KEY_NAMESPACES_TO_ADD = "namespacesToAdd";
        public static readonly string KEY_SEARCH_RESULT_TYPE = "searchResultType";
        public static readonly string KEY_APP_CONFIG_READER_TYPE = "appConfigReaderType";
        public static readonly string KEY_SCHEMA_BUILDER_TYPE = "schemaBuilderType";
        public static readonly string KEY_DRIVER_INITIALIZER_TYPE = "driverInitializerType";
        public static readonly string KEY_TYPE_NAME = "type";
        public static readonly string KEY_ASSEMBLY_LOCATION = "location";

        protected virtual void AddAssemblyPathToList(ISelectedType selectedType, List<string> paths)
        {
            if (paths == null) {  throw new ArgumentNullException("paths");}
            if (selectedType == null || string.IsNullOrEmpty(selectedType.AssemblyLocation))
            {
                return;
            }
            var path = Path.GetDirectoryName(selectedType.AssemblyLocation);
            if (!string.IsNullOrEmpty(path) && !paths.Contains(path))
            {
                paths.Add(path);
            }
        }

        protected virtual string GetStringFromAttribute(XElement element, string name, string defaultValue = null)
        {
            var value = (string)element.Attribute(name);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }
        protected virtual HashSet<string> GetHashSetFromElement(XElement element, string name, HashSet<string> defaultValue = null)
        {
            var element2 = element.Element(name);
            if (element2 == null || ! element2.HasElements)
            {
                return defaultValue;
            }
            return new HashSet<string>(element2.Elements().Select(e => e.Value).Distinct().ToList());
        }

        protected virtual ISelectedType GetSelectedTypeFromElement(XElement element, string name, Type defaultValue = null)
        {
            var selectedType = new SelectedType();
            var type = defaultValue;
            var element2 = element.Element(name);
            if (element2 != null)
            {
                var typeName = GetStringFromAttribute(element2, KEY_TYPE_NAME);
                if (! string.IsNullOrEmpty(typeName))
                {
                    type = Type.GetType(typeName);
                    if (type == null)
                    {
                        throw new TypeLoadException(string.Format("The type {0} could not be loaded", typeName));
                    }
                }
            }
            return new SelectedType(type);
        }

        protected virtual void SetAttribute(XElement element, string name, string value, string defaultValue = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            var attribute = element.Attribute(name);
            if (string.IsNullOrEmpty(value))
            {
                if (attribute != null)
                {
                    attribute.Remove();
                    return;
                }
            }
            else
            {
                if (attribute == null)
                {
                    attribute = new XAttribute(name, value);
                    element.Add(attribute);
                    return;
                }
                else
                {
                    attribute.Value = value;
                    return;
                }
            }
        }
        protected virtual void SetElement(XElement element, string tagName, string childName, IEnumerable<string> value, IEnumerable<string> defaultValue = null)
        {
            if (value == null || ! value.Any())
            {
                value = defaultValue;
            }
            if (value != null && ! value.Any())
            {
                value = null;
            }
            var element2 = element.Element(tagName);
            if (value == null && element2 == null)
            {
                return;
            }
            if (value == null)
            {
                element2.Remove();
                return;
            }
            if (element2 == null)
            {
                element2 = new XElement(tagName);
                element.Add(element2);
            }
            element2.RemoveAll();
            element2.Add(value.Select(v => new XElement(childName, v)));
        }

        protected virtual void SetElement(XElement element, string name, ISelectedType value, Type defaultValue = null)
        {
            Type type = null;
            if (value != null)
            {
                type = value.GetSelectedType();
            }
            if (type == null)
            {
                type = defaultValue;
            }
            var element2 = element.Element(name);
            if (type == null && element2 == null)
            {
                return;
            }
            if (type == null)
            {
                element2.Remove();
                return;
            }
            if (element2 == null)
            {
                element2 = new XElement(name);
                element.Add(element2);
            }
            SetAttribute(element2, KEY_TYPE_NAME, type.AssemblyQualifiedName);
        }
    }
}
