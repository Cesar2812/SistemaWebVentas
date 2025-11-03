using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class InventarioController : Controller
    {
        #region Categorias
        public IActionResult Categorias()
        {
            return View();
        }
        #endregion


        #region Productos
        public IActionResult Productos()
        {
            return View();
        }
        #endregion
    }
}
