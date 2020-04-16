using MongoDB.Bson.Serialization.Attributes;
using Niusys.Extensions.Storage.Mongo;
using System;

namespace OpenBlog.Repository.Mongo.Abstracts
{
    [BsonIgnoreExtraElements(true), Serializable]
    public class MongoEntityBase : MongoEntity
    {
    }
}
