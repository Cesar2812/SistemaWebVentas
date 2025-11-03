using LayerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LayerDataBase.Interface
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<Venta> Register(Venta entity);
        Task<List<DetalleVenta>> Report(DateTime fechaInicio, DateTime fechaFin);


    }
}
