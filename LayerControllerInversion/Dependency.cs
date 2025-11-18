using LayerDataBase.Implementation;
using LayerDataBase.Interface;
using LayerEntity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LayerBusiness.Interface;
using LayerBusiness.Implementation;

namespace LayerControllerInversion;

public static class Dependency
{

    //single responsablity and Inversion Dependency
    public static void DependencyInyection(this IServiceCollection services, IConfiguration configuration)
    {
        //conection dataBase
        services.AddDbContext<VentaContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));//cadena de conexion a EF
        });

        // transient valores pueden Variar recibe cualquier entidad
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IVentaRepository, VentaRepository>();//repositorio de venta

        services.AddScoped<ICorreoService, CorreoService>();

        services.AddScoped<IFireBaseService, FireBaseService>();

        services.AddScoped<IUtilitiesService, UtilitiesService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IUsuarioService, UsuarioService>();

        services.AddScoped<INegocioService, NegocioService>();
    }

}
