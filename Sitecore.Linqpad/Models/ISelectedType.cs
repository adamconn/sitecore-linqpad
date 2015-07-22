using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Models
{
    public interface ISelectedType
    {
        string TypeName { get; set; }
        string AssemblyLocation { get; set; }
        GetValidTypes GetValidTypesDelegate { get; set; }
        bool IsValidType(Type type);
        void SetSelectedType(Type type);
        Type GetSelectedType();
        T GetInstance<T>() where T : class;
    }
}
