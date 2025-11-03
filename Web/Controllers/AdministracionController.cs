using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdministracionController : Controller
    {
        #region Usuarios
        public IActionResult Usuarios()
        {
            return View();
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
        #endregion
    }
}
