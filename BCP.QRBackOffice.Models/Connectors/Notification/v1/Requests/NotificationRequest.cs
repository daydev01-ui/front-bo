using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Notification.v1.Requests
{
    public class NotificationRequest<TEmailData> : Token
    {
        public Target Target { get; set; }
        public int SendType { get; set; }
        public List<Client> Clients { get; set; }
        public List<string> Groups { get; set; }
        public string Application { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public Data<TEmailData> Data { get; set; }
        public bool Test { get; set; }
    }

    public class Target
    {
        public bool Push { get; set; }
        public bool Email { get; set; }
        public bool Sms { get; set; }
        public bool WhatsApp { get; set; }
    }

    public class Client
    {
        public string Cic { get; set; }
        public string Idc { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Email { get; set; }
    }

    public class Data<TEmailData>
    {
        public string Alert { get; set; }
        public string EmailName { get; set; }
        public string EmailFrom { get; set; }
        public string WhatsAppMessage { get; set; }
        public TEmailData EmailDetails { get; set; }
    }

    public class EmailDetailsBranch
    {
        public string NombreDeEmpresa { get; set; } = string.Empty;
        public string NombreDeSucursal { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string RolDeUsuario { get; set; } = string.Empty;
        public string NombreDeUsuario { get; set; } = string.Empty;
        public string ContrasenaDeUsuario { get; set; } = string.Empty;
    }

    public class EmailDetailsStation
    {
        public string NombreDeEmpresa { get; set; } = string.Empty;
        public string NombreDeSucursal { get; set; } = string.Empty;
        public string NombreDeCaja { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string RolDeUsuario { get; set; } = string.Empty;
        public string NombreDeUsuario { get; set; } = string.Empty;
        public string ContrasenaDeUsuario { get; set; } = string.Empty;
    }

    public class EmailDetailsUserAdmin
    {
        public string NombreDeEmpresa { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string RolDeUsuario { get; set; } = string.Empty;
        public string NombreDeUsuario { get; set; } = string.Empty;
        public string ContrasenaDeUsuario { get; set; } = string.Empty;
    }
}
