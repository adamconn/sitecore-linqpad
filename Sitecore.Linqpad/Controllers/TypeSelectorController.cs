using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Linqpad.Dialogs;

namespace Sitecore.Linqpad.Controllers
{
    public class TypeSelectorController : ITypeSelectorController
    {
        public TypeSelectorController(SelectedTypeViewModel view)
        {
            if (view == null) { throw new ArgumentNullException("view"); }
            this.View = view;
        }
        public SelectedTypeViewModel View { get; private set; }

        public void LoadView()
        {
            //copy from model to view
            var model = this.View.Model;
            if (model == null) { throw new NullReferenceException("The view model does not have a model assigned to it"); }
            this.View.GetValidTypesDelegate = model.GetValidTypesDelegate;
            var type = model.GetSelectedType();
            if (type != null)
            {
                this.View.TypeName = type.FullName;
                this.View.AssemblyLocation = type.Assembly.Location;
            }
        }

        public void SaveView()
        {
            //copy from view to model
            var model = this.View.Model;
            if (model == null) { throw new NullReferenceException("The view model does not have a model assigned to it"); }
            var assembly = Assembly.LoadFile(this.View.AssemblyLocation);
            var typeName = this.View.TypeName.Split(',')[0];
            var type = assembly.GetType(typeName);
            model.SetSelectedType(type);
            this.View.TextBox.Text = type.AssemblyQualifiedName;
        }

    }
}
