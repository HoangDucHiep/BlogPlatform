using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Application.Abstractions.Authentication;
public interface IUserContext
{
    Guid UserId();
    string IdentityId();
}
