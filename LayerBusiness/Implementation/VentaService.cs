using LayerDataBase.Interface;
using LayerEntity;
using LayerBusiness.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LayerBusiness.Implementation;

public class VentaService : IVentaService
{
    private readonly IGenericRepository<Producto> _repositorioProducto;
    private readonly IVentaRepository _repositorioVenta;

    public VentaService(IGenericRepository<Producto> repositorioProducto, IVentaRepository repositorioVenta)
    {
        _repositorioProducto = repositorioProducto;
        _repositorioVenta = repositorioVenta;
    }


    public async Task<List<Producto>> ObtenerProductos(string busuqeda)
    {
        IQueryable<Producto> query=await _repositorioProducto.Consult(
            p=>p.EsActivo == true &&
            p.Stock>0 &&
            string.Concat(p.CodigoBarra,p.Marca,p.Descripcion).Contains(busuqeda)
        );
        return query.Include(c=>c.IdCategoriaNavigation).ToList();
    }


    public async Task<Venta> Registrar(Venta entidad)
    {
        try
        {
            return await _repositorioVenta.Register(entidad);
        }
        catch
        {
            throw;
        }
    }


    public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
    {
        IQueryable<Venta> query = await _repositorioVenta.Consult();
        fechaInicio=fechaInicio is null? "": fechaInicio;
        fechaFin = fechaFin is null ? "" : fechaFin;

        if(fechaInicio !="" && fechaFin != "")//se obtienen las fechas como parametrizacion
        {
            DateTime inicio = DateTime.ParseExact(fechaInicio,"dd/MM/yyyy", new CultureInfo("es-NI"));
            DateTime fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-NI"));
            return query.Where(v=>
                v.FechaRegistro.Value.Date >= inicio.Date && v.FechaRegistro.Value.Date <= fin.Date
            ).Include(tdv=> tdv.IdTipoDocumentoVentaNavigation)
            .Include(u=> u.IdUsuarioNavigation)
            .Include(dv=>dv.DetalleVenta)
            .ToList();
        }
        else
        {
            return query
            .Where(v=> v.NumeroVenta==numeroVenta)
            .Include(tdv=> tdv.IdTipoDocumentoVentaNavigation)
            .Include(u=> u.IdUsuarioNavigation)
            .Include(dv=>dv.DetalleVenta)
            .ToList();
        } 
    }

    public async Task<Venta> Detalle(string numeroVenta)
    {
        IQueryable<Venta> query = await _repositorioVenta.Consult(v=> v.NumeroVenta==numeroVenta);
        return query.Include(tdv=> tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u=> u.IdUsuarioNavigation)
                    .Include(dv=>dv.DetalleVenta)
                    .First();//la primera o por defecto 
    }

  
    public async Task<List<DetalleVenta>> ReporteVenta(string fechaInicio, string fechaFin)
    {
        DateTime inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-NI"));
        DateTime fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-NI"));

        List<DetalleVenta> lista = await _repositorioVenta.Report(inicio,fin);
        return lista;
    }
}
