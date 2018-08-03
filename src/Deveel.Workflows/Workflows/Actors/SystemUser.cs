using System;

namespace Deveel.Workflows.Actors
{
    public class SystemUser : IActor
    {
        string IActor.Name => "@system";
    }
}
