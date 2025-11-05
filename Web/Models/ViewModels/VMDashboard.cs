namespace Web.Models.ViewModels
{
    public class VMDashboard
    {

        public int TotalVentas { get; set; }    

        public string? TotalIngresos { get; set; }

        public int TotalProductos { get; set; }

        public int TotalCategorias { get; set; }

        public List<VMVentaSemana>? VentasUltimaSemana {  get; set; }

        public List<VMProductosSemana>? ProductosTopUltimaSemana { get; set; }
    }
}
