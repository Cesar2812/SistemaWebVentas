using LayerEntity;



namespace LayerDataBase.Interface;

public interface IVentaRepository : IGenericRepository<Venta>
{
    Task<Venta> Register(Venta entity);
    Task<List<DetalleVenta>> Report(DateTime fechaInicio, DateTime fechaFin);


}
