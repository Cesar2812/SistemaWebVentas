using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;
using System.Net;
using System.Net.Mail;

namespace LayerBusiness.Implementation;

public class CorreoService:ICorreoService
{
    private readonly IGenericRepository<Configuracion> _repository;//para buscar los datos de configuracion en base de datos

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

            var credentials = new NetworkCredential(config["correo"], config["clave"]);//creando credenciales

            var correo = new MailMessage()
            {
                From= new MailAddress(config["correo"], config["alias"]),
                Subject=asunto,
                Body=mensaje,
                IsBodyHtml=true //estrutura html en el body del correo
            };

            correo.To.Add(new MailAddress(correoDestino));

            //cliente servidor
            var clientServer = new SmtpClient()
            {
                Host= config["host"],//servidor de correo de Gmail desde donde se envia
                Port= Convert.ToInt32(config["puerto"]),
                Credentials= credentials,
                DeliveryMethod=SmtpDeliveryMethod.Network, //por red
                UseDefaultCredentials=false,
                EnableSsl=true,// en https
            };
            clientServer.Send(correo);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
