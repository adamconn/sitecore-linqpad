using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.DriverInitializers
{
    public class MethodRunner
    {
        public MethodRunner(string typeName, string methodName)
        {
            if (string.IsNullOrEmpty(typeName)) { throw new ArgumentNullException("typeName"); }
            if (string.IsNullOrEmpty(methodName)) {  throw new ArgumentNullException("methodName"); }
            this.TypeName = typeName;
            this.MethodName = methodName;
        }
        public string TypeName { get; private set; }
        public string MethodName { get; private set; }
        public object[] Parameters { get; set; }
        public virtual void Run()
        {
            var type = Type.GetType(this.TypeName);
            if (type == null) { throw new TypeLoadException(); }
            var method = type.GetMethod(this.MethodName);
            if (method == null) { throw new MissingMethodException(this.TypeName, this.MethodName);}
            var obj2 = Activator.CreateInstance(type);
            method.Invoke(obj2, this.Parameters);
        }
    }
}
