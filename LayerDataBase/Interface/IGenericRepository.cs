using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LayerDataBase.Interface;

public interface IGenericRepository<TEntity> where TEntity : class //para todas las entidades
{
    Task<TEntity> Get(Expression<Func<TEntity, bool>> filter);//como parametro una expresion labda para filtrar 

    Task<TEntity> Create(TEntity entity);

    Task<bool> Update(TEntity entity);

    Task<bool> Delete(TEntity entity);

    Task<IQueryable<TEntity>> Consult(Expression<Func<TEntity, bool>> filter = null);

}
