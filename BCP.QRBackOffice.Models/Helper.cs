using System.Text.RegularExpressions;

namespace BCP.QRBackOffice.Models
{
    public static class Helper
    {
        public static Status ServiceError = MessageError.Service;
        public static Status TimeoutError = MessageError.Timeout;
        public static Status StatusException = MessageError.Exception;
        public static Status Unauthorized = MessageError.Unauthorized;
        public static Status InvalidRequest = MessageError.InvalidRequest;
        internal readonly static Regex _codes = new Regex(@"^(00|000|2\d{2})$", RegexOptions.Compiled);

        private static readonly Random random = new();
        private static readonly string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string numbers = "0123456789";

        public static bool IsEmpty<T>(T value)
        {
            return value == null || value.Equals(default(T));
        }

        public static string GeneratePassword()
        {
            char[] characters = new char[8];
            for (int i = 0; i < characters.Length; i++)
            {
                int numberRandom = random.Next(3);
                switch (numberRandom)
                {
                    case 0:
                        characters[i] = uppercaseLetters[random.Next(uppercaseLetters.Length)];
                        break;

                    case 1:
                        characters[i] = lowerCaseLetters[random.Next(lowerCaseLetters.Length)];
                        break;

                    case 2:
                        characters[i] = numbers[random.Next(numbers.Length)];
                        break;
                }
            }
            string randomLetters = new(characters);
            return (string.Concat(randomLetters.AsSpan(0, 4), "-", randomLetters.AsSpan(4)));
        }
    }
}
