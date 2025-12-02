using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerEntity;
using LayerBusiness.Interface;

namespace Web.Controllers;


public class VentaController : Controller
{

    private readonly ITipoDocumentoVentaService _tipoDocumentoService;
    private readonly IVentaService _ventaService;
    private readonly IMapper _mapper;


    public VentaController(ITipoDocumentoVentaService tipoDocumentoService,
        IVentaService ventaService,
        IMapper mapper)
    {
        _tipoDocumentoService = tipoDocumentoService;
        _ventaService = ventaService;
        _mapper = mapper;
    }

    #region VentaNueva
    public IActionResult NuevaVenta()
    {
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> ObtenerTipoDocumentos()
    {
        List<VMTipoDocumento> vmListaTipoDocumento= _mapper.Map<List<VMTipoDocumento>>(await _tipoDocumentoService.List());
        return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumento);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos(string busqueda)
    {
        List<VMProducto> vmListaProducto = _mapper.Map<List<VMProducto>>(await _ventaService.ObtenerProductos(busqueda));
        return StatusCode(StatusCodes.Status200OK, vmListaProducto);
    }


    [HttpPost]
    public async  Task<IActionResult> RegistrarVenta([FromBody] VMVenta vmVenta)
    {
        GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();
        try
        {
            vmVenta.IdUsuario = 9; //Temporalmente hasta tener el sistema de login
            Venta ventaRegistrar = _mapper.Map<Venta>(vmVenta);
            Venta ventaRegistrada = await _ventaService.Registrar(ventaRegistrar);
            gResponse.Estado = true;
            gResponse.objeto = _mapper.Map<VMVenta>(ventaRegistrada);
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    #endregion


    #region HistorialVenta
    public IActionResult HistorialVenta()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
    {
        List<VMVenta> vmListaVentas = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta, fechaInicio, fechaFin));
        return StatusCode(StatusCodes.Status200OK, vmListaVentas);
    }


    #endregion
}
