using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Models.ViewModels;
using LayerBusiness.Interface;

namespace Web.Utilities.ViewComponents;

public class MenuViewComponent:ViewComponent
{
    private readonly IMenuService _menuService;
    private readonly IMapper _mapper;

    public MenuViewComponent(IMenuService menuService, IMapper mapper)
    {
        _menuService= menuService;
        _mapper= mapper;
    } 

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ClaimsPrincipal claimsUser = HttpContext.User;

        List<VMMenu> listMenus;


        if (claimsUser.Identity.IsAuthenticated)
        {
            string idUsuario = claimsUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            listMenus = _mapper.Map<List<VMMenu>>(await _menuService.GetAllMenusAsync(Convert.ToInt32(idUsuario)));


        }
        else
        {
            listMenus = new List<VMMenu>();
        }
        return View(listMenus);    
    }
}
