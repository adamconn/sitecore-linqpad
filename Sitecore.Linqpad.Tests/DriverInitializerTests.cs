using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sitecore.Linqpad.DriverInitializers;

namespace Sitecore.Linqpad.Tests
{
    [TestClass]
    public class DriverInitializerTests
    {
        [TestMethod, TestCategory("DriverInitializer class"), ExpectedException(typeof(ArgumentNullException))]
        public void MethodRunnerEmptyTypeName()
        {
            var runner = new MethodRunner("", "");
            runner.Run();
        }
        [TestMethod, TestCategory("DriverInitializer class"), ExpectedException(typeof(ArgumentNullException))]
        public void MethodRunnerEmptyMethodName()
        {
            var runner = new MethodRunner(typeof(string).AssemblyQualifiedName, "");
            runner.Run();
        }
        [TestMethod, TestCategory("DriverInitializer class"), ExpectedException(typeof(TypeLoadException))]
        public void MethodRunnerInvalidTypeName()
        {
            var runner = new MethodRunner("xxx", "xxx");
            runner.Run();
        }
        [TestMethod, TestCategory("DriverInitializer class"), ExpectedException(typeof(MissingMethodException))]
        public void MethodRunnerInvalidMethodName()
        {
            var runner = new MethodRunner(typeof(string).AssemblyQualifiedName, "xxx");
            runner.Run();
        }
        [TestMethod, TestCategory("DriverInitializer class")]
        public void MethodRunnerStringBuilderClear()
        {
            var runner = new MethodRunner(typeof(StringBuilder).AssemblyQualifiedName, "Clear");
            runner.Run();
        }
    }
}
