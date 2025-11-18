using LayerEntity;
namespace LayerBusiness.Interface;

public interface INegocioService
{
    Task<Negocio> Get();

    Task<Negocio> SaveChanges(Negocio entidad, Stream logo = null, string nombreLogo = "");

}
