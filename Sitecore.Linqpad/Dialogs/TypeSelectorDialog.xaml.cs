using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Dialogs
{
    /// <summary>
    /// Interaction logic for TypeSelectorDialog.xaml
    /// </summary>
    public partial class TypeSelectorDialog : Window, ISelectedType
    {
        public TypeSelectorDialog()
        {
            InitializeComponent();
        }

        public SelectedTypeViewModel Model { get; set; }

        protected virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Model == null) { throw new NullReferenceException("A model must be specified."); }
            SetSelectedAssembly(false);
        }
        
        protected virtual void SetSelectedAssembly(bool displayError = true)
        {
            SetSelectedAssembly(this.Model.AssemblyLocation, displayError);
        }
        protected virtual void SetSelectedAssembly(string assemblyLocation, bool displayError = true)
        {
            if (string.IsNullOrEmpty(assemblyLocation))
            {
                return;
            }
            var assembly = GetAssemblyFromLocation(assemblyLocation);
            if (assembly == null)
            {
                if (displayError)
                {
                    DisplayErrorMessage("Invalid Selection", string.Format("{0} is not an assembly.", assemblyLocation));
                }
                return;
            }
            txtAssembly.Text = assemblyLocation;
            PopulateListOfTypes(assembly);
        }
        private Assembly GetAssemblyFromLocation(string assemblyLocation)
        {
            if (!File.Exists(assemblyLocation))
            {
                return null;
            }
            return Assembly.LoadFile(assemblyLocation);
        }
        protected virtual OpenFileDialog GetSelectAssemblyDialog()
        {
            if (this.Model == null) { throw new NullReferenceException("A model must be specified."); }
            string initialDirectory = null;
            string fileName = null;
            if (File.Exists(this.Model.AssemblyLocation))
            {
                initialDirectory = Path.GetDirectoryName(this.Model.AssemblyLocation);
                fileName = Path.GetFileName(this.Model.AssemblyLocation);
            }
            var dialog = new OpenFileDialog
            {
                Title = "Select Assembly",
                Filter = "Component Files|*.dll;*.tlb;*.olb;*.olb;*.ocx;*.exe",
                InitialDirectory = initialDirectory,
                FileName = fileName
            };
            return dialog;
        }

        protected virtual void BrowseAssembly(object sender, RoutedEventArgs e)
        {
            //
            //display the dialog
            var dialog = GetSelectAssemblyDialog();
            var result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            //
            //validate selection
            if (!File.Exists(dialog.FileName))
            {
                DisplayErrorMessage("Invalid Selection", string.Format("The file {0} does not exist.", dialog.FileName));
                return;
            }
            SetSelectedAssembly(dialog.FileName);
        }

        protected virtual void PopulateListOfTypes(Assembly assembly)
        {
            if (assembly == null)
            {
                return;
            }
            IEnumerable<Type> types = null;
            if (this.Model.GetValidTypesDelegate != null)
            {
                types = this.Model.GetValidTypesDelegate(assembly);
            }
            else
            {
                types = assembly.GetTypes();
            }
            if (types == null)
            {
                return;
            }
            listTypes.ItemsSource = types;
            if (! string.IsNullOrEmpty(this.Model.TypeName))
            {
                var selectedType = Type.GetType(this.Model.TypeName);
                if (selectedType == null)
                {
                    return;
                }
                var index = listTypes.Items.IndexOf(selectedType);
                listTypes.SelectedIndex = index;
                listTypes.ScrollIntoView(selectedType);
            }
        }

        protected virtual void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (listTypes.SelectedIndex == -1)
            {
                DisplayErrorMessage("Invalid Selection", "A type must be selected.");
                return;
            }
            var type = listTypes.SelectedItem as Type;
            this.Model.AssemblyLocation = type.Assembly.Location;
            this.Model.TypeName = type.AssemblyQualifiedName;
            this.Model.SaveViewToModelCallback();
            this.DialogResult = true;
        }
        
        protected virtual void DisplayErrorMessage(string caption, string message)
        {
            System.Windows.MessageBox.Show(this, message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public string TypeName { get; set; }
        public string AssemblyLocation { get; set; }
        public GetValidTypes GetValidTypesDelegate { get; set; }
        public virtual bool IsValidType(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual void SetSelectedType(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual Type GetSelectedType()
        {
            throw new NotImplementedException();
        }

        public virtual T GetInstance<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
