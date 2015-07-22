using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Sitecore.Linqpad.AppConfigReaders;
using Sitecore.Linqpad.Models;
using Sitecore.Linqpad.Controllers;
using Sitecore.Linqpad.Server;
using MessageBox = System.Windows.MessageBox;
using Sitecore.Linqpad.Common;
using Cursors = System.Windows.Input.Cursors;
using TextBox = System.Windows.Controls.TextBox;

namespace Sitecore.Linqpad.Dialogs
{
    public partial class ConnectionDialog : Window, ISitecoreConnectionSettings
    {
        private const string TEST_CONNECTION_CAPTION = "Test Connection to Sitecore";
        private const string ABOUT_DRIVER_CAPTION = "About Driver";
        public ConnectionDialog()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        ~ConnectionDialog()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var paths = CxSettingsPathsHelper.Current.GetAssemblyDirectories(this);
            if (paths != null && paths.Count > 0)
            {
                var name = new AssemblyName(args.Name);
                foreach (var path in paths)
                {
                    var filePath = Path.Combine(path, name.Name + ".dll");
                    if (File.Exists(filePath))
                    {
                        return Assembly.LoadFile(filePath);
                    }
                }
            }
            return null;
        }

        #region ISitecoreConnectionSettings
        public SitecoreDriverSettings Model { get; set; }
        public CallbackWithParameter<SitecoreDriverSettings> SaveViewToModelCallback { get; set; }

        public string ClientUrl
        {
            get { return this.txtClientUrl.Text; }
            set { this.txtClientUrl.Text = value; }
        }
        public string Username
        {
            get { return this.txtUsername.Text; }
            set { this.txtUsername.Text = value; }
        }
        public string Password
        {
            get { return this.txtPassword.Text; }
            set { this.txtPassword.Text = value; }
        }
        public string WebRootPath
        {
            get { return this.txtWebrootPath.Text; }
            set { this.txtWebrootPath.Text = value; }
        }
        public string ContextDatabaseName
        {
            get { return this.txtContextDatabase.Text; }
            set { this.txtContextDatabase.Text = value; }
        }

        private HashSet<string> _namespacesToAdd = null;
        public HashSet<string> NamespacesToAdd
        {
            get
            {
                if (_namespacesToAdd != null)
                {
                    _namespacesToAdd.Clear();
                }
                else
                {
                    _namespacesToAdd = new HashSet<string>();
                }
                if (! string.IsNullOrEmpty(this.txtNamespacesToAdd.Text))
                {
                    _namespacesToAdd.UnionWith(this.txtNamespacesToAdd.Text.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray());
                }
                return _namespacesToAdd;
            }
            set
            {
                string newValue = null;
                if (value != null && value.Count > 0)
                {
                    newValue = GetNamespacesAsString(value);
                }
                this.txtNamespacesToAdd.Text = newValue;
            }
        }

        private string GetNamespacesAsString(IEnumerable<string> value)
        {
            return string.Join(string.Empty, value.Select(x => string.IsNullOrEmpty(x) ? null : string.Format("{0};\n", x)));
        }
        private string GetValueFromSelectedType(ISelectedType selectedType)
        {
            if (selectedType == null)
            {
                return null;
            }
            var type = selectedType.GetSelectedType();
            if (type == null)
            {
                return null;
            }
            return type.AssemblyQualifiedName;
        }
        public ISelectedType SearchResultType { get; set; }
        public ISelectedType AppConfigReaderType { get; set; }
        public ISelectedType SchemaBuilderType { get; set; }
        public ISelectedType DriverInitializerType { get; set; }
        #endregion

