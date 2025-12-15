using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;  

namespace LayerBusiness.Implementation;

public class MenuService : IMenuService
{
    private readonly IGenericRepository<Menu> _repositorioMenu;
    private readonly IGenericRepository<RolMenu> _repositoriRolMenu;
    private readonly IGenericRepository<Usuario> _usuarioReporsitory;

    public MenuService(IGenericRepository<Menu> repositorioMenu,
                       IGenericRepository<RolMenu> repositoriRolMenu,
                       IGenericRepository<Usuario> usuarioReporsitory)
    {
        _repositorioMenu = repositorioMenu;
        _repositoriRolMenu = repositoriRolMenu;
        _usuarioReporsitory = usuarioReporsitory;
    } 


    public async Task<List<Menu>> GetAllMenusAsync(int idUsuario)
    {
        IQueryable<Usuario> tbUsuario = await _usuarioReporsitory.Consult(u => u.IdUsuario == idUsuario);
        IQueryable<RolMenu> tbRolMenu = await _repositoriRolMenu.Consult();
        IQueryable<Menu> tbMenu= await _repositorioMenu.Consult();

        //obtneindo la lista de munuPadre por el rol del usuario

        IQueryable<Menu> MenuPadre = (from u in tbUsuario
                                      join r in tbRolMenu on u.IdRol equals r.IdRol
                                      join m in tbMenu on r.IdMenu equals m.IdMenu
                                      join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                      select mpadre).Distinct().AsQueryable();


        IQueryable<Menu> MenusHijos = (from u in tbUsuario
                                       join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                       join m in tbMenu on rm.IdMenu equals m.IdMenu
                                       where m.IdMenu != m.IdMenuPadre
                                       select m).Distinct().AsQueryable();

        List<Menu> listaMenu =(from mpadre in MenuPadre
                               select new Menu()
                               {
                                   Descripcion = mpadre.Descripcion,
                                   Icono = mpadre.Icono,
                                   Controlador = mpadre.Controlador,
                                   PaginaAccion = mpadre.PaginaAccion,
                                   InverseIdMenuPadreNavigation=(from mhijo in MenusHijos
                                                                 where mhijo.IdMenuPadre==mpadre.IdMenu
                                                                 select mhijo).ToList()
                               }).ToList();
        return listaMenu;
    }
}
