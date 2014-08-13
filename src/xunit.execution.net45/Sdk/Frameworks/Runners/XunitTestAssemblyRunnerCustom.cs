using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    public class XunitTestAssemblyRunnerCustom : XunitTestAssemblyRunner
    {
        ITestCollectionOrderer testCollectionOrderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="testAssembly">The assembly that contains the tests to be run.</param>
        /// <param name="testCases">The test cases to be run.</param>
        /// <param name="messageSink">The message sink to report run status to.</param>
        /// <param name="executionOptions">The user's requested execution options.</param>
        public XunitTestAssemblyRunnerCustom(ITestAssembly testAssembly,
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink messageSink,
            ITestFrameworkOptions executionOptions)
            : base(testAssembly, testCases, messageSink, executionOptions) { }


        /// <inheritdoc/>
        protected override async Task<RunSummary> RunTestCollectionsAsync(IMessageBus messageBus, CancellationTokenSource cancellationTokenSource)
        {
            if (disableParallelization)
                return await base.RunTestCollectionsAsync(messageBus, cancellationTokenSource).ConfigureAwait(false);

            var testCollections = TestCases.GroupBy(tc => tc.TestMethod.TestClass.TestCollection, TestCollectionComparer.Instance);
            testCollections = testCollectionOrderer.OrderTestCollections(testCollections).ToArray();

             var tasks  = testCollections.Select(collectionGroup => Task.Factory.StartNew(() => RunTestCollectionAsync(messageBus, collectionGroup.Key, collectionGroup, cancellationTokenSource),
                    cancellationTokenSource.Token,
                    TaskCreationOptions.DenyChildAttach,
                    scheduler))
                .ToArray();

            var summaries = await Task.WhenAll(tasks.Select(t => t.Unwrap())).ConfigureAwait(false);

            return new RunSummary()
            {
                Total = summaries.Sum(s => s.Total),
                Failed = summaries.Sum(s => s.Failed),
                Skipped = summaries.Sum(s => s.Skipped)
            };
        }

        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
        {
            return new XunitTestCollectionRunnerCustom(testCollection, testCases, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();
        }
    }

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

     [AttributeUsage(AttributeTargets.Assembly, Inherited = true, AllowMultiple = false)]
     public sealed class TestCollectionOrdererAttribute : Attribute
     {
         /// <summary>
         /// Initializes a new instance of the <see cref="TestCollectionOrdererAttribute"/> class.
         /// </summary>
         /// <param name="ordererTypeName">The type name of the orderer class (that implements <see cref="T:Xunit.Sdk.ICollectionOrderer"/>).</param>
         /// <param name="ordererAssemblyName">The assembly that <paramref name="ordererTypeName"/> exists in.</param>
         public TestCollectionOrdererAttribute(string ordererTypeName, string ordererAssemblyName) { }
     }
}