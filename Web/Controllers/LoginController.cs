using Microsoft.AspNetCore.Mvc;
using Web.Models.ViewModels;
using LayerBusiness.Interface;
using LayerEntity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Web.Controllers;

public class LoginController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public LoginController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;   
    }

    public IActionResult Acceso()
    {
        ClaimsPrincipal claimsUser = HttpContext.User;

        if (claimsUser.Identity.IsAuthenticated) 
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }


    public IActionResult RestablecerClave() 
    {
        return View();
    } 


   
    [HttpPost]
    public async Task<IActionResult> Acceso(VMUsuarioLogin modelo)
    {
        Usuario usuario_encontrado = await _usuarioService.GetByCredentials(modelo.Correo, modelo.Clave);
        if (usuario_encontrado == null)
        {
            ViewData["Mensaje"] = "Usuario no existe";
            return View();
        }
        else
        {
            ViewData["Mensaje"] =null;

            List<Claim> listClaimsUsuario = new()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_encontrado.UrlFoto)
            };


            ClaimsIdentity claimsIdentity = new(listClaimsUsuario, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new()
            {
                AllowRefresh=true,
                IsPersistent=modelo.MantenerSesion
            };

            //registrando datos en el inicio de sesion del sistema
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

        }

        return RedirectToAction("Index", "Dashboard");

    }


    [HttpPost]
    public async Task<IActionResult> RestablecerClave( VMUsuarioLogin modelo)
    {
        try
        {
            string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestorePass?clave=[clave]";
            bool resultado = await _usuarioService.RestorePass(modelo.Correo, urlPlantillaCorreo);

            if (resultado)
            {
                ViewData["Mensaje"] = "Listo, su clave fue restablecida.Revice Su Correo";
                ViewData["MensajeError"] = null;
            }
            else
            {
                ViewData["MensajeError"] = "Tenemos Problemas porfavor intentelo otra vez mas tarde";
                ViewData["Mensaje"] = null;
            }
        }
        catch (Exception ex) {
            ViewData["MensajeError"]=ex.Message;
            ViewData["Mensaje"] = null;
        } 
        return View();
    }


}

