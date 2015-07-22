using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.Server
{
    public class SitecoreConnectionManager : ISitecoreConnectionManager
    {
        public SitecoreConnectionManager(ISitecoreConnectionSettings model) : this(model, null)
        {
        }

        public SitecoreConnectionManager(ISitecoreConnectionSettings model, CookieAwareWebClient client)
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            this.Model = model;
            if (client == null)
            {
                client = new CookieAwareWebClient(null);
            }
            this.Client = client;
            this.BaseUrl = new Uri(model.ClientUrl, UriKind.Absolute);
            this.LoginUrl = new Uri(this.BaseUrl + "/admin/login.aspx");
            this.GetConfigUrl = new Uri(this.BaseUrl + "/admin/showconfig.aspx");
            this.GetVersionUrl = new Uri(this.BaseUrl + "/shell/sitecore.version.xml");
        }

        public virtual ServerResponse<XElement> GetSitecoreConfig()
        {
            var response = new ServerResponse<XElement>();
            try
            {
                if (this.DoLogin())
                {
                    var document = XDocument.Parse(this.Client.DownloadString(this.GetConfigUrl));
                    this.DoLogout();
                    response.Data = document.Root;
                }
            }
            catch (Exception ex)
            {
                response.AddException(ex);
            }
            return response;
        }

        public virtual ServerResponse<Version> GetSitecoreVersion()
        {
            var response = new ServerResponse<Version>();
            try
            {
                var xml = this.Client.DownloadString(this.GetVersionUrl);
                var element = XElement.Parse(xml);
                var element2 = element.Element("version");
                var maj = 0;
                int.TryParse(element2.Element("major").Value, out maj);
                var min = 0;
                int.TryParse(element2.Element("minor").Value, out min);
                var build = 0;
                int.TryParse(element2.Element("build").Value, out build);
                var rev = 0;
                int.TryParse(element2.Element("revision").Value, out rev);
                response.Data = new Version(maj, min, build, rev);
            }
            catch (WebException ex)
            {
                response.AddException(ex);
            }
            return response;
        }

        public virtual ServerResponse<bool> TestConnection()
        {
            var response = new ServerResponse<bool>();
            string message = null;
            try
            {
                response.Data = this.DoLogin();
                if (response.Data)
                {
                    this.DoLogout();
                }
            }
            catch (Exception ex)
            {
                response.AddException(ex);
            }
            return response;
        }

        protected virtual bool DoLogin()
        {
            var fieldsFromLoginForm = this.GetFieldsFromLoginForm();
            if ((fieldsFromLoginForm == null) || (fieldsFromLoginForm.Count == 0))
            {
                return false;
            }
            if (fieldsFromLoginForm["LoginTextBox"] == null)
            {
                fieldsFromLoginForm.Add("LoginTextBox", this.Model.Username);
            }
            else
            {
                fieldsFromLoginForm.Set("LoginTextBox", this.Model.Username);
            }
            if (fieldsFromLoginForm["PasswordTextBox"] == null)
            {
                fieldsFromLoginForm.Add("PasswordTextBox", this.Model.Password);
            }
            else
            {
                fieldsFromLoginForm.Set("PasswordTextBox", this.Model.Password);
            }
            var buffer = this.Client.UploadValues(this.LoginUrl, fieldsFromLoginForm);
            if (this.Client.Cookies.GetCookies(this.BaseUrl)[".ASPXAUTH"] == null)
            {
                throw new AuthenticationException("Login failed. Please check your username and password.");
            }
            return true;
        }

        protected virtual void DoLogout()
        {
            if (this.Client != null)
            {
                var cookies = this.Client.Cookies.GetCookies(this.BaseUrl);
                foreach (Cookie cookie in cookies)
                {
                    cookie.Expired = true;
                }
            }
        }

        protected virtual NameValueCollection GetFieldsFromLoginForm()
        {
            var values = new NameValueCollection();
            var html = this.Client.DownloadString(this.LoginUrl);
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var elementbyId = document.GetElementbyId("LoginForm");
            foreach (var node2 in (IEnumerable<HtmlNode>)elementbyId.SelectNodes("//input"))
            {
                var attribute = node2.Attributes["value"];
                var attribute2 = node2.Attributes["name"];
                if (attribute != null)
                {
                    values.Add(attribute2.Value, attribute.Value);
                }
            }
            return values;
        }

        protected Uri BaseUrl { get; private set; }

        protected CookieAwareWebClient Client { get; private set; }

        protected ISitecoreConnectionSettings Model { get; private set; }

        protected Uri GetConfigUrl { get; private set; }

        protected Uri GetVersionUrl { get; private set; }

        protected Uri LoginUrl { get; private set; }
    }
}
