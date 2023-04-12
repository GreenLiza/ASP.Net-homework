using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.Business
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleService = roleService;
        }

        public async Task<bool> IsUserWithUsernameExistAsync(string username)
        {
            var userByUsername = await _unitOfWork.Users.FindBy(user => user.Username.Equals(username)).FirstOrDefaultAsync();
            return userByUsername != null;
        }
        public async Task<bool> IsUserWithEmailExistAsync(string email)
        {
            var userByUsername = await _unitOfWork.Users.FindBy(user => user.Email.Equals(email)).FirstOrDefaultAsync();
            return userByUsername != null;
        }
        public async Task<bool> IsUserExistAsync(string email, string username)
        {
            var isUserByEmail = await IsUserWithEmailExistAsync(email);
            var isUserByUsername = await IsUserWithUsernameExistAsync(username);
            var exists = isUserByEmail || isUserByUsername;
            return exists;
        }

        public async Task<UserDto?> RegisterUserAsync(string email, string username, string password)
        {
            if (!await IsUserExistAsync(email, username))
            {
                var userRoleId = await _roleService.GetRoleIdByName("User");

                var user = new User()
                {
                    Email = email,
                    Username = username,
                    Password = GetPasswordHash(password),
                    RoleId = userRoleId
                };

                var userEntry = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<UserDto>(await _unitOfWork.Users.GetByIdAsync(userEntry.Entity.Id));
            }

            return null;
        }

        public async Task<UserDto?> LogInUserAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);
            if (user != null && await IsPasswordCorrectAsync(username, password))
            {
                return _mapper.Map<UserDto>(user);
            }
            return null;
        }

        private string GetPasswordHash(string password)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        public async Task<bool> IsPasswordCorrectAsync(string username, string password)
        {
            if (await IsUserWithUsernameExistAsync(username))
            {
                var passwordHash = GetPasswordHash(password);
                var user = await _unitOfWork.Users.GetAsQueryable().AsNoTracking()
                    .FirstOrDefaultAsync(user => user.Username.Equals(username)
                           && user.Password.Equals(passwordHash));

                return user != null;
            }
            else
            {
                throw new ArgumentException("User with that username does not exist", nameof(username));
            }
        }

        public async Task<int> GetUserRoleId(string username)
        {
            var roleId = await _unitOfWork.Users.FindBy(user => user.Username.Equals(username))
            .Select(user => user.RoleId)
            .FirstOrDefaultAsync();

            return roleId;
        }

        public async Task<UserDto?> GetUserByUsername(string username)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);
            if (user != null)
            {
                var userDto = _mapper.Map<UserDto>(user);
                return userDto;
            }
            return null;
        }
    }
}
