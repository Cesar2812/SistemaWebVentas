using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;

namespace LayerBusiness.Implementation;

public class NegocioService : INegocioService
{
    private readonly IGenericRepository<Negocio> _negocioRepository;
    private readonly IFireBaseService _fireBaseService;

    public NegocioService(IGenericRepository<Negocio> negocioRepository, IFireBaseService fireBaseService)
    {
        _negocioRepository = negocioRepository;
        _fireBaseService = fireBaseService;
    }

    public async Task<Negocio> Get()
    {

        try
        {
            Negocio negocio_Encontrado= await _negocioRepository.Get(n=>n.IdNegocio==1);
            return negocio_Encontrado;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Negocio> SaveChanges(Negocio entidad, Stream logo = null, string nombreLogo = "")
    {
        try
        {
            Negocio negocio_encontrado= await _negocioRepository.Get(n=>n.IdNegocio==1);

            negocio_encontrado.NumeroDocumento=entidad.NumeroDocumento;
            negocio_encontrado.Nombre=entidad.Nombre;
            negocio_encontrado.Correo=entidad.Correo;
            negocio_encontrado.Direccion=entidad.Direccion;
            negocio_encontrado.Telefono=entidad.Telefono;
            negocio_encontrado.PorcentajeImpuesto=entidad.PorcentajeImpuesto;
            negocio_encontrado.SimboloMoneda=entidad.SimboloMoneda;

            negocio_encontrado.NombreLogo=negocio_encontrado.NombreLogo==""?nombreLogo:negocio_encontrado.NombreLogo;

            if (logo != null)
            {
                string urlFoto = await _fireBaseService.LoadStorage(logo, "carpeta_logo", negocio_encontrado.NombreLogo);
                negocio_encontrado.UrlLogo = urlFoto;
            }

            await _negocioRepository.Update(negocio_encontrado);

            return negocio_encontrado;

        }
        catch
        {
            throw;
        }
    }
}
