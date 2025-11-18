namespace Web.Utilities.Response
{
    //responde a todas las solicitudes que se hagan en el sistema pasandole un generic
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }

        public string? Mensaje { get; set; }

        public TObject? objeto { get; set; }

        public List<TObject>? ListObject { get; set; }
    }
}
