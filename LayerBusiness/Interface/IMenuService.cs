using LayerEntity;

namespace LayerBusiness.Interface;

public interface IMenuService
{

    Task<List<Menu>> GetAllMenusAsync(int idUsuario);
}
