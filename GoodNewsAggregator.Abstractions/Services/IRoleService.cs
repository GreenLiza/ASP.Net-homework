using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions.Services
{
    public interface IRoleService
    {
        Task<int> GetRoleIdByName(string name);
        Task<string> GetRoleNameById(int id);
    }
}
