using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerEntity;
using LayerBusiness.Interface;

using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Web.Controllers;

[Authorize]
public class VentaController : Controller
{
    //depencias de abstraccion de servicios
    private readonly ITipoDocumentoVentaService _tipoDocumentoService;
    private readonly IVentaService _ventaService;
    private readonly IMapper _mapper;
    private readonly IConverter _converter;



    public VentaController(ITipoDocumentoVentaService tipoDocumentoService,
        IVentaService ventaService,
        IMapper mapper,IConverter converter)
    {
        _tipoDocumentoService = tipoDocumentoService;
        _ventaService = ventaService;
        _mapper = mapper;
        _converter = converter;
    }

    #region VentaNueva

    //vista de Nueva Venta
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


    //endpoint para registrar la venta
    [HttpPost]
    public async  Task<IActionResult> RegistrarVenta([FromBody] VMVenta vmVenta)
    {
        GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();
        try
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            //obteniendo el usuario Logueado
            string idUsuario = claimsUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            vmVenta.IdUsuario = Convert.ToInt32(idUsuario);
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
    //vista historial de ventas
    public IActionResult HistorialVenta()
    {
        return View();
    }

    //endpoint para obtener el historial de ventas
    [HttpGet]
    public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
    {
        List<VMVenta> vmListaVentas = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta, fechaInicio, fechaFin));
        return StatusCode(StatusCodes.Status200OK, vmListaVentas);
    }


    //vista para el pdf de la Venta
    public IActionResult MostrarPDFVenta(string numeroVenta)
    {
        string urlPlantillaVistaPDF = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";//obteniendo la plantilla del pasandole el parametro

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = new GlobalSettings()
            {
                PaperSize=PaperKind.A4,
                Orientation=Orientation.Portrait
            },
            Objects =
            {
                new ObjectSettings()
                {
                    Page=urlPlantillaVistaPDF,
                }
            }   
        };

        var archivoPDF =_converter.Convert(pdf);

        return File(archivoPDF, "application/pdf");
    }
    #endregion



}
