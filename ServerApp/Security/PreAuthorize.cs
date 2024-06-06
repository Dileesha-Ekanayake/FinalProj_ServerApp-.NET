using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ServerApp.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PreAuthorize : TypeFilterAttribute
    {
        public PreAuthorize(string claim)
            : base(typeof(ClaimsRequirementFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class ClaimsRequirementFilter : IAuthorizationFilter
    {
        private readonly string _claim;

        public ClaimsRequirementFilter(string claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorities = context.HttpContext.Items["Authorities"] as string[];

            if (authorities == null || !authorities.Contains(_claim))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
