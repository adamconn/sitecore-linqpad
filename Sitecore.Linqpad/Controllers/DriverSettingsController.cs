using Sitecore.Linqpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Controllers
{
    public class DriverSettingsController : IDriverSettingsController
    {
        public DriverSettingsController(ISitecoreConnectionSettings view)
        {
            if (view == null) { throw new ArgumentNullException("view"); }
            this.View = view;
        }
        public ISitecoreConnectionSettings View { get; private set; }

        public virtual void LoadView(SitecoreDriverSettings model)
        {
            CopySettings(model.CxSettings, this.View);
        }
        public virtual void SaveView(SitecoreDriverSettings model)
        {
            CopySettings(this.View, model.CxSettings);
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Save(model.CxInfo, model.CxSettings);
        }

        protected virtual void CopySettings(ISitecoreConnectionSettings from, ISitecoreConnectionSettings to)
        {
            //
            //basic settings
            to.ClientUrl = from.ClientUrl;
            to.Username = from.Username;
            to.Password = from.Password;
            to.WebRootPath = from.WebRootPath;
            to.ContextDatabaseName = from.ContextDatabaseName;
            //
            //advanced settings
            to.NamespacesToAdd = (from.NamespacesToAdd == null) ? null : new HashSet<string>(from.NamespacesToAdd.Select(ns => ns).ToList());
            to.SearchResultType = (from.SearchResultType == null) ? null : new SelectedType(from.SearchResultType.GetSelectedType()){GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSearchResultItem};
            to.AppConfigReaderType = (from.AppConfigReaderType == null) ? null : new SelectedType(from.AppConfigReaderType.GetSelectedType()){GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForAppConfigReaderType};
            to.SchemaBuilderType = (from.SchemaBuilderType == null) ? null : new SelectedType(from.SchemaBuilderType.GetSelectedType()){GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForSchemaBuilderType};
            to.DriverInitializerType = (from.DriverInitializerType == null) ? null : new SelectedType(from.DriverInitializerType.GetSelectedType()){GetValidTypesDelegate = DefaultValuesForCxSettings.Current.GetValidTypesForDriverInitializerType};
        }
    }
}
