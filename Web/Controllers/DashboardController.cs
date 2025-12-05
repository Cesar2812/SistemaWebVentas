using Microsoft.AspNetCore.Mvc;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerBusiness.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashboard> gResponse = new();
            try
            {
                VMDashboard vmDashboard = new();

                vmDashboard.TotalVentas = await _dashboardService.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashboardService.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashboardService.TotalProductos();
                vmDashboard.TotalCategorias = await _dashboardService.TotalCategorias();

                List<VMVentaSemana> listaVentasSemana = new();
                List<VMProductosSemana> listaProductosSemana = new();


                foreach(KeyValuePair<string,int> item in await _dashboardService.VentasUltimaSemana())
                {
                    listaVentasSemana.Add(new VMVentaSemana()
                    {
                        Fecha=item.Key,
                        Total=item.Value,
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashboardService.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                       Producto=item.Key,
                       Cantidad=item.Value,
                    });
                }

                vmDashboard.VentasUltimaSemana = listaVentasSemana;
                vmDashboard.ProductosTopUltimaSemana= listaProductosSemana;

                gResponse.Estado = true;
                gResponse.objeto= vmDashboard;
            }
            catch (Exception ex) {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message; ;
            }
            return StatusCode(StatusCodes.Status200OK,gResponse);
        }
    }
}
