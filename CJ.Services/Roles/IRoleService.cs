using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Roles
{
    public interface IRoleService
    {
        RoleDto GetRole(int id);
    }
}
