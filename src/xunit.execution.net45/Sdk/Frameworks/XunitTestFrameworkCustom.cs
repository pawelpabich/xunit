using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    /// <summary>
    /// 
    /// </summary>
    public class XunitTestFrameworkCustomX : XunitTestFramework
    {
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new XunitTestFrameworkExecutorCustom(assemblyName, SourceInformationProvider);
        }
    }
}