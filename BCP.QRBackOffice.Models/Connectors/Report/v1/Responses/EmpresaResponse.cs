using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCP.QRBackOffice.Models.Connectors.Report.v1.Responses
{
    public class EmpresaResponse
    {
        public long Id { get; set; }
        public long UsuaId { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? DocumentoNumero { get; set; }
        public string? DocumentoTipo { get; set; }
        public string? DocumentoExtension { get; set; }
        public string? DocumentoComplemento { get; set; }
        public string? Correo { get; set; }
        public string? Estado { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool EstadoB { get; set; }
        public string? Abreviacion { get; set; }
        public bool TransaccionContable { get; set; }
        public int CanalContable { get; set; }
        public bool Factura { get; set; }
    }
}
