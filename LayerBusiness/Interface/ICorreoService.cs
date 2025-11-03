using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerBusiness.Interface
{
    public interface ICorreoService
    {
        Task<bool> SendEmail(string correoDestino, string asunto, string mensaje);
    }
}
