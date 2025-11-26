using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;

namespace LayerBusiness.Implementation;

public class TipoDocumentoVentaService : ITipoDocumentoVentaService
{
    public IGenericRepository<TipoDocumentoVenta> _repository;

    public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repository)
    {
        _repository = repository;
    }

    public async Task<List<TipoDocumentoVenta>> List()
    {
        IQueryable<TipoDocumentoVenta> query = await _repository.Consult();
        return query.ToList();
    } 
}
