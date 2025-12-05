using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;


namespace Web.Utilities.ViewComponents;

public class MenuUsuarioViewComponent:ViewComponent
{
    //metodo por defecto en viewCompnenet
    public async Task<IViewComponentResult> InvokeAsync()
    {
        ClaimsPrincipal claimUser = HttpContext.User;//reciviendo el contexto del usuario loguado 

        string nombreUser = "";
        string urlFotoUsuario = "";

        if (claimUser.Identity.IsAuthenticated)
        {
            nombreUser = claimUser.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c=>c.Value).SingleOrDefault();

            urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;//castanando la url a un tipo identity
        }
        ViewData["nombreUsuario"] = nombreUser;
        ViewData["urlFotoUsuario"] = urlFotoUsuario;

        return View();
    }
}
