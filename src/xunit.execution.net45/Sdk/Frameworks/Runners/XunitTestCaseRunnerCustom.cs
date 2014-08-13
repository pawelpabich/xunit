using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xunit.Sdk
{
    public class XunitTestCaseRunnerCustom : XunitTestCaseRunner
    {
        readonly List<BeforeAfterTestAttribute> beforeAfterAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestCaseRunner"/> class.
        /// </summary>
        /// <param name="testCase">The test case to be run.</param>
        /// <param name="displayName">The display name of the test case.</param>
        /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public XunitTestCaseRunnerCustom(IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
           
        }

      
        protected override Task<RunSummary> RunTestAsync()
        {
            return new XunitTestRunnerCustom(TestCase, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, DisplayName, SkipReason, beforeAfterAttributes, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        }
    }
}