using System;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.BusinessRules;
using Deveel.Workflows.Infrastructure;
using Deveel.Workflows.Scripts;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent.Dsl;
using Xunit;

namespace Deveel.Workflows
{
    public class SimpleFlowsTests
    {
        [Fact]
        public async Task SingleActivityFlow()
        {
            var workflow = new Process("test1");
            workflow.Sequence.Add(new ServiceTask("task1", c => {}));

            var services = new ServiceCollection();
            services.AddSingleton<IExecutionRegistry, InMemoryExecutionRegistry>();

            var provider = services.BuildServiceProvider().CreateScope();

            var context = new ExecutionContext(new User("me"), provider);

            await workflow.ExecuteAsync(context);
        }

        [Fact]
        public async Task ForkJoinActivities()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IExecutionRegistry, InMemoryExecutionRegistry>();
            services.AddSingleton<IVariableRegistry, InMemoryVariableRegistry>();

            var provider = services.BuildServiceProvider().CreateScope();

            var context = new ExecutionContext(new User("me"), provider);

            var processId = "proc1";
            var process = new Process(processId);
            process.Sequence.Add(new ManualTask("some manual"));
            process.Sequence.Add(new ForkGateway("fork")
            {
                Flows =
                {
                    new SimpleGatewayFlow(new ServiceTask("addOne", async c => await c.SetVariableAsync("one", 2))),
                    new SimpleGatewayFlow(new ServiceTask("addTwo", async c => await c.SetVariableAsync("two", 3)))
                }
            });
            process.Sequence.Add(new JoinGateway("join", async c =>
            {
                var arg1 = (int)(await c.FindVariableAsync("one"));
                var arg2 = (int) (await c.FindVariableAsync("two"));

                await c.SetVariableAsync("sum", arg1 + arg2);
            })
            {
                Flows =
                {
                    new InGatewayFlow("addOne"),
                    new InGatewayFlow("addTwo")
                }
            });

            await process.ExecuteAsync(context);

            var sum = await context.FindVariableAsync("sum");

            Assert.NotNull(sum);
            Assert.IsType<int>(sum);
            Assert.Equal(5, sum);
        }

        [Fact]
        public async Task WithCSharpScriptTask()
        {
            const string script = @"
int b;
for(int i = 0; i < a; i++) {
b = i+1;
}";
            var services = new ServiceCollection();
            services.AddSingleton<IExecutionRegistry, InMemoryExecutionRegistry>();
            services.AddSingleton<IVariableRegistry, InMemoryVariableRegistry>();

            var provider = services.BuildServiceProvider().CreateScope();

            var context = new ExecutionContext(new User("me"), provider);
            await context.SetVariableAsync("a", 2);

            var processId = "proc1";
            var process = new Process(processId);
            process.Sequence.Add(new ManualTask("some manual"));
            process.Sequence.Add(new ScriptTask("script", script, new CSharpScriptEngine()));

            await process.ExecuteAsync(context);
        }

        [Fact]
        public async Task WithBusinessRuleTask()
        {
            var ruleProvider = new NRulesRulesProvider();
            ruleProvider.AddRules("bizRule1", typeof(BusinessRule));

            var services = new ServiceCollection();
            services.AddSingleton<IExecutionRegistry, InMemoryExecutionRegistry>();
            services.AddSingleton<IVariableRegistry, InMemoryVariableRegistry>();
            services.AddSingleton<IRulesProvider>(ruleProvider);

            var provider = services.BuildServiceProvider().CreateScope();

            var context = new ExecutionContext(new User("me"), provider);
            await context.SetVariableAsync("a", 2);

            var processId = "proc1";
            var process = new Process(processId);
            process.Sequence.Add(new ManualTask("some manual"));
            process.Sequence.Add(new BusinessRuleTask("bizTask", "bizRule1"));

            await process.ExecuteAsync(context);

        }

        #region BusinessRule

        class BusinessRule : Rule
        {
            public override void Define()
            {
                Variable a = null;

                When()
                    .Match(() => a)
                    .Exists<Variable>(v => v.Name == "a");

                Then()
                    .Do(c => Console.Out.WriteLine(a.Value));

            }
        }

        #endregion
    }
}
