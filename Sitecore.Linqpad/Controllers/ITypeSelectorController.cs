using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Linqpad.Dialogs;

namespace Sitecore.Linqpad.Controllers
{
    public interface ITypeSelectorController
    {
        SelectedTypeViewModel View { get; }
        void LoadView(); //no model is passed because the model is available from the viewmodel
        void SaveView(); //no model is passed because the model is available from the viewmodel
    }
}