        #region Test Connection button
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var manager = new SitecoreConnectionManager(this);
            if (! this.IsValidWebRoot(this.txtWebrootPath.Text))
            {
                return;
            }
            if (! this.IsValidConnectionSettings(manager))
            {
                return;
            }
            var version = GetSitecoreVersion(manager);
            if (version == null)
            {
                return;
            }
            var reader = this.AppConfigReaderType.GetInstance<IAppConfigReader>();
            if (reader == null)
            {
                MessageBox.Show("No app config reader could be resolved.", TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var builder = new StringBuilder();
            builder.AppendFormat("Connection settings are valid for {0}.", version);
            if (!reader.IsSupportedVersion(version))
            {
                builder.Append(
                    "\n\nThis version of Sitecore is not supported by the currently selected IAppConfigReader class.");
                builder.Append(
                    "\n\nThis means that config changes implemented by the IAppConfigReader may conflict with your version of Sitecore. This will not affect the Sitecore server, but it may prevent LINQPad from being able to properly communicate with the Sitecore server.");
                builder.Append(
                    "\n\nIf you experience problems you may need to implement a new IAppConfigReader or extend the one currently being used.");
            }
            MessageBox.Show(builder.ToString(), TEST_CONNECTION_CAPTION, MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
        }

        protected virtual Version GetSitecoreVersion(SitecoreConnectionManager manager)
        {
            var response = manager.GetSitecoreVersion();
            if (response == null)
            {
                MessageBox.Show("GetSitecoreVersion() returned a null response.", TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            if (response.Data == null)
            {
                var b1 = new StringBuilder();
                b1.AppendLine("The Sitecore server version could not be determined.\n\nThe following messages were returned:\n");
                b1.AppendLine(string.Join("\n", response.GetExceptions().Select(ex => string.Format("* {0}", ex.Message))));
                MessageBox.Show(b1.ToString(), TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return response.Data;
        }

        protected virtual bool IsValidConnectionSettings(SitecoreConnectionManager manager)
        {
            if (string.IsNullOrEmpty(this.ClientUrl))
            {
                return false;
            }
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var version = GetSitecoreVersion(manager);
                if (version == null)
                {
                    return false;
                }
                var response = manager.TestConnection();
                if (response == null)
                {
                    MessageBox.Show("TestConnection() returned a null response.", TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (!response.Data)
                {
                    var b1 = new StringBuilder();
                    b1.AppendLine("Unable to connect to the Sitecore server using the specified settings.");
                    if (response.GetExceptions().Any())
                    {
                        b1.AppendLine("\n\nThe following exceptions were thrown:\n");
                        b1.AppendLine(string.Join("\n", response.GetExceptions().Select(ex => string.Format("* {0}", ex.Message))));
                    }
                    MessageBox.Show(b1.ToString(), TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return response.Data;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        protected virtual bool IsValidWebRoot(string path)
        {
            var flag = true;
            string message = null;
            if (!Directory.Exists(path))
            {
                message = "The specified web root folder does not exist.";
                flag = false;
            }
            else if (!File.Exists(Path.Combine(path, "web.config")))
            {
                message = "Cannot find web.config in the specified web root folder.";
                flag = false;
            }
            if (!flag)
            {
                MessageBox.Show(message, TEST_CONNECTION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            return flag;
        }
        #endregion

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.txtSearchResultItemTypeName.Text = this.SearchResultType.TypeName;
            this.txtAppConfigReaderTypeName.Text = this.AppConfigReaderType.TypeName;
            this.txtSchemaBuilderTypeName.Text = this.SchemaBuilderType.TypeName;
            this.txtDriverInitializerTypeName.Text = this.DriverInitializerType.TypeName;
        }

        protected virtual void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (this.Model == null) { return; }
            if (! CxSettingsCompareHelper.Current.AreEqual(this, this.Model.CxSettings))
            {
                var result = MessageBox.Show(this, "Are you sure you want to cancel and lose your changes?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                {
                    cancelEventArgs.Cancel = true;
                }
            }
        }

        protected virtual void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();
            var assembly = this.GetType().Assembly;
            var attr = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false) as AssemblyInformationalVersionAttribute[];
            if (attr != null && attr.Length != 0)
            {
                builder.AppendFormat("Driver version: {0}\n", attr[0].InformationalVersion);
            }
            builder.AppendFormat("Assembly version: {0}\n", assembly.GetName().Version);
            builder.AppendFormat("Timestamp: {0}\n", File.GetLastWriteTime(assembly.Location).ToString());
            builder.AppendFormat("Location: {0}\n", Path.GetDirectoryName(assembly.Location));
            MessageBox.Show(builder.ToString(), ABOUT_DRIVER_CAPTION, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        protected virtual void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //
            //save the changes to the driver settings
            if (this.SaveViewToModelCallback != null)
            {
                this.SaveViewToModelCallback(this.Model);
            }
            //
            //update app.config for this connection
            var reader = this.AppConfigReaderType.GetInstance<IAppConfigReader>();
            if (reader == null)
            {
                MessageBox.Show("Unable to get an IAppConfigReader object.", "Error Saving Connection", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var cxManager = new SitecoreConnectionManager(this.Model.CxSettings);
                var config = reader.Read(cxManager, this.Model, "web.config", false);
                if (config == null)
                {
                    MessageBox.Show("IAppConfigReader object returned no document.", "Error Saving Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    reader.Save(config, this.Model);
                }
            }
            //
            //
            this.DialogResult = true;
        }

        protected virtual void BtnBrowseWebRoot_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (Directory.Exists(this.WebRootPath))
            {
                dialog.SelectedPath = this.WebRootPath;
            }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtWebrootPath.Text = dialog.SelectedPath;
            }
        }

        #region DisplayHelp methods
        private Dictionary<object, HelpTopic> _controlToHelpTopicMapping = null;

        protected Dictionary<object, HelpTopic> ControlToHelpTopicMapping
        {
            get
            {
                if (_controlToHelpTopicMapping == null)
                {
                    _controlToHelpTopicMapping = new Dictionary<object, HelpTopic>();
                    InitializeControlToHelpInfoMapping(_controlToHelpTopicMapping);
                }
                return _controlToHelpTopicMapping;
            }
        }
        protected virtual void InitializeControlToHelpInfoMapping(Dictionary<object, HelpTopic> mapping)
        {
            if (mapping == null)
            {
                return;
            }
            mapping.Clear();

            mapping.Add(helpClientUrl, new HelpTopic() { Title = "Client URL", Text = "Location of the Sitecore client on the server.\n\nFor example, 'http://localhost/sitecore'" });
            mapping.Add(helpUsername, new HelpTopic() { Title = "User Name", Text = "User name for a Sitecore user with administrator access.\n\nFor example, 'sitecore\\admin'" });
            mapping.Add(helpPassword, new HelpTopic() { Title = "Password", Text = "Password for the Sitecore user." });
            mapping.Add(helpWebrootPath, new HelpTopic() { Title = "Web Root", Text = "Path to the webroot folder for the Sitecore server.\n\nFor example, 'c:\\Sitecore\\Website'" });
            mapping.Add(helpContextDatabase, new HelpTopic() { Title = "Context Database", Text = "Name of the database that is resolved when Sitecore.Context.Database is called. \n\nFor example, 'master'" });
            mapping.Add(helpSearchResultItemTypeName, new HelpTopic() { Title = "Search Result Type", Text = "The search result item type is the type of the data in the IQueryable<T> data source." });
            mapping.Add(helpAppConfigReaderTypeName, new HelpTopic() { Title = "App.config Reader", Text = "The app.config reader reads the web.config file from the Sitecore server and makes necessary changes in order for the configuration to be used by LINQPad." });
            mapping.Add(helpSchemaBuilderTypeName, new HelpTopic() { Title = "Schema Builder", Text = "The schema builder generates the object that is available for the connection\n\nFor example, the default schema builder generates an object with a property that represents each index available on the Sitecore server." });
            mapping.Add(helpDriverInitializerTypeName, new HelpTopic() { Title = "Driver Initializer", Text = "LINQPad controls when the driver is loaded. This class allows you to add code that runs when the driver is loaded.\n\nSitecore content search needs a hook to be initialized. This is normally handled the initialize pipeline. This pipeline does not run within the LINQPad context, but this hook still must be initialized. A driver initializer ensures this happens." });

            var builder = new StringBuilder();
            builder.AppendLine("The namespaces included in this section will be automatically added within the LINQPad context.\n");
            builder.AppendLine("By default the following namespaces will be added. These are also the namespaces that will be restored if this field is reset:");
            var defaultNs = DefaultValuesForCxSettings.Current.NamespacesToAdd;
            if (defaultNs != null)
            {
                foreach (var ns in defaultNs)
                {
                    builder.AppendFormat("* {0}\n", ns);
                }
            }
            builder.AppendLine("In addition, the namespace for the search result item type is included automatically.\n");
            mapping.Add(helpNamespacesToAdd, new HelpTopic() { Title = "Additional Namespaces", Text = builder.ToString() });
        }

        protected virtual HelpTopic GetHelpTopicForControl(object sender)
        {
            var mapping = this.ControlToHelpTopicMapping;
            if (mapping == null || ! mapping.ContainsKey(sender))
            {
                return null;
            }
            return mapping[sender];
        }

        private void lnkDisplayHelp_Click(object sender, RoutedEventArgs e)
        {
            string title = null;
            string text = null;
            var topic = GetHelpTopicForControl(sender);
            if (topic != null)
            {
                title = string.Format("Help for {0}", topic.Title);
                text = topic.Text;
            }
            else
            {
                title = "Help Unavailable";
                text = string.Format("No help topic is available for the selected control {0}.", sender);
            }
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        #endregion

        #region Type selectors
        private void browseSearchResultItemType_Click(object sender, RoutedEventArgs e)
        {
            if (this.SearchResultType == null)
            {
                this.SearchResultType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSearchResultItem
                };
            }
            ShowTypeSelectorDialog(this.SearchResultType, "Select IQueryable<T> Type", txtSearchResultItemTypeName);
        }
        private void browseAppConfigReaderType_Click(object sender, RoutedEventArgs e)
        {
            if (this.AppConfigReaderType == null)
            {
                this.AppConfigReaderType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForAppConfigReaderType
                };
            }
            ShowTypeSelectorDialog(this.AppConfigReaderType, "Select app.config Reader Type", txtAppConfigReaderTypeName);
        }
        private void browseSchemaBuilderType_Click(object sender, RoutedEventArgs e)
        {
            if (this.SchemaBuilderType == null)
            {
                this.SchemaBuilderType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSchemaBuilderType
                };
            }
            ShowTypeSelectorDialog(this.SchemaBuilderType, "Select Scheme Builder Type", txtSchemaBuilderTypeName);
        }
        private void browseDriverInitializerType_Click(object sender, RoutedEventArgs e)
        {
            if (this.DriverInitializerType == null)
            {
                this.DriverInitializerType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForDriverInitializerType
                };
            }
            ShowTypeSelectorDialog(this.DriverInitializerType, "Select Driver Initializer Type", txtDriverInitializerTypeName);
        }
        private void ShowTypeSelectorDialog(ISelectedType selectedType, string dialogTitle, TextBox textBox)
        {
            var view = new SelectedTypeViewModel(selectedType)
            {
                DialogTitle = dialogTitle,
                TextBox = textBox
            };
            var controller = new TypeSelectorController(view);
            controller.LoadView();
            var dialog = new TypeSelectorDialog()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Model = view
            };
            view.SaveViewToModelCallback = controller.SaveView;
            if (dialog.ShowDialog() != true)
            {
                return;
            }
        }
        #endregion

        #region Type selector textbox event handlers
        protected virtual void txtDriverInitializerTypeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var type = Type.GetType(this.txtDriverInitializerTypeName.Text);
            if (this.SearchResultType == null)
            {
                type = DefaultValuesForCxSettings.Current.DriverInitializerType;
                this.DriverInitializerType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForDriverInitializerType
                };
            }
            this.DriverInitializerType.SetSelectedType(type);
        }

        void txtSchemaBuilderTypeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var type = Type.GetType(this.txtSchemaBuilderTypeName.Text);
            if (this.SearchResultType == null)
            {
                type = DefaultValuesForCxSettings.Current.SchemaBuilderType;
                this.SchemaBuilderType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSchemaBuilderType
                };
            }
            this.SchemaBuilderType.SetSelectedType(type);
        }

        void txtAppConfigReaderTypeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var type = Type.GetType(this.txtAppConfigReaderTypeName.Text);
            if (this.SearchResultType == null)
            {
                type = DefaultValuesForCxSettings.Current.AppConfigReaderType;
                this.AppConfigReaderType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForAppConfigReaderType
                };
            }
            this.AppConfigReaderType.SetSelectedType(type);
        }

