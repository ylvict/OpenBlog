using AutoMapper;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Abstracts;
using OpenBlog.Repository.Mongo.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Niusys.Extensions.Storage.Mongo;

namespace OpenBlog.Repository.Mongo
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IAppNoSqlBaseRepository<UserEntity> _userRep;

        public UserRepository(IMapper mapper, IAppNoSqlBaseRepository<UserEntity> userRep)
        {
            _mapper = mapper;
            _userRep = userRep;
        }

        public async Task<string> CreateUserAsync(User user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);
            await _userRep.AddAsync(userEntity);
            return userEntity.Sysid.ToString();
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var userEntity = await GetUserInternal(userId.SafeToObjectId());
            return _mapper.Map<User>(userEntity);
        }

        public async Task<bool> IsSystemAdminInited()
        {
            var filterBuilder = Builders<UserEntity>.Filter;
            var filter = filterBuilder.Where(x => x.UserType == UserType.SystemAdmin);
            var systemAdminUsers = await _userRep.SearchAsync(filter);
            return systemAdminUsers.Any();
        }

        public async Task InitSystemAdminUser(string email, string passwordSalt, string passwordHash)
        {
            var userEntity = new UserEntity()
            {
                Email = email, 
                DisplayName = email, 
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                UserType =  UserType.SystemAdmin,
                CreateTime = DateTime.Now, 
                UpdateTime = DateTime.Now
            };
            await _userRep.AddAsync(userEntity);
        }

        private async Task<UserEntity> GetUserInternal(ObjectId userId)
        {
            return await _userRep.GetByPropertyAsync(x => x.Sysid, userId);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var userEntity = await _userRep.GetByPropertyAsync(x => x.Email, email);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<bool> ValidateLastChanged(string userId, string lastChangeToken)
        {
            if (!DateTime.TryParse(lastChangeToken, out var lastChangedTime))
            {
                return false;
            }

            var userEntity = await GetUserInternal(userId.SafeToObjectId());

            return userEntity.UpdateTime > lastChangedTime;
        }
    }
}