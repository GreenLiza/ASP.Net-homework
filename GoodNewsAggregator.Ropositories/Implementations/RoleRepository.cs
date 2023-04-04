using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Repositories.Implementations
{
    public class RoleRepository : Repository<Role>
    {
        public RoleRepository(NewsAggregatorContext dbContext) : base(dbContext)
        {
        }

    }
}
