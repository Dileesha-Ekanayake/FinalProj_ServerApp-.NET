using System.Security.Claims;

namespace ServerApp.Security
{
    public class IdentityUser : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IList<Claim> Claims { get; set; }

        public IEnumerable<Claim> GenerateClaims()
        {
            foreach (var claim in Claims)
            {
                yield return claim;
            }
        }
    }

    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        IEnumerable<Claim> GenerateClaims();
    }
}
