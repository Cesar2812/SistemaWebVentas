using LayerDataBase.Implementation;
using LayerDataBase.Interface;
using LayerEntity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LayerControllerInversion
{
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

            // transient valores pueden Variar
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IVentaRepository, VentaRepository>();
        }

    }
}
