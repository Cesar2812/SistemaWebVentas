using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult SendPass(string email, string pass)
        {
            ViewData["Correo"] = email;
            ViewData["Clave"]=pass;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";//esquema https del host del sistema y dominio

            return View();
        }

        public IActionResult RestorePass(string pass)
        {
            ViewData["Clave"] = pass;
            return View();
        }
    }
}
