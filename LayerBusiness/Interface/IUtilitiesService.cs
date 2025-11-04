using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerBusiness.Interface;

public interface IUtilitiesService
{
    string GeneratePassword();//retorna un codigo para logeaurse

    string ConvertSha256(string text);//devulve el texto encriptado en Sha256

}
