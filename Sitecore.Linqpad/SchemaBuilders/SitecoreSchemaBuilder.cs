using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using LINQPad.Extensibility.DataContext;
using Microsoft.CSharp;
using Sitecore.Linqpad.Models;

namespace Sitecore.Linqpad.SchemaBuilders
{
    public class SitecoreSchemaBuilder : ISchemaBuilder
    {
        protected IList<string> KnownPropertyNames;

        public virtual List<ExplorerItem> BuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            KnownPropertyNames = new List<string>();

            var unit = new CodeCompileUnit();
            var namespace2 = new CodeNamespace(nameSpace);
            namespace2.Imports.Add(new CodeNamespaceImport("System"));
            namespace2.Imports.Add(new CodeNamespaceImport("System.Linq"));
            namespace2.Imports.Add(new CodeNamespaceImport("Sitecore.ContentSearch"));
            namespace2.Imports.Add(new CodeNamespaceImport("Sitecore.ContentSearch.SearchTypes"));

            var settings = new SitecoreConnectionSettings();
            var mapper = new DriverDataCxSettingsMapper();
            mapper.Read(cxInfo, settings);

            var selectedType = settings.SearchResultType.GetSelectedType();
            namespace2.Imports.Add(new CodeNamespaceImport(selectedType.Namespace));
            var declaration = new CodeTypeDeclaration(typeName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            namespace2.Types.Add(declaration);
            unit.Namespaces.Add(namespace2);
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            this.AddConstructorCode(constructor, settings);
            declaration.Members.Add(constructor);
            var indexNames = this.GetIndexNames(cxInfo);
            var list = new List<ExplorerItem>();
            foreach (var str in indexNames)
            {
                this.AddIndexAsProperty(str, selectedType, declaration);
                var item = new ExplorerItem(str, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = false
                };
                item.DragText = this.GetDragText(item, settings);
                list.Add(item);
            }
            var provider = new CSharpCodeProvider();
            var options = new CompilerParameters();
            var assemblyFilesToReference = this.GetAssemblyFilesToReference(settings);
            foreach (var str2 in assemblyFilesToReference)
            {
                options.ReferencedAssemblies.Add(str2);
            }
            options.GenerateInMemory = true;
            options.OutputAssembly = assemblyToBuild.CodeBase;
            var results = provider.CompileAssemblyFromDom(options, new CodeCompileUnit[] { unit });
            if (results.Errors.Count > 0)
            {
                throw new Exception(string.Concat(new object[] { "Cannot compile typed context: ", results.Errors[0].ErrorText, " (line ", results.Errors[0].Line, ")" }));
            }
            return list;
        }

