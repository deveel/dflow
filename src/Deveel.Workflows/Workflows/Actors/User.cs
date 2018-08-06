using System.Collections.Generic;

namespace Deveel.Workflows.Actors
{
    public sealed class User : IActor
    {
        public User(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
