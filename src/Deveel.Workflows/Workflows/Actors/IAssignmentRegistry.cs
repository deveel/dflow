using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Actors
{
    public interface IAssignmentRegistry
    {
        Task CreateAssignmentAsync(UserAssignment assignment);

        Task<IList<UserAssignment>> FindByUserAsync(string userName);
    }
}
