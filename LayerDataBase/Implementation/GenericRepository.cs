using LayerDataBase.Interface;
using LayerEntity;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LayerDataBase.Implementation;


//clase generica que implementa la interfaz generica que permite realizar operaciones CRUD genericas sobre una entidad en especifico
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly VentaContext _dbVentaContext;


    public GenericRepository(VentaContext dbVentaContext)
    {
        _dbVentaContext = dbVentaContext;
    }

    
    public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
    {
        try
        {
            TEntity entity = await _dbVentaContext.Set<TEntity>().FirstOrDefaultAsync(filter);//linq para filtrar
            return entity;
        }
        catch
        {
            throw;
        }
    }

    public async Task<TEntity> Create(TEntity entity)
    {
        try
        {
            _dbVentaContext.Set<TEntity>().Add(entity);
            await _dbVentaContext.SaveChangesAsync();
            return entity;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> Update(TEntity entity)
    {
        try
        {
            _dbVentaContext.Update(entity);
            await _dbVentaContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> Delete(TEntity entity)
    {
        try
        {
            _dbVentaContext.Remove(entity);
            await _dbVentaContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw;
        }

    }

    public async Task<IQueryable<TEntity>> Consult(Expression<Func<TEntity, bool>> filter = null)
    {
        IQueryable<TEntity> queryEntity = filter == null ? _dbVentaContext.Set<TEntity>() : _dbVentaContext.Set<TEntity>().Where(filter);
        return queryEntity;
    }

}