        protected virtual void txtSearchResultItemTypeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var type = Type.GetType(this.txtSearchResultItemTypeName.Text);
            if (this.SearchResultType == null)
            {
                type = DefaultValuesForCxSettings.Current.SearchResultItemType;
                this.SearchResultType = new SelectedType()
                {
                    GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSearchResultItem
                };
            }
            this.SearchResultType.SetSelectedType(type);
        }
        #endregion

        #region Field value reset descriptions

        protected virtual void ResetType(ISelectedType currentSelectedType, Type newType, TextBox textBox)
        {
            if (ConfirmTypeReset(currentSelectedType, newType))
            {
                currentSelectedType.SetSelectedType(newType);
                textBox.Text = newType.AssemblyQualifiedName;
            }   
        }
        protected virtual bool ConfirmTypeReset(ISelectedType currentSelectedType, Type newType)
        {
            var currentType = (currentSelectedType == null) ? null : currentSelectedType.GetSelectedType();
            if (currentType == newType)
            {
                MessageBox.Show(this, "The type is already set to its default value.", "Reset Selected Type", MessageBoxButton.OK);
                return false;
            }
            var result = MessageBox.Show(this, "Are you sure you want to reset this type to its default value?", "Reset Selected Type", MessageBoxButton.YesNo);
            return (result == MessageBoxResult.Yes);
        }
        private void resetSearchResultItemType_Click(object sender, RoutedEventArgs e)
        {
            ResetType(this.SearchResultType, DefaultValuesForCxSettings.Current.SearchResultItemType, this.txtSearchResultItemTypeName);
        }
        private void resetAppConfigReaderType_Click(object sender, RoutedEventArgs e)
        {
            ResetType(this.AppConfigReaderType, DefaultValuesForCxSettings.Current.AppConfigReaderType, this.txtAppConfigReaderTypeName);
        }
        private void resetSchemaBuilderType_Click(object sender, RoutedEventArgs e)
        {
            ResetType(this.SchemaBuilderType, DefaultValuesForCxSettings.Current.SchemaBuilderType, this.txtSchemaBuilderTypeName);
        }
        private void resetDriverInitializerType_Click(object sender, RoutedEventArgs e)
        {
            ResetType(this.DriverInitializerType, DefaultValuesForCxSettings.Current.DriverInitializerType, this.txtDriverInitializerTypeName);
        }
        private void resetNamespaces_Click(object sender, RoutedEventArgs e)
        {
            if (this.NamespacesToAdd.SetEquals(DefaultValuesForCxSettings.Current.NamespacesToAdd))
            {
                return;
            }
            var result = MessageBox.Show(this, "Are you sure you want to reset the namespaces to its default value?", "Reset Namespaces", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            this.txtNamespacesToAdd.Text = GetNamespacesAsString(DefaultValuesForCxSettings.Current.NamespacesToAdd);
        }
        #endregion

    }
}