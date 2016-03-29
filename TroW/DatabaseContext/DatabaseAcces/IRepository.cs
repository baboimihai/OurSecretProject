using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DatabaseContext.Domain;

namespace DatabaseContext.DatabaseAcces
{

    public interface IRepository<TEntity> where TEntity : DomainBase 
    {
        bool Add(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
        IList<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        TEntity GetById(Guid id);
    }
}
