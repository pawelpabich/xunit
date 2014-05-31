using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    /// <summary>
    /// A class implements this interface to participate in ordering test collections
    /// for the test runner. Test collection orderers are applied using the
    /// <see cref="TestCollectionOrdererAttribute"/>, which can be applied at
    /// the assembly level.
    /// </summary>
    public interface ITestCollectionOrderer
    {
        /// <summary>
        /// Orders test collections for execution.
        /// </summary>
        /// <param name="testCollections">The test collections to be ordered.</param>
        /// <returns>The test collections in the order to be run.</returns>
        IEnumerable<IGrouping<ITestCollection, TTestCase>> OrderTestCollections<TTestCase>(IEnumerable<IGrouping<ITestCollection, TTestCase>> testCollections) where TTestCase : ITestCase;
    }
}