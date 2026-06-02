namespace BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Responses
{
    public class SegurinetResponse
    {
        public string Matricula { get; set; } = string.Empty;
        public string Nombre_Completo { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;
        public List<string> Aplicativos { get; set; } = new List<string>();
        public string Perfil { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Tipo_Error { get; set; } = string.Empty;
        public List<int> Rol { get; set; } = new List<int>();
        public Dictionary<string, bool> Roles { get; set; } = new Dictionary<string, bool>();
    }
}
