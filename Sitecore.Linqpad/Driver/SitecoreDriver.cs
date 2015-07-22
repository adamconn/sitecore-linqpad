using System.IO;
using System.Windows;
using LINQPad.Extensibility.DataContext;
using Sitecore.Linqpad.AssemblyLoading;
using Sitecore.Linqpad.Controllers;
using Sitecore.Linqpad.Dialogs;
using Sitecore.Linqpad.DriverInitializers;
using Sitecore.Linqpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sitecore.Linqpad.SchemaBuilders;

namespace Sitecore.Linqpad.Driver
{
    public class SitecoreDriver : DynamicDataContextDriver
    {
        public override string Name
        {
            get { return "Sitecore"; }
        }
        public override string Author
        {
            get { return "Adam Conn"; }
        }

        protected virtual ISitecoreConnectionSettings GetCxSettings(IConnectionInfo cxInfo)
        {
            var settings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(cxInfo, settings);
            return settings;
        }
        public override void InitializeContext(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            var settings = GetCxSettings(cxInfo);
            var initType = settings.DriverInitializerType;
            if (initType == null) { return; }
            var initializer = initType.GetInstance<IDriverInitializer>();
            if (initializer == null) { return; }
            initializer.Run(cxInfo, context, executionManager);
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            var settings = GetCxSettings(cxInfo);
            var schemaBuilderType = settings.SchemaBuilderType;
            if (schemaBuilderType == null) { return null; }
            var builder = schemaBuilderType.GetInstance<ISchemaBuilder>();
            if (builder == null) { return null; }
            var paths = new List<string>() {Path.Combine(settings.WebRootPath, "bin")};
            using (var context = new AssemblyLoadingContext(paths))
            {
                return builder.BuildAssembly(cxInfo, assemblyToBuild, ref nameSpace, ref typeName);    
            }
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
        {
            var set = new HashSet<string>(base.GetAssembliesToAdd(cxInfo));
            var settings = GetCxSettings(cxInfo);
            var paths = CxSettingsPathsHelper.Current.GetAssemblyLocations(settings);
            set.UnionWith(paths);
            return set.ToList();
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
        {
            var set = new HashSet<string>(base.GetNamespacesToAdd(cxInfo));
            var settings = GetCxSettings(cxInfo);
            set.UnionWith(settings.NamespacesToAdd);
            return set.ToList();
        }

        public override string GetConnectionDescription(IConnectionInfo cxInfo)
        {
            var settings = GetCxSettings(cxInfo);
            var selectedType = settings.SearchResultType;
            string description = null;
            if (selectedType != null)
            {
                var type = selectedType.GetSelectedType();
                if (type != null)
                {
                    description = string.Format("{0} [{1}]", settings.ClientUrl, type.Name);
                }
            }
            if (string.IsNullOrEmpty(description))
            {
                description = settings.ClientUrl;
            }
            return description;
        }

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
        {
            var view = new ConnectionDialog();
            if (!view.IsInitialized)
            {
                view.InitializeComponent();
            }
            try
            {
                var settings = GetCxSettings(cxInfo);

                var driverSettings = new SitecoreDriverSettings() { CxInfo = cxInfo, CxSettings = settings, SettingsMapper = new DriverDataCxSettingsMapper() };
                view.Model = driverSettings;

                var controller = new DriverSettingsController(view);
                controller.LoadView(driverSettings);
                view.SaveViewToModelCallback = controller.SaveView;

                var result = view.ShowDialog();
                return (result == true);
            }
            catch (Exception ex)
            {
                var message = string.Format("An exception was thrown when trying to load the dialog.\n\n" +
                    "You might need to manually edit the connections file located in {0}.\n\n" +
                    "=====================================\n" +
                    "{1}\n{2}", 
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LINQPad"),
                    ex.Message, 
                    ex.StackTrace);
                MessageBox.Show(view, message, "Connection Dialog", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
