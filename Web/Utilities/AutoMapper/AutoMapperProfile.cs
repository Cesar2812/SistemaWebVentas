using Web.Models.ViewModels;
using LayerEntity;
using System.Globalization;
using AutoMapper;

namespace Web.Utilities.AutoMapper;

//conversion de los modelos a ViewModels y de ViewModels a modelos
public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        #region Rol
        //rol   origen,Destino
        CreateMap<Rol, VMRol>().ReverseMap();
        #endregion

        #region Usuario
        CreateMap<Usuario, VMUsuario>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
            .ForMember(destino => destino.NombreRol, opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

        CreateMap<VMUsuario, Usuario>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
            .ForMember(destino=>destino.IdRolNavigation,opt=>opt.Ignore());
        #endregion

        #region Negocio
        CreateMap<Negocio, VMNegocio>()
            .ForMember(destino => destino.PorcentajeImpuesto, opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-NI"))));


        CreateMap<VMNegocio, Negocio>()
            .ForMember(destino => destino.PorcentajeImpuesto, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-NI"))));

        #endregion


        #region Categoria

        CreateMap<Categoria, VMCategoria>()
        .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true? 1 : 0));

        CreateMap<VMCategoria, Categoria>()
        .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false));

        #endregion


        #region Producto
        CreateMap<Producto, VMProducto>()
        .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
        .ForMember(destino => destino.NombreCategoria, opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion))
        .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-NI"))));


        CreateMap<VMProducto, Producto>()
        .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == 1?true : false))
        .ForMember(destino => destino.IdCategoriaNavigation, opt => opt.Ignore())
        .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-NI"))));

        #endregion

        #region TipoDocumentoVenta
        CreateMap<TipoDocumentoVenta,VMTipoDocumento>().ReverseMap();

        #endregion

        #region Venta

        CreateMap<Venta, VMVenta>()
        .ForMember(destino => destino.TipoDocumentoVenta, opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion))
        .ForMember(destino => destino.Usuario, opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
        .ForMember(destino => destino.SubTotal, opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.ImpuestoTotal, opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.FechaRegistro, opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));



        CreateMap<VMVenta, Venta>()
        .ForMember(destino => destino.SubTotal, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.ImpuestoTotal, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total, new CultureInfo("es-NI"))));
        #endregion


        #region DetalleVenta
        CreateMap<DetalleVenta, VMDetalleVenta>()
        .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-Ni"))))
        .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-Ni"))));




        CreateMap<VMDetalleVenta,  DetalleVenta>()
       .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-Ni"))))
       .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-Ni"))));

        #endregion

        #region reporte
        CreateMap<DetalleVenta, VMReporteVenta>()
        .ForMember(destino => destino.FechaRegistro, opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
        .ForMember(destino => destino.NumeroVenta, opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta))
        .ForMember(destino => destino.TipoDocumento, opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion))
        .ForMember(destino => destino.DocumentoCliente, opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente))
        .ForMember(destino => destino.NombreCliente, opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente))
        .ForMember(destino => destino.SubTotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.ImpuestoTotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.TotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.Producto, opt => opt.MapFrom(origen => origen.IdProductoNavigation.Descripcion))
        .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio, new CultureInfo("es-NI"))))
        .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total, new CultureInfo("es-NI"))));
        #endregion


        #region Menu

        CreateMap<Menu, VMMenu>()
        .ForMember(destino => destino.SubMenus, opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));


        #endregion
    }
}
