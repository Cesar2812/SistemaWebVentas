using LayerBusiness.Interface;
using LayerEntity;
using LayerDataBase.Interface;

namespace LayerBusiness.Implementation;

public class CategoriaService : ICategoriaService
{
    private readonly IGenericRepository<Categoria> _categoriaRepository;

    public CategoriaService(IGenericRepository<Categoria> categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }


    public async Task<List<Categoria>> GetAllCategoriasAsync()
    {
        IQueryable<Categoria> query = await _categoriaRepository.Consult();

        return query.ToList();

    }

    public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
    {
        try
        {
            Categoria categoria_creada= await _categoriaRepository.Create(categoria);

            if (categoria_creada.IdCategoria== 0)
            {
                throw new TaskCanceledException("No se pudo crear la categoria");
            }
            return categoria_creada;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Categoria> UpdateCategoriaAsync(Categoria categoria)
    {
        try
        {
            Categoria categoria_encontrada = await _categoriaRepository.Get(c => c.IdCategoria == categoria.IdCategoria);
            categoria_encontrada.Descripcion = categoria.Descripcion;
            categoria_encontrada.EsActivo = categoria.EsActivo;
            bool respuesta = await _categoriaRepository.Update(categoria_encontrada);
            if (!respuesta)
            {
                throw new TaskCanceledException("No se pudo editar la categoria");
            }
            return categoria_encontrada;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteCategoriaAsync(int id)
    {
        try
        {
            Categoria categoria_encontrada = await _categoriaRepository.Get(c => c.IdCategoria ==id);
            if (categoria_encontrada == null)
            {
                throw new TaskCanceledException("No se pudo encontrar la categoria");
            }
            bool respuesta = await _categoriaRepository.Delete(categoria_encontrada);
            return respuesta;
        }
        catch
        {
            throw;
        }
    }  
}
