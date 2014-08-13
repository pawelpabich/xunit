using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    public class XunitTestFrameworkExecutorCustom : XunitTestFrameworkExecutor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestFrameworkExecutor"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the test assembly.</param>
        /// <param name="sourceInformationProvider">The source line number information provider.</param>
        public XunitTestFrameworkExecutorCustom(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider)
            : base(assemblyName, sourceInformationProvider)
        {
        }


        /// <inheritdoc/>
        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink messageSink, ITestFrameworkOptions executionOptions)
        {
            using (var assemblyRunner = new XunitTestAssemblyRunnerCustom(TestAssembly, testCases, messageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }
    }
}