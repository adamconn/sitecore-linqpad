using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public class CxSettingsCompareHelper
    {
        private static CxSettingsCompareHelper _current = null;
        public static CxSettingsCompareHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new CxSettingsCompareHelper();
                }
                return _current;
            }
            set { _current = value; }
        }
        public virtual bool AreEqual(ISitecoreConnectionSettings cxSettings1, ISitecoreConnectionSettings cxSettings2)
        {
            if (cxSettings1 == cxSettings2) { return true; }
            if (cxSettings1 == null || cxSettings2 == null) { return false; }
            if (cxSettings1.Equals(cxSettings2)) { return true; }
            return (GetHashCodeFor(cxSettings1) == GetHashCodeFor(cxSettings2));
        }

        protected virtual int GetHashCodeFor(string value)
        {
            if (string.IsNullOrEmpty(value)) { return string.Empty.GetHashCode(); }
            return value.GetHashCode();
        }
        protected virtual int GetHashCodeFor(ISitecoreConnectionSettings cxSettings)
        {
            if (cxSettings == null) { return 0; }
            unchecked
            {
                var hash = GetHashCodeFor(cxSettings.ClientUrl);
                hash = hash * GetHashCodeFor(cxSettings.ClientUrl);
                hash = hash * GetHashCodeFor(cxSettings.Username);
                hash = hash * GetHashCodeFor(cxSettings.Password);
                hash = hash * GetHashCodeFor(cxSettings.WebRootPath);
                hash = hash * GetHashCodeFor(cxSettings.ContextDatabaseName);
                if (cxSettings.NamespacesToAdd != null)
                {
                    foreach (var ns in cxSettings.NamespacesToAdd)
                    {
                        hash = hash * GetHashCodeFor(ns);
                    }
                }
                hash = hash * GetHashCodeFor(cxSettings.SearchResultType);
                hash = hash * GetHashCodeFor(cxSettings.AppConfigReaderType);
                hash = hash * GetHashCodeFor(cxSettings.SchemaBuilderType);
                hash = hash * GetHashCodeFor(cxSettings.DriverInitializerType);
                return hash;
            }
        }

        protected virtual int GetHashCodeFor(ISelectedType selectedType)
        {
            if (selectedType == null) { return string.Empty.GetHashCode(); }
            var type = selectedType.GetSelectedType();
            if (type == null) { return string.Empty.GetHashCode(); }
            return type.GetHashCode();
        }
    }
}
