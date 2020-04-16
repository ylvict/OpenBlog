using Microsoft.Extensions.Logging;
using Niusys.Extensions.Storage.Mongo;
using System.Collections.Generic;
using System.Text;

namespace OpenBlog.Repository.Mongo.Abstracts
{
    public class AppNoSqlBaseRepository<TEntity> 
        : NoSqlBaseRepository<TEntity, MongoDefaultSetting>, IAppNoSqlBaseRepository<TEntity>
        where TEntity : MongoEntityBase
    {

        public AppNoSqlBaseRepository(MongodbContext<MongoDefaultSetting> mongoDatabase, ILogger<AppNoSqlBaseRepository<TEntity>> logger)
            : base(mongoDatabase, logger)
        {
        }
    }
}
