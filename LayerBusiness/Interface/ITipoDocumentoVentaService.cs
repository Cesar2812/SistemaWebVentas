using LayerEntity;
namespace LayerBusiness.Interface;

public interface ITipoDocumentoVentaService
{
    public Task<List<TipoDocumentoVenta>> List();
}
