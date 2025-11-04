using LayerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerBusiness.Interface;

public interface IUsuarioService
{
    //RepositoryBD
    Task<List<Usuario>> List();

    Task<Usuario> Create(Usuario entityUser, Stream photo=null, string namePhoto="",string urlPlantillaCorreo="");

    Task<Usuario> Update(Usuario entityUser, Stream photo = null, string namePhoto = "");

    Task<Usuario> GetById(int idUsuario);

    Task<bool> Delete(int idUsuario);

    Task<bool> SaveProfile(Usuario entityUser);

    //Login
    Task<Usuario> GetByCredentials(string email,string pass);

    //Profile
    Task<bool> ChangePass(int idUsuario, string pass, string newPass);


    Task<bool> RestorePass(string email, string urlPlantillaCorreo);

}
