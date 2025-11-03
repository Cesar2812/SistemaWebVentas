using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class VentaController : Controller
    {
        #region VentaNueva
        public IActionResult NuevaVenta()
        {
            return View();
        }

        #endregion


        #region HistorialVenta
        public IActionResult HistorialVenta()
        {
            return View();
        }

        #endregion
    }
}
