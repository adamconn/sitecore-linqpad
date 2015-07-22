using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public delegate IEnumerable<Type> GetValidTypes(Assembly assembly);
    /// <summary>
    /// The model used by the type selector dialog.
    /// </summary>
    public class SelectedType : ISelectedType
    {
        public SelectedType()
        {
        }
        public SelectedType(Type type)
        {
            if (type != null)
            {
                this.TypeName = type.AssemblyQualifiedName;
                this.AssemblyLocation = type.Assembly.Location;
            }
        }

        public string TypeName { get; set; }
        public string AssemblyLocation { get; set; }

        public GetValidTypes GetValidTypesDelegate { get; set; }
        public bool IsValidType(Type type)
        {
            if (type == null || this.GetValidTypesDelegate == null)
            {
                return true;
            }
            var types = this.GetValidTypesDelegate(type.Assembly);
            if (types == null)
            {
                return true;
            }
            return this.GetValidTypesDelegate(type.Assembly).Contains(type);
        }

        public virtual void SetSelectedType(Type type)
        {
            if (type == null)
            {
                this.TypeName = null;
                this.AssemblyLocation = null;
                return;
            }
            if (! this.IsValidType(type)) { throw new ArgumentException("The specified type is not valid to be selected.", "type"); }
            this.TypeName = type.AssemblyQualifiedName;
            this.AssemblyLocation = type.Assembly.Location;
        }

        public virtual Type GetSelectedType()
        {
            if (string.IsNullOrEmpty(this.TypeName)) { return null; }
            Type type = null;
            if (!string.IsNullOrEmpty(this.AssemblyLocation) && File.Exists(this.AssemblyLocation))
            {
                var assembly = Assembly.LoadFile(this.AssemblyLocation);
                var typeName = this.TypeName.Split(',')[0];
                type = assembly.GetType(typeName);
            }
            if (type == null)
            {
                type = Type.GetType(this.TypeName);
            }
            if (type == null) { throw new TypeLoadException(string.Format("The type {0} could not be loaded.", this.TypeName)); }
            return type;
        }
        public virtual T GetInstance<T>() where T : class
        {
            var selectedType = GetSelectedType();
            if (selectedType == null)
            {
                return default(T);
            }
            return (Activator.CreateInstance(selectedType) as T);
        }

    }
}
