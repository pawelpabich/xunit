using System.Collections.Generic;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    public class FactDiscovererCustom : IXunitTestCaseDiscoverer
    {
        /// <inheritdoc/>
        public IEnumerable<IXunitTestCase> Discover(ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return new IXunitTestCase[] { new XunitTestCaseCustom(testMethod) };
        }
    }
}