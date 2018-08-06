using System;

namespace Deveel.Workflows.Actors
{
    public class SystemUser : IActor
    {
        private SystemUser(string name)
        {
            Name = name;
        }

        static SystemUser()
        {
            Current = new SystemUser(Environment.UserName);
        }

        public string Name { get; }

        public static SystemUser Current { get; }
    }
}
