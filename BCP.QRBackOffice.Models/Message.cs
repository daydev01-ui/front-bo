using System.ComponentModel;

namespace BCP.QRBackOffice.Models
{
    public enum MessageError
    {
        [Description("Ha ocurrido un error en el servicio.")]
        Service = 1,

        [Description("Usuario no autorizado.")]
        Unauthorized = 3,

        [Description("Solicitud no valida.")]
        InvalidRequest = 4,

        [Description("Tiempo de espera agotado.")]
        Timeout = 91,

        [Description("Exception.")]
        Exception = 99
    }
}
