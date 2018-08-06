using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Deveel.Workflows
{
    public class UserTaskTests : TaskTestsBase
    {
        public UserTaskTests() 
            : base("proc1")
        {
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            sequence.Add(new UserTask("task1", "you"));
        }

        protected override void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IAssignmentRegistry>(_ =>
            {
                var registry = new Mock<IAssignmentRegistry>();
                registry.Setup(x => x.AssignAsync(It.Is<UserAssignment>(a => a.TaskId == "task1"), It.IsAny<CancellationToken>()))
                    .Returns<UserAssignment, CancellationToken>(async (assignment, t) =>
                    {
                        await Task.Delay(3000);
                        return new AssignmentResult(true);
                    });
                return registry.Object;
            });

            services.AddSingleton<IUserQuery>(_ =>
            {
                var query = new Mock<IUserQuery>();
                query.Setup(x => x.FindUserAsync(It.IsAny<string>()))
                    .Returns<string>(name => Task.FromResult(new User(name)));
                return query.Object;
            });
        }

        [Fact]
        public async Task YouCompletedTheTask()
        {
            await Process.RunAsync(Context);
        }
    }
}
