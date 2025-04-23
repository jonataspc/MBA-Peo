using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peo.Identity.Application.Endpoints.Responses
{
    public record LoginResponse(string Token, Guid UserId);
}
