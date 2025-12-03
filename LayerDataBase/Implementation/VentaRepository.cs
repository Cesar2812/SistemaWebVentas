using LayerDataBase.Interface;
using LayerEntity;
using Microsoft.EntityFrameworkCore;

namespace LayerDataBase.Implementation;

public class VentaRepository : GenericRepository<Venta>, IVentaRepository
{
    private readonly VentaContext _dbVentaContext;

    public VentaRepository(VentaContext dbVentaContext) : base(dbVentaContext)
    {
        _dbVentaContext = dbVentaContext;
    }

    public async Task<Venta> Register(Venta entity)
    {
        Venta ventaGenerate = new();//objeto de la venta generada

        //transaccionnsobre la base de datos
        using (var transaction = _dbVentaContext.Database.BeginTransaction())
        {
            try
            {
                //iterando sobre los prudctos del detalle
                foreach (DetalleVenta dv in entity.DetalleVenta)//mediante el forEach se recorre sobre la entidad  que se pasa como parametro
                {
                    Producto productFound = _dbVentaContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                    productFound.Stock = productFound.Stock - dv.Cantidad;
                    _dbVentaContext.Productos.Update(productFound);//actuliza el campo stock de la tablaProducto
                }
                await _dbVentaContext.SaveChangesAsync();

                NumeroCorrelativo correlativo = _dbVentaContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();//el primerRegistro que encuentre
                correlativo.UltimoNumero = correlativo.UltimoNumero + 1;//=1
                correlativo.FechaActualizacion = DateTime.Now;
                _dbVentaContext.NumeroCorrelativos.Update(correlativo);
                await _dbVentaContext.SaveChangesAsync();

                string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                string numeeroVenta = ceros + correlativo.UltimoNumero.ToString();//para el numero de venta
                numeeroVenta = numeeroVenta.Substring(numeeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                entity.NumeroVenta = numeeroVenta;

                await _dbVentaContext.Venta.AddAsync(entity);
                await _dbVentaContext.SaveChangesAsync();

                ventaGenerate = entity;

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
        return ventaGenerate;

    }

    public async Task<List<DetalleVenta>> Report(DateTime fechaInicio, DateTime fechaFin)
    {
        //reporte de venta por rango de fecha
        List<DetalleVenta> listaResumen = await _dbVentaContext.DetalleVenta
            .Include(v => v.IdVentaNavigation).ThenInclude(u => u.IdUsuarioNavigation)
            .Include(v => v.IdVentaNavigation).ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation).
            Include(dv => dv.IdProductoNavigation).
            Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date && dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date).
            ToListAsync();

        return listaResumen;

    }
}
