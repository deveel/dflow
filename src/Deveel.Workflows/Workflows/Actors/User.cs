using System.Collections.Generic;

namespace Deveel.Workflows.Actors
{
    public sealed class User : IActor
    {
        public User(string name)
        {
            Name = name;
            EndPoints = new List<IUserEndPoint>();
        }

        public string Name { get; }

        public ICollection<IUserEndPoint> EndPoints { get; }
    }
}
