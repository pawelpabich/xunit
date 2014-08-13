using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xunit.Sdk
{
    public class XunitTestInvokerCustom : XunitTestInvoker
    {
      
        public XunitTestInvokerCustom(IXunitTestCase testCase,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            string displayName,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, displayName, aggregator, cancellationTokenSource)
        {
            //This method would need to be marked as virtual
            public override Task<decimal> RunAsync()
            {
                
            }
        }

     
}