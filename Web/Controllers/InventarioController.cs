using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerBusiness.Interface;
using LayerEntity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;



namespace Web.Controllers;

[Authorize]
public class InventarioController : Controller
{
    private readonly ICategoriaService _categoriaService;
    private readonly IProductoService _productoService;
    private readonly IMapper _mapper;
    public InventarioController(ICategoriaService categoriaService, IMapper mapper, IProductoService productoService)
    {
        _categoriaService = categoriaService;
        _productoService = productoService;
        _mapper = mapper;
    }

    #region Categorias
    public IActionResult Categorias()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListaCategorias()
    {
        List<VMCategoria> vmCategoria=_mapper.Map<List<VMCategoria>>(await _categoriaService.GetAllCategoriasAsync());
        return StatusCode(StatusCodes.Status200OK, new { data= vmCategoria });
    }

    [HttpPost]
    public async Task<IActionResult> CrearCategoria([FromBody] VMCategoria modelo)
    {
        GenericResponse<VMCategoria> gResponse = new();
        try
        {
            Categoria categoria_creada = await _categoriaService.CreateCategoriaAsync(_mapper.Map<Categoria>(modelo));
            modelo= _mapper.Map<VMCategoria>(categoria_creada);

            gResponse.Estado = true;
            gResponse.objeto= modelo;
        }
        catch(Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        } 

        return StatusCode(StatusCodes.Status200OK,gResponse);
    }


    [HttpPut]
    public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
    {
        GenericResponse<VMCategoria> gResponse = new();
        try
        {
            Categoria categoria_editada = await _categoriaService.UpdateCategoriaAsync(_mapper.Map<Categoria>(modelo));
            modelo = _mapper.Map<VMCategoria>(categoria_editada);

            gResponse.Estado = true;
            gResponse.objeto = modelo;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpDelete]

    public async Task<IActionResult> Eliminar(int idCategoria)
    {
        GenericResponse<string> gResponse = new();
        try
        {
            gResponse.Estado = await _categoriaService.DeleteCategoriaAsync(idCategoria);
        }
        catch(Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }




    #endregion


    #region Productos
    public IActionResult Productos()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListaProductos()
    {
        List<VMProducto> vmProducto = _mapper.Map<List<VMProducto>>(await _productoService.ListProduct());
        return StatusCode(StatusCodes.Status200OK, new { data = vmProducto });
    } 


    [HttpPost]
    public async Task<IActionResult> CrearProducto([FromForm] IFormFile imagen, [FromForm] string modelo)
    {

        GenericResponse<VMProducto> gResponse = new();//deveulve un vieModel

        try
        {
            VMProducto? vMProducto=  JsonConvert.DeserializeObject<VMProducto>(modelo);
            string nombreImagen = string.Empty;
            Stream imageStream = null;

            if(imagen != null)
            {
                string nombre_en_codigo= Guid.NewGuid().ToString("N");//codigo aleatorio para el nombre
                string extension = Path.GetExtension(imagen.FileName);
                nombreImagen = string.Concat(nombre_en_codigo, extension);
                imageStream = imagen.OpenReadStream();
            }

            Producto productoCreado= await _productoService.Create(_mapper.Map<Producto>(vMProducto), imageStream,nombreImagen);

            vMProducto= _mapper.Map<VMProducto>(productoCreado);
            gResponse.Estado = true;
            gResponse.objeto= vMProducto;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
          
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);

    }

    [HttpPut]
    public async Task<IActionResult> EditarProducto([FromForm] IFormFile imagen, [FromForm] string modelo)
    {
        GenericResponse<VMProducto> gResponse = new();//deveulve un vieModel
        try
        {
            VMProducto? vMProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
           
            Stream imageStream = null;
            string nombreImagen = string.Empty;
           
            if (imagen != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");//codigo aleatorio para el nombre
                string extension = Path.GetExtension(imagen.FileName);
                nombreImagen = string.Concat(nombre_en_codigo, extension);
                imageStream = imagen.OpenReadStream();
            }
            Producto productoEditado = await _productoService.Update(_mapper.Map<Producto>(vMProducto), imageStream,nombreImagen);
            vMProducto = _mapper.Map<VMProducto>(productoEditado);
            gResponse.Estado = true;
            gResponse.objeto = vMProducto;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


    [HttpDelete]
    public async Task<IActionResult> EliminarProducto(int idProducto)
    {
        GenericResponse<string> gResponse = new();
        try
        {
            gResponse.Estado = await _productoService.Delete(idProducto);
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }
    #endregion
}
