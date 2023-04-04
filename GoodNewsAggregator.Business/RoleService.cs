using AutoMapper;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using Microsoft.EntityFrameworkCore;


namespace GoodNewsAggregator.Business
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetRoleIdByName(string name)
        {
            var roleId = await _unitOfWork.Roles.FindBy(role => role.Name.Equals(name))
                .Select(role => role.Id)
                .FirstOrDefaultAsync();

            return roleId;
        }
    }
}
