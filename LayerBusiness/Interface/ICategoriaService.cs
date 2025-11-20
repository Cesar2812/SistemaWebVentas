using LayerEntity;

namespace LayerBusiness.Interface;

public interface ICategoriaService
{
    Task<List<Categoria>> GetAllCategoriasAsync();
    Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    Task<Categoria> UpdateCategoriaAsync(Categoria categoria);
    Task<bool> DeleteCategoriaAsync(int id);
}
