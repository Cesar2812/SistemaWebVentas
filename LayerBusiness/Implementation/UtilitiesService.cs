using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerBusiness.Interface;
using System.Security.Cryptography;

namespace LayerBusiness.Implementation
{
    public class UtilitiesService : IUtilitiesService
    {
        public string GeneratePassword()
        {
            string pass= Guid.NewGuid().ToString("N").Substring(0,6);//cadena de texto random
            return pass;
        }

        public string ConvertSha256(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty; 

            StringBuilder sb= new StringBuilder();

            using (SHA256 hash= SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(text));//convierte el texto en un array de byte
                foreach (byte b in result) 
                {

                    sb.Append(b.ToString("x2"));
          
                }
            }
            return sb.ToString();
        }  
    }
}
