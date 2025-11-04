using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerBusiness.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;
using LayerEntity;
using LayerDataBase;
using LayerDataBase.Interface;

namespace LayerBusiness.Implementation;

public class UsuarioService : IUsuarioService
{
    private readonly IGenericRepository<Usuario> _repository;
    private readonly IFireBaseService _fireBaseService;
    private readonly IUtilitiesService _utilitiesService;
    private readonly ICorreoService _correoService;

    public UsuarioService(IGenericRepository<Usuario> repository, IFireBaseService fireBaseService,
        IUtilitiesService utilitiesService, ICorreoService correoService)
    {
        _repository = repository;
        _fireBaseService = fireBaseService;
        _utilitiesService = utilitiesService;
        _correoService = correoService;
    }

    public async Task<List<Usuario>> List()
    {
        IQueryable<Usuario> query = await _repository.Consult();
        return query.Include(rol=> rol.IdRolNavigation).ToList(); 
    }

    public async Task<Usuario> Create(Usuario entityUser, Stream photo = null, string namePhoto = "", string urlPlantillaCorreo = "")
    {
        Usuario usuarioExist = await _repository.Get(u => u.Correo == entityUser.Correo);
        if(usuarioExist != null)
        {
            throw new TaskCanceledException("Ya existe un usuario con este correo");
        }

        try
        {
            string passGenerate = _utilitiesService.GeneratePassword();
            entityUser.Clave = _utilitiesService.ConvertSha256(passGenerate);
            entityUser.NombreFoto=namePhoto;

            if (photo != null)//si se pasa la foto
            {
                string urlFoto = await _fireBaseService.LoadStorage(photo, "carpeta_usuario", namePhoto);
                entityUser.UrlFoto=urlFoto;
            }

            Usuario usuarioCreate= await _repository.Create(entityUser);

            if (usuarioCreate.IdUsuario == 0)
            {
                throw new TaskCanceledException("No se Pudo Crear el Usuario");
            }

            if (urlPlantillaCorreo != "")//osea se pasa la Url de la vista
            {
                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", usuarioCreate.Correo).Replace("[clave]", passGenerate);//se remplaza en el body

                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response=(HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;

                        if (response.CharacterSet == null)
                        {
                            readerStream = new StreamReader(dataStream);
                        }
                        else
                        {
                            readerStream= new StreamReader(dataStream,Encoding.GetEncoding(response.CharacterSet));
                        }

                        htmlCorreo= readerStream.ReadToEnd();//lee el header, el codigo y el body
                        response.Close();
                        readerStream.Close();
                    }
                }

                //se obtiene el html
                if (htmlCorreo != "")
                {
                    await _correoService.SendEmail(usuarioCreate.Correo, "CuentaCreada", htmlCorreo);

                }
            }
            IQueryable<Usuario> query = await _repository.Consult(u => u.IdUsuario == usuarioCreate.IdUsuario);
            usuarioCreate = query.Include(rol => rol.IdRolNavigation).First();

            return usuarioCreate;

        }
        catch
        {
            throw;
        }
    }

    public async Task<Usuario> Update(Usuario entityUser, Stream photo = null, string namePhoto = "")
    {
        Usuario usuarioExist = await _repository.Get(u => u.Correo == entityUser.Correo && u.IdUsuario!=entityUser.IdUsuario);//valida con el resto menos con el del parametro
        if (usuarioExist != null)
        {
            throw new TaskCanceledException("Ya existe un usuario con este correo");
        }

        try
        {
            IQueryable<Usuario> queryUsuario = await _repository.Consult(u=> u.IdUsuario==entityUser.IdUsuario);
            Usuario usuario_Editar = queryUsuario.First();

            usuario_Editar.Nombre=entityUser.Nombre;
            usuario_Editar.Correo= entityUser.Correo;
            usuario_Editar.IdRol = entityUser.IdRol;

            if (usuario_Editar.NombreFoto == "")
            {
                usuario_Editar.NombreFoto = namePhoto;
            }
            if(photo != null)
            {
                string url = await _fireBaseService.LoadStorage(photo, "carpeta_usuario", usuario_Editar.NombreFoto);
                usuario_Editar.UrlFoto=url;
            }

            bool respuesta = await _repository.Update(entityUser);

            if (!respuesta)
            {
                throw new TaskCanceledException("No se pudo editar el usuario");
            }

            Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();

            return usuario_editado;

        }
        catch
        {
            throw;
        }

    }

    public async Task<bool> Delete(int idUsuario)
    {
        try
        {
            Usuario usuarioEncontrado = await _repository.Get(u => u.IdUsuario == idUsuario);

            if (usuarioEncontrado == null)
            {
                throw new TaskCanceledException("No se encontro el Usuario");
            }

            string nombreFoto = usuarioEncontrado.NombreFoto;
            bool respuest = await _repository.Delete(usuarioEncontrado);

            if (respuest)
            {
                await _fireBaseService.DeleteStorage("carpeta_usuario", nombreFoto);
            }

            return true;
        }
        catch
        {
            throw;
        }
    }


    public Task<Usuario> GetById(int idUsuario)
    {
        throw new NotImplementedException();
    }

   


    


    public Task<bool> ChangePass(int idUsuario, string pass, string newPass)
    {
        throw new NotImplementedException();
    }

   
    public Task<Usuario> GetByCredentials(string email, string pass)
    {
        throw new NotImplementedException();
    }


    public Task<bool> SaveProfile(Usuario entityUser)
    {
        throw new NotImplementedException();
    }


    public Task<bool> RestorePass(string email, string urlPlantillaCorreo)
    {
        throw new NotImplementedException();
    }

    

    
}
