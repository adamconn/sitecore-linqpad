using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Common;

namespace Sitecore.Linqpad.Dialogs
{
    public class SelectedTypeViewModel 
    {
        public SelectedTypeViewModel(ISelectedType model)
        {
            this.Model = model;
        }

        public DefaultCallback SaveViewToModelCallback { get; set; } //this delegate is needed in order to avoid having to set a reference directly to controller
        
        public GetValidTypes GetValidTypesDelegate { get; set; }
        public string DialogTitle { get; set; }
        public TextBox TextBox { get; set; }
        public string TypeName { get; set; }
        public string AssemblyLocation { get; set; }
        public ISelectedType Model { get; private set; }
    }
}
