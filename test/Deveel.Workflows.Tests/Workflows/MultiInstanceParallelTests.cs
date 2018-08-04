using System;
using Deveel.Workflows.Expressions;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Deveel.Workflows
{
    public class MultiInstanceParallelTests : TaskTestsBase
    {
        public MultiInstanceParallelTests() : base(Guid.NewGuid().ToString())
        {
        }

        protected override void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IVariableRegistry, InMemoryVariableRegistry>();
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            sequence.Add(new MultiInstanceActivity(new ServiceTask("task1", c => value++), FlowExpression.Constant(3),
                MultiInstanceType.Parallel));
        }

        private int value;

        [Fact]
        public async void ExecuteMultiInstances()
        {
            await Process.RunAsync(Context);

            Assert.Equal(3, value);
        }
    }
}
