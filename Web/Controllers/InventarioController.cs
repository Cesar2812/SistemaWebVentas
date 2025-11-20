using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Models.ViewModels;
using Web.Utilities.Response;
using LayerBusiness.Interface;
using LayerEntity;



namespace Web.Controllers;

public class InventarioController : Controller
{
    private readonly ICategoriaService _categoriaService;
    private readonly IMapper _mapper;
    public InventarioController(ICategoriaService categoriaService, IMapper mapper)
    {
        _categoriaService = categoriaService;
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
    #endregion
}
