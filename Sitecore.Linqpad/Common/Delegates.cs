using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Common
{
    public delegate void CallbackWithParameter<T>(T parameter);
    public delegate void DefaultCallback();
}
