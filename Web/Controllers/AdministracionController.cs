using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerBusiness.Interface;
using LayerEntity;
using Newtonsoft.Json;

namespace Web.Controllers;

public class AdministracionController : Controller
{
    private readonly IUsuarioService _usuarioService;//servicio de usuario 
    private readonly IMapper _mapper;
    private readonly IRoleService _roleService;

    private readonly INegocioService _negocioService;

    public AdministracionController(IUsuarioService usuarioService, IMapper mapper, IRoleService roleService,INegocioService negocioService)
    {
        
        _usuarioService = usuarioService;
        _mapper=mapper;
        _roleService=roleService;

        _negocioService=negocioService;
    }


    #region Usuarios

    public IActionResult Usuarios()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> ListRoles()
    {
        List<VMRol> vmListRoles = _mapper.Map<List<VMRol>>(await _roleService.List());//convietriendo la lista de roles a VM
        return StatusCode(StatusCodes.Status200OK, vmListRoles);
    }


    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        List<VMUsuario> vmLisUsers = _mapper.Map<List<VMUsuario>>(await _usuarioService.List());//convietriendo la lista de usuarios a VM
        return StatusCode(StatusCodes.Status200OK, new { data=vmLisUsers});//objc de C#
    }



    [HttpPost]
    public async Task<IActionResult> CreateUsers([FromForm] IFormFile foto, [FromForm] string model)
    {
        GenericResponse<VMUsuario> gResponse= new GenericResponse<VMUsuario>();

        try
        {
            VMUsuario? vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(model);

            string nombreFoto = "";
            Stream? fotoStream = null;

            if (foto != null)
            {
                string nombre_en_codigo=Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombre_en_codigo, extension);
                fotoStream = foto.OpenReadStream();
            }

            //esquema http o https
            string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/SendPass?correo=[correo]&clave=[clave]";


            Usuario usuario_creado = await _usuarioService.Create(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);


            vmUsuario=_mapper.Map<VMUsuario>(usuario_creado);
            gResponse.Estado = true;
            gResponse.objeto = vmUsuario;
        }
        catch(Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje=ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


    [HttpPut]
    public async Task<IActionResult> UpdateUsers([FromForm] IFormFile foto, [FromForm] string model)
    {
        GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

        try
        {
            VMUsuario? vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(model);

            string nombreFoto = "";
            Stream? fotoStream = null;

            if (foto != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombre_en_codigo, extension);
                fotoStream = foto.OpenReadStream();
            }


            Usuario usuarii_editado = await _usuarioService.Update(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);


            vmUsuario = _mapper.Map<VMUsuario>(usuarii_editado);
            gResponse.Estado = true;
            gResponse.objeto = vmUsuario;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int idUsuario)
    {
        GenericResponse<string> gResponse = new GenericResponse<string>();

        try
        {
            gResponse.Estado = await _usuarioService.Delete(idUsuario);
        }
        catch (Exception ex) 
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;

        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }






    public IActionResult PerfilUser()
    {
        return View();
    }

    #endregion


    #region Negocio
    public IActionResult Negocio()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> GetNegocio()
    {
        GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
        try
        {
            Negocio negocio_encontrado = await _negocioService.Get();
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(negocio_encontrado);
            gResponse.Estado = true;
            gResponse.objeto = vmNegocio;
        }
        catch(Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


    [HttpPost]
    public async Task<IActionResult> SaveChangesNegocio([FromForm] IFormFile logo, [FromForm] string model)
    {
        GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
        try
        {
            VMNegocio? vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(model);
            string nombreLogo = "";
            Stream? logoStream = null;
            if (logo != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(logo.FileName);
                nombreLogo = string.Concat(nombre_en_codigo, extension);
                logoStream = logo.OpenReadStream();
            }
            Negocio negocio_editado = await _negocioService.SaveChanges(_mapper.Map<Negocio>(vmNegocio), logoStream, nombreLogo);
            vmNegocio = _mapper.Map<VMNegocio>(negocio_editado);
            gResponse.Estado = true;
            gResponse.objeto = vmNegocio;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }
    #endregion
}
