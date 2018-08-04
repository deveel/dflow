using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Actors
{
    public interface IAssignmentRegistry
    {
        Task<AssignmentResult> AssignAsync(UserAssignment assignment);
    }
}
