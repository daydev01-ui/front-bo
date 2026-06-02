using BCP.Framework;
using Newtonsoft.Json;

namespace QRBackoffice.Intranet.Security
{
    public class SecuritySegCript
    {
        public string EncryptDecrypt(bool encrypt, string text, string segCryptName)
        {
            try
            {
                SegCrypt segCrypt = new(segCryptName);
                return segCrypt.EncryptDecrypt(encrypt, text);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Message: {0} Exception: {1}", ex.Message, JsonConvert.SerializeObject(ex));
                return string.Empty;
            }
        }
    }
}
