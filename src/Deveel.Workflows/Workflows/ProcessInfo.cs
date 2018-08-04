using System;

namespace Deveel.Workflows
{
    public sealed class ProcessInfo
    {
        public ProcessInfo(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public string Name { get; set; }
    }
}
