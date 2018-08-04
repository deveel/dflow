using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Events;
using Xunit;

namespace Deveel.Workflows
{
    public class BoundaryEventTests : TaskTestsBase
    {
        private bool timeout;

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
           sequence.Add(new ServiceTask("waitTask", context => Thread.Sleep(2000))
           {
               BoundaryEvents =
               {
                   new BoundaryEvent(new TimerEvent("timer1", new TimerInfo(TimeSpan.FromMilliseconds(200))), 
                       new ServiceTask("timeoutTask", context => timeout = true))
               }
           });
        }

        [Fact]
        public async void TimeoutTask()
        {
            await Process.RunAsync(Context);

            Assert.True(timeout);
        }
    }
}
