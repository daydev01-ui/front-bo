using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace BCP.QRBackOffice.Models
{
    public readonly struct Status
    {
        //private readonly static Type __error = typeof(ErrorMessage);
        //private readonly static Type __success = typeof(SuccessMessage);
        //private readonly static Type __response = typeof(ResponseMessage);
        private readonly static Type __description = typeof(DescriptionAttribute);

        //private readonly static string ErrorCode = ((int)ErrorMessage.Error).ToString("00");
        private readonly static BindingFlags __binding = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        public readonly static NumberFormatInfo DefaultNumberFormat = NumberFormatInfo.GetInstance(CultureInfo.InvariantCulture);

        public readonly string Code;
        public readonly string Message;

        internal Status(string code, string message)
        {
            Code = code;
            Message = message;
        }

        internal Status(Enum value, Type type)
        {
            Message = GetMessage(value.ToString(), type);
            Code = ((IConvertible)value).ToInt32(DefaultNumberFormat).ToString("00", null);
        }

        public Status(int code, string message) : this(code.ToString("00", null), message) { }

        internal static string GetMessage(string message, Type type)
        {
            object[] attributes = type.GetField(message, __binding).GetCustomAttributes(__description, false);
            if (attributes != null && attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
            return message;
        }

        public static implicit operator Status(Enum value) => new Status(value, value.GetType());

        //public static implicit operator Status(ErrorMessage value) => new Status(value, __error);

        //public static implicit operator Status(ResponseMessage value) => new Status(value, __response);

        //public static implicit operator Status(SuccessMessage value) => new Status(0, GetMessage(value.ToString(), __success));
    }
}
