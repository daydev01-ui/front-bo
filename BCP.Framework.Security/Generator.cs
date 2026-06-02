namespace QRBackoffice.Intranet.Security
{
    static class Generator
    {
        private static readonly Random random = new();
        private static readonly string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string numbers = "0123456789";

        public static string GeneratorPassword()
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
            return (randomLetters.Substring(0, 4) + "-" + randomLetters.Substring(4));
        }
    }
}
