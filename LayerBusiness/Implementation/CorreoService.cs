using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LayerBusiness.Implementation
{
    public class CorreoService:ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repository;

        public CorreoService(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<bool> SendEmail(string correoDestino, string asunto, string mensaje)
        {
            try
            {
                IQueryable<Configuracion> query = await _repository.Consult(c => c.Recurso.Equals("Servicio_Correo"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credenciales = new NetworkCredential(config["correo"], config["clave"]);


            }
            catch
            {

            }
        }
    }
}
