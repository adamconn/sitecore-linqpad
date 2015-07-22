using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQPad.Extensibility.DataContext;

namespace Sitecore.Linqpad.Models
{
    public interface ISitecoreConnectionSettingsMapper
    {
        void Read(IConnectionInfo cxInfo, ISitecoreConnectionSettings cxSettings);
        void Save(IConnectionInfo cxInfo, ISitecoreConnectionSettings cxSettings);
    }
}
