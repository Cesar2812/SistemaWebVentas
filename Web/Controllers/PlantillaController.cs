using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using LayerBusiness.Interface;


namespace Web.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioServicio;
        private readonly IVentaService _ventaServicio;

        public PlantillaController(IMapper mapper, INegocioService negocioServicio, IVentaService ventaServicio)
        {
            _mapper = mapper;
            _negocioServicio = negocioServicio;
            _ventaServicio = ventaServicio;
        }


        //enviar clave al crear un usuario
        public IActionResult SendPass(string correo, string clave)
        {
            ViewData["correo"] = correo;
            ViewData["clave"]=clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";//esquema https del host del sistema y dominio

            return View();
        }

        public IActionResult RestorePass(string clave)
        {
            ViewData["clave"] = clave;
            return View();
        } 




        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaServicio.Detalle(numeroVenta));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Get());

            VMPDFVenta modeloPDF = new();
            modeloPDF.negocio=vmNegocio;
            modeloPDF.venta=vmVenta;
            return View(modeloPDF);
        }
    }
}
