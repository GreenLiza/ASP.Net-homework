using GoodNewsAggregator.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByUsername(string username);
        Task<int> GetUserRoleId(string username);
        Task<bool> IsUserWithUsernameExistAsync(string username);
        Task<bool> IsUserWithEmailExistAsync(string email);
        Task<bool> IsUserExistAsync(string email, string username);
        Task<bool> IsPasswordCorrectAsync(string email, string password);
        Task<UserDto?> RegisterUserAsync(string email, string username, string password);
        Task<UserDto?> LogInUserAsync(string username, string password);
    }
}