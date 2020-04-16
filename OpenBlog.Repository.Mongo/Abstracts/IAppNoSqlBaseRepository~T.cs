using Niusys.Extensions.Storage.Mongo;

namespace OpenBlog.Repository.Mongo.Abstracts
{
    public interface IAppNoSqlBaseRepository<TEntity> : INoSqlBaseRepository<TEntity>
          where TEntity : MongoEntityBase
    {

    }
}
