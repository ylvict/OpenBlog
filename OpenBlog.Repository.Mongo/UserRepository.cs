using MongoDB.Bson.Serialization.Attributes;
using OpenBlog.DomainModels;
using System;

namespace OpenBlog.Repository.Mongo
{
    public class UserRepository : IUserRepository
    {
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
