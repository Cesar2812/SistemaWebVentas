using LayerEntity;


namespace LayerBusiness.Interface;

public interface IProductoService
{

    //RepositoryBD
    Task<List<Producto>> ListProduct();

    Task<Producto> Create(Producto entityProduct, Stream photo = null, string namePhoto = "");

    Task<Producto> Update(Producto entityProduct, Stream photo = null, string namePhoto = "");


    Task<bool> Delete(int idProducto);
}
