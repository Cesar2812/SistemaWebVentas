using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LayerDataBase.DBContext;
using Microsoft.EntityFrameworkCore;
using LayerDataBase.Interface;
using LayerDataBase.Implementation;
using LayerBusiness.Interface;
using LayerBusiness.Implementation;
using System.Runtime.CompilerServices;


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

    }

}
