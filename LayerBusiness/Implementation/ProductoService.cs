using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;
using Microsoft.EntityFrameworkCore;


namespace LayerBusiness.Implementation;

public class ProductoService : IProductoService
{
    private readonly IGenericRepository<Producto> _repositoryProducto;
    private readonly IFireBaseService _fireBaseService;

    public ProductoService(IGenericRepository<Producto> repositoryProducto, IFireBaseService fireBaseService)
    {
        _repositoryProducto = repositoryProducto;
        _fireBaseService = fireBaseService;
       
    }

    public async Task<List<Producto>> ListProduct()
    {
        IQueryable<Producto> query = await _repositoryProducto.Consult();
        return query.Include(c=>c.IdCategoriaNavigation).ToList();
    }


    public async Task<Producto> Create(Producto entityProduct, Stream photo = null, string namePhoto = "")
    {
        //consultando a la base de datos si existe el codigo de barra
        Producto producto_existe = await _repositoryProducto.Get(p => p.CodigoBarra == entityProduct.CodigoBarra);

        if (producto_existe != null)
        {
                throw new TaskCanceledException("El producto ya existe");
        }
        try
        {
            entityProduct.NombreImagen = namePhoto;
            if (photo != null)
            {
                    string responsePhoto = await _fireBaseService.LoadStorage(photo, "carpeta_producto", namePhoto);
                    entityProduct.UrlImagen = responsePhoto;
            }

            Producto producto_creado=await _repositoryProducto.Create(entityProduct);

            if (producto_creado.IdProducto == 0)
            {
                throw new TaskCanceledException("No se pudo crear el producto");
            }

            IQueryable<Producto> query = await _repositoryProducto.Consult(p=>p.IdProducto==producto_creado.IdProducto);
            producto_creado= query.Include(c=>c.IdCategoriaNavigation).First();
            return producto_creado;
        }
        catch
        {
            throw;
        }
    }


    public async Task<Producto> Update(Producto entityProduct, Stream photo = null, string namePhoto = "")
    {
        //consultando a la base de datos si existe el codigo de barra
        Producto producto_existe = await _repositoryProducto.Get(p => p.CodigoBarra == entityProduct.CodigoBarra);

        if (producto_existe != null)
        {
            throw new TaskCanceledException("El producto ya existe");
        }
        try
        {
            IQueryable<Producto> queryProducto = await _repositoryProducto.Consult(p => p.IdProducto == entityProduct.IdProducto);
            Producto producto_a_editar = queryProducto.First();//el primero que se obtega con los parametros

            producto_a_editar.CodigoBarra = entityProduct.CodigoBarra;
            producto_a_editar.Marca = entityProduct.Marca;
            producto_a_editar.Descripcion = entityProduct.Descripcion;
            producto_a_editar.IdCategoria = entityProduct.IdCategoria;
            producto_a_editar.Stock = entityProduct.Stock;
            producto_a_editar.Precio = entityProduct.Precio;
            producto_a_editar.EsActivo = entityProduct.EsActivo;

            if (producto_a_editar.NombreImagen == "")
            {
                producto_a_editar.NombreImagen =namePhoto;
            }

            if (photo != null)
            {
                string responsePhoto = await _fireBaseService.LoadStorage(photo, "carpeta_producto", producto_a_editar.NombreImagen);
                entityProduct.UrlImagen = responsePhoto;
            }

            bool respuesta = await _repositoryProducto.Update(producto_a_editar);

            if (!respuesta)
            {
                throw new TaskCanceledException("No se pudo editar el producto");
            }

            Producto producto_actualizado= queryProducto.Include(c=>c.IdCategoriaNavigation).First();
            return producto_actualizado;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> Delete(int idProducto)
    {
        try
        {
            Producto producto_econtrado= await _repositoryProducto.Get(p=>p.IdProducto==idProducto);
            if (producto_econtrado==null)
            {
                throw new TaskCanceledException("El producto no existe");
            }
            string nombreImagen = producto_econtrado.NombreImagen;
            bool respuesta = await _repositoryProducto.Delete(producto_econtrado);
            if (respuesta)
            {
                await _fireBaseService.DeleteStorage("carpeta_producto", nombreImagen);
            }
            return true;
        }
        catch
        {
            throw;
        }
    }  
}
