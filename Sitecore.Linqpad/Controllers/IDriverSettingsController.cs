using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Linqpad.Dialogs;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Controllers
{
    public interface IDriverSettingsController
    {
        ISitecoreConnectionSettings View { get; }
        void LoadView(SitecoreDriverSettings model);
        void SaveView(SitecoreDriverSettings model);
    }
}
