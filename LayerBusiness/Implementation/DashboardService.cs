using Microsoft.EntityFrameworkCore;
using LayerBusiness.Interface;
using LayerEntity;
using LayerDataBase.Interface;
using System.Globalization;

namespace LayerBusiness.Implementation;

public class DashboardService : IDashboardService
{

    private readonly IVentaRepository _ventaRepository;
    private readonly IGenericRepository<DetalleVenta> _repositorioDetalleVenta;
    private readonly IGenericRepository<Categoria> _categoriaRepository;
    private readonly IGenericRepository<Producto> _productoRepository;

    private DateTime fechaInicio=DateTime.Now;//obtiene la fecha actual 

    public DashboardService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> repositorioDetalleVenta, IGenericRepository<Categoria> categoriaRepository,
        IGenericRepository<Producto> productoRepository)
    {
        _ventaRepository = ventaRepository;
        _repositorioDetalleVenta= repositorioDetalleVenta;
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;

        fechaInicio = fechaInicio.AddDays(-7);//hace 7 dias
    }

    public async Task<int> TotalVentasUltimaSemana()
    {
        try
        {
            IQueryable<Venta> query = await _ventaRepository.Consult(v => v.FechaRegistro.Value.Date >= fechaInicio.Date); //ventas registradas despues de la fecha de inicio
            int total=query.Count();
            return total;
        }
        catch
        {
            throw;
        }
    }


    public async Task<string> TotalIngresosUltimaSemana()
    {
        try
        {
            IQueryable<Venta> query = await _ventaRepository.Consult(v => v.FechaRegistro.Value.Date >= fechaInicio.Date); //ventas registradas despues de la fecha de inicio
            decimal resultado = query
                .Select(v=>v.Total)
                .Sum(v=>v.Value);//sumando sobre la misma columna
            return Convert.ToString(resultado, new CultureInfo("es-NI"));
        }
        catch
        {
            throw;
        }
    }


    public async Task<int> TotalProductos()
    {
        try
        {
            IQueryable<Producto> query = await _productoRepository.Consult();
            int resultado = query.Count();
            return resultado;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> TotalCategorias()
    {
        try
        {
            IQueryable<Categoria> query = await _categoriaRepository.Consult();
            int resultado = query.Count();
            return resultado;
        }
        catch
        {
            throw;
        }
    }


    public async Task<Dictionary<string, int>> VentasUltimaSemana()
    {
        try
        {
            IQueryable<Venta> query = await _ventaRepository.Consult(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);

            //creando DIccionariio
            Dictionary<string, int> resultado = query
                .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key).
                Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }).ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

            return resultado;
        }
        catch
        {
            throw;
        }
    }


    public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
    {
        try
        {
            IQueryable<DetalleVenta> query = await _repositorioDetalleVenta.Consult();

            //creando DIccionariio
            Dictionary<string, int> resultado = query
                .Include(v => v.IdVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date)
                .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count()).Take(4).
                Select(dv => new { producto = dv.Key, total = dv.Count() }).ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);

            return resultado;
        }
        catch
        {
            throw;
        }
    } 
   
}
