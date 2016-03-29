using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using DatabaseContext.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace DatabaseContext.DatabaseAcces
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : DomainBase 
    {
        private MongoDatabase database;
        private MongoCollection<TEntity> collection;

        public Repository()
        {
            GetDatabase();
            GetCollection();
        }
        public bool Add(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            return collection.Insert(entity).UpdatedExisting;
        }

        public bool Update(TEntity entity)
        {
            return collection
                .Save(entity)
                    .DocumentsAffected > 0;
        }

        public bool Delete(TEntity entity)
        {
            return collection
                .Remove(Query.EQ("_id", entity.Id))
                    .DocumentsAffected > 0;
        }

        public IList<TEntity>
            GetQuery(Expression<Func<TEntity, bool>> predicate)
        {
            return collection
                .AsQueryable<TEntity>()
                    .Where(predicate.Compile())
                        .ToList();
        }

        public IList<TEntity> GetAll()
        {
            return collection.FindAllAs<TEntity>().ToList();
        }

        public TEntity GetById(Guid id)
        {
            return collection.FindOneByIdAs<TEntity>(id);
        }

        private void GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());
            var server = client.GetServer();

            database = server.GetDatabase(GetDatabaseName());
        }

        private string GetConnectionString()
        {
            return ConfigurationManager
            .AppSettings
                .Get("MongoDbConnectionString")
                    .Replace("{DB_NAME}", GetDatabaseName());

        }

        private string GetDatabaseName()
        {
            return ConfigurationManager
            .AppSettings
                .Get("MongoDbDatabaseName");
        }

        private void GetCollection()
        {
            collection = database
                .GetCollection<TEntity>(typeof(TEntity).Name);
        }

    }
}
