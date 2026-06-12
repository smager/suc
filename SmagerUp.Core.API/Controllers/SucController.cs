using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmagerUp.Core.API.Controllers
{
    public abstract class SucController : ControllerBase
    {
        protected Guid? ClientId
        {
            get
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(claim, out var id))
                    return id;
                return null;
            }
        }

        protected string ClientName => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    }

}
