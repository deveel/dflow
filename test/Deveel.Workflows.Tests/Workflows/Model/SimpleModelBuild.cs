using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Infrastructure;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Deveel.Workflows.Model
{
    public class SimpleModelBuild
    {
        [Fact]
        public void BuildSimpleProcess()
        {
            var model = new ProcessModel
            {
                Id = "test1",
                Sequence = new ProcessSequenceModel
                {
                    new ManualTaskModel
                    {
                        Id = "task1"
                    },
                    new UserTaskModel
                    {
                        Id = "userTask1",
                        Assignee = "me"
                    }
                }
            };

            var context = new Mock<IExecutionContext>();
            context.Setup(x => x.GetService(It.Is<Type>(type => typeof(IUserQuery).IsAssignableFrom(type)))).Returns(new MockUserQuery());

            var process = model.Build(context.Object);

            Assert.NotNull(process);
            Assert.Equal(model.Id, process.Id);
        }

        [Fact]
        public void BuildWithGateways()
        {
            var model = new ProcessModel
            {
                Id = "proc1",
                Sequence = new ProcessSequenceModel
                {
                    new UserTaskModel
                    {
                        Id = "task1",
                        Assignee = "me"

                    },
                    new ExclusiveGatewayModel
                    {
                        Id = "gw1",
                        Flows = new List<GatewayFlowModel>
                        {
                            new GatewayFlowModel
                            {
                                Activity = new UserTaskModel
                                {
                                    Id = "task2",
                                    Assignee = "you",
                                },
                                Condition = "var1.a == 22"
                            },
                            new GatewayFlowModel
                            {
                                Activity = new UserTaskModel
                                {
                                    Id = "task3",
                                    Assignee = "him",
                                }
                            }
                        }
                    }
                }
            };

            var userQuery = new Mock<IUserQuery>();
            userQuery.Setup(x => x.FindUserAsync(It.IsAny<string>()))
                .Returns<string>(userName => Task.FromResult(new User(userName)));

            var scope = new ServiceCollection()
                .AddSingleton<IUserQuery>(userQuery.Object)
                .AddSingleton<IVariableRegistry, InMemoryVariableRegistry>()
                .BuildServiceProvider()
                .CreateScope();

            var context = new ExecutionContext(new SystemUser(), scope);

            var process = model.Build(context);

            Assert.NotNull(process);
            Assert.Equal(model.Id, process.Id);
        }

        class MockUserQuery : IUserQuery
        {
            public Task<User> FindUserAsync(string userName) => Task.FromResult(new User(userName));
        }
    }
}
