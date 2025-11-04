using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerBusiness.Interface
{
    public interface IFireBaseService
    {
        Task<string> LoadStorage(Stream streamFile, string destinationFolder, string fileName);
        Task<bool> DeleteStorage(string destinationFolder, string fileName);
    }
}
