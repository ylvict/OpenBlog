using AutoMapper;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Abstracts;
using OpenBlog.Repository.Mongo.Entities;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;

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

            if(!userRep.Collection.AsQueryable().Any())
            {
                userRep.AddAsync(
                    new UserEntity() 
                    { 
                        FullName = "Duke Cheng",
                        Email = "dk@feinian.me",
                        PasswordHash = "K6oKuADN/NUWXAH9P8O4u7Ckrf0=",
                        PasswordSalt = "",
                    });
            }
        }

        public async Task<string> CreateUserAsync(User user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);
            await _userRep.AddAsync(userEntity);
            return userEntity.Sysid.ToString();
        }

        public async Task<User> GetUserByEmial(string email)
        {
            var userEntity = await _userRep.GetByPropertyAsync(x => x.Email, email);
            return _mapper.Map<User>(userEntity);
        }

        public bool ValidateLastChanged(string lastChanged)
        {
            if (!DateTime.TryParse(lastChanged, out var lastChangedTime))
            {
                return false;
            }

            return lastChangedTime.AddSeconds(30) > DateTime.Now;
        }
    }
}
