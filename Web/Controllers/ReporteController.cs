using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using LayerBusiness.Interface;

namespace Web.Controllers;

public class ReporteController : Controller
{
    private readonly IMapper _mapper;
    private readonly IVentaService _ventaService;

    public ReporteController(IMapper mapper,IVentaService ventaService)
    {
        _mapper = mapper;
        _ventaService = ventaService;    
    }

    //vista de los reportes
    public IActionResult ReporteVentas()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
    {
        List<VMReporteVenta> vmListaReporte = _mapper.Map<List<VMReporteVenta>>( await _ventaService.ReporteVenta(fechaInicio, fechaFin));
        return StatusCode(StatusCodes.Status200OK, new { data = vmListaReporte });
    }
}
