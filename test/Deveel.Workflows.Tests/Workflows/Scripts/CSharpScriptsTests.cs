using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

using Deveel.Workflows.Variables;

namespace Deveel.Workflows.Scripts
{
    public class CSharpScriptsTests
    {
        private IScriptEngine engine;
        private NodeContext context;

        public CSharpScriptsTests()
        {
            engine = new CSharpScriptEngine();
            context = CreateContext();
        }

        private NodeContext CreateContext()
        {
            var provider = new ServiceCollection()
                .AddSingleton<IVariableRegistry, InMemoryVariableRegistry>()
                .BuildServiceProvider();
            var system = new SystemContext(provider);

            var process = new Process(new ProcessInfo("test", Guid.NewGuid().ToString()));
            var processContext = system.CreateContext(process);

            return processContext.CreateContext(new ManualTask("task"));
        }

        [Fact]
        public async void ReferenceVars()
        {
            const string code =
                @"for(int i = 0; i < vars.a; i++) {
context.SetVariableAsync(""c"", i).Wait();
}";

            var executor = engine.CreateExecutor(code, ScriptInfo.Generate(context));

            var globals = new ScriptContext(context);
            await globals.SetVariableAsync("a", 2);

            var result = await executor.ExecuteAsync(globals);

            Assert.NotNull(result);

            var variable = await context.FindVariableAsync("c");

            Assert.Equal(1, variable);
        }

        [Fact]
        public async void SetVars()
        {
            const string code =
    @"for(int i = 0; i < vars.a; i++) {
vars.b = i;
}";

            var executor = engine.CreateExecutor(code, ScriptInfo.Generate(context));

            var globals = new ScriptContext(context);
            await globals.SetVariableAsync("a", 2);
            await globals.SetVariableAsync("b", -2);

            var result = await executor.ExecuteAsync(globals);

            Assert.NotNull(result);

            var variable = await context.FindVariableAsync("b");

            Assert.Equal(1, variable);


        }
    }
}
