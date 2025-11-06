using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult SendPass(string correo, string clave)
        {
            ViewData["correo"] = correo;
            ViewData["clave"]=clave;
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