        protected virtual void AddAssemblyFiles(Assembly assembly, HashSet<Assembly> assemblies)
        {
            if ((assembly != null) && !assemblies.Contains(assembly))
            {
                assemblies.Add(assembly);
                foreach (var name in assembly.GetReferencedAssemblies())
                {
                    try
                    {
                        this.AddAssemblyFiles(Assembly.Load(name), assemblies);
                    }
                    catch (FileNotFoundException ex)
                    {
                        //there's no good way to report error messages...
                    }
                }
            }
        }
        protected virtual void AddConstructorCode(CodeConstructor constructor, ISitecoreConnectionSettings settings)
        {
            var contextDb = settings.ContextDatabaseName;
            if (string.IsNullOrEmpty(contextDb))
            {
                contextDb = "master";
            }
            var targetObject = new CodeTypeReferenceExpression(new CodeTypeReference("Sitecore.Data.Database"));
            var right = new CodeMethodInvokeExpression(targetObject, "GetDatabase", new CodeExpression[] { new CodePrimitiveExpression(contextDb) });
            var expression3 = new CodeTypeReferenceExpression(new CodeTypeReference("Sitecore.Context"));
            var left = new CodePropertyReferenceExpression(expression3, "Database");
            var statement = new CodeAssignStatement(left, right);
            constructor.Statements.Add(statement);
        }
        protected virtual void AddIndexAsProperty(string name, Type type, CodeTypeDeclaration targetClass)
        {
            string str = string.Format("{0}.{1}", type.Namespace, type.Name);
            CodeMemberProperty property = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = this.GetCleanPropertyName(name),
                HasGet = true,
                Type = new CodeTypeReference(string.Format("IQueryable<{0}>", str))
            };
            CodeMethodInvokeExpression initExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("ContentSearchManager")), "GetIndex"), new CodeExpression[] { new CodePrimitiveExpression(property.Name) });
            CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement(new CodeTypeReference("ISearchIndex"), "index", initExpression);
            property.GetStatements.Add(statement);
            CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression("index");
            CodeMethodInvokeExpression expression3 = new CodeMethodInvokeExpression(targetObject, "CreateSearchContext", new CodeExpression[0]);
            CodeVariableReferenceExpression expression4 = new CodeVariableReferenceExpression("context");
            CodeVariableDeclarationStatement statement2 = new CodeVariableDeclarationStatement(new CodeTypeReference("IProviderSearchContext"), expression4.VariableName, expression3);
            property.GetStatements.Add(statement2);
            CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(expression4, "GetQueryable", new CodeTypeReference[] { new CodeTypeReference(str) });
            CodeMethodInvokeExpression expression = new CodeMethodInvokeExpression(method, new CodeExpression[0]);
            CodeMethodReturnStatement statement3 = new CodeMethodReturnStatement(expression);
            property.GetStatements.Add(statement3);
            targetClass.Members.Add(property);
        }

        protected string GetCleanPropertyName(string name)
        {
            name = Regex.Replace(name, "[^a-zA-Z0-9_]+", "_");
            if (Regex.IsMatch(name, "^[0-9]"))
            {
                name = "index_" + name;
            }

            string newName = name;

            for(int i = 1; KnownPropertyNames.Contains(newName); i++)
            {
                newName = name + i;
            }

            KnownPropertyNames.Add(newName);

            return newName;
        }

        protected virtual IQueryable<string> GetAssemblyFilesToReference(ISitecoreConnectionSettings settings)
        {
            var list = new List<Assembly> 
            {
                Assembly.Load(new AssemblyName("Sitecore.ContentSearch")),
                Assembly.Load(new AssemblyName("Sitecore.ContentSearch.Linq")),
                typeof(IQueryable).Assembly
            };
            var selectedType = settings.SearchResultType.GetSelectedType();
            if (selectedType != null)
            {
                list.Add(selectedType.Assembly);
            }
            var assemblies = new HashSet<Assembly>();
            foreach (var assembly in list)
            {
                this.AddAssemblyFiles(assembly, assemblies);
            }
            return (from a in assemblies.AsQueryable<Assembly>() select a.Location);
        }
        private const string IndexFieldAttributeTypeName = "Sitecore.ContentSearch.IndexFieldAttribute, Sitecore.ContentSearch.Linq";
        private const string IndexFieldAttributePropertyName = "IndexFieldName";
        protected virtual string GetPropertyNameForLinqExpression(Type type, string searchFieldName)
        {
            if (type == null) {  throw new ArgumentNullException("type"); }
            if (string.IsNullOrEmpty(searchFieldName)) {  throw new ArgumentNullException("searchFieldName"); }
            var attributeType = Type.GetType(IndexFieldAttributeTypeName);
            if (attributeType == null) { throw new TypeLoadException(string.Format("Unable to find the type '{0}'", IndexFieldAttributeTypeName)); }
            var attributeProperty = attributeType.GetProperty(IndexFieldAttributePropertyName);
            if (attributeProperty == null) { throw new MissingMemberException(string.Format("Unable to find the property '{0}'", IndexFieldAttributePropertyName)); }
            foreach (var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttributes(attributeType, true)
                    .Where(a => a.GetType() == attributeType)
                    .Where(a => (attributeProperty.GetValue(a) as string) == searchFieldName)
                    .FirstOrDefault();
                if (attribute != null)
                {
                    return property.Name;
                }
            }
            return null;
        }
        protected virtual string GetDragText(ExplorerItem item, ISitecoreConnectionSettings settings)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("var index = ContentSearchManager.GetIndex(\"{0}\");", item.Text));
            builder.AppendLine("using (var context = index.CreateSearchContext())");
            builder.AppendLine("{");
            var type = settings.SearchResultType.GetSelectedType();
            var typeName = settings.NamespacesToAdd.Contains(type.Namespace) ? type.Name : type.FullName;
            builder.AppendLine(string.Format("\tcontext.GetQueryable<{0}>()", typeName));
            var whereClauses = new HashSet<string>();
            var selectAssignments = new HashSet<string>();
            var nameProperty = GetPropertyNameForLinqExpression(type, "_name");
            var languageProperty = GetPropertyNameForLinqExpression(type, "_language");
            var itemIdProperty = GetPropertyNameForLinqExpression(type, "_group");
            if (! string.IsNullOrEmpty(nameProperty))
            {
                whereClauses.Add(string.Format("item.{0} == \"Home\"", nameProperty));
                selectAssignments.Add(string.Format("Name = item.{0}", nameProperty));
            }
            if (! string.IsNullOrEmpty(languageProperty))
            {
                whereClauses.Add(string.Format("item.{0} == \"en\"", languageProperty));
            }
            if (! string.IsNullOrEmpty(itemIdProperty))
            {
                selectAssignments.Add(string.Format("Id = item.{0}.ToString()", itemIdProperty));
            }
            foreach (var clause in whereClauses)
            {
                builder.AppendFormat("\t\t.Where(item => {0})\n", clause);
            }
            builder.AppendFormat("\t\t\t\t.Select(item => new {{{0}}} )\n", string.Join(", ", selectAssignments.ToArray()));
            builder.AppendLine("\t\t\t\t\t.Dump();");
            builder.AppendLine("}");
            return builder.ToString();
        }
        protected virtual IEnumerable<string> GetIndexNames(IConnectionInfo cxInfo)
        {
            if (! File.Exists(cxInfo.AppConfigPath)) { throw new FileNotFoundException();}
            var doc = XDocument.Load(cxInfo.AppConfigPath);
            var enumerable = doc.XPathSelectElements("/configuration/sitecore/contentSearch/configuration/indexes/index");
            var list = new List<string>();
            foreach (var element in enumerable)
            {
                var attribute = element.Attribute("id");
                if (attribute != null)
                {
                    list.Add(attribute.Value);
                }
            }
            return list;
        }

    }
}
