using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LayerEntity;


namespace LayerBusiness.Interface
{
    public interface IVentaService
    {
        Task<List<Producto>> ObtenerProductos(string busuqeda);


        Task<Venta> Registrar(Venta entidad);

        Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin);

        Task<Venta> Detalle(string numeroVenta);

        Task<List<DetalleVenta>> ReporteVenta(string fechaInicio, string fechaFin);

    }
}
