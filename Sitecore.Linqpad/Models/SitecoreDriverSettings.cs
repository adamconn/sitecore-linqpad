using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQPad.Extensibility.DataContext;

namespace Sitecore.Linqpad.Models
{
    public class SitecoreDriverSettings
    {
        public IConnectionInfo CxInfo { get; set; }
        public ISitecoreConnectionSettings CxSettings { get; set; }
        public ISitecoreConnectionSettingsMapper SettingsMapper { get; set; }
    }
}
