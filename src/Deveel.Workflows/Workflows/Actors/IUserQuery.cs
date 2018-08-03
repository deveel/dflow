using System.Threading.Tasks;

namespace Deveel.Workflows.Actors
{
    public interface IUserQuery
    {
        Task<User> FindUserAsync(string userName);
    }
}
