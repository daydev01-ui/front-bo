using BCP.Framework;
using System.Runtime.InteropServices;
using System.Text;

namespace QRBackoffice.Intranet.Security
{
    static class Loader
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr module, string functionName);

        public static Delegate LoadDll<T>(string dllPath, string functionName)
        {
            try
            {
                var module = LoadLibrary(dllPath);
                var functionAddress = GetProcAddress(module, functionName);
                return Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
            }
            catch (Exception ex)
            {
                Logger.Error("Message: {0};", ex.Message);
                throw new Exception("Error en Delegate LoadDll<T>, " + ex.Message + " dllPath: " + dllPath + " functionName: " + functionName);
            }
        }
    }

    public class SegCrypt
    {
        public SegCrypt(string segCryptName)
        {
            if (string.IsNullOrEmpty(segCryptName))
                segCryptName = "SegCrypt64.dll";
            //EncryptDecryptReader = (EncryptDecryptDelegate)Loader.LoadDll<EncryptDecryptDelegate>($"C:\\Windows\\SysWOW64\\{segCryptName}.dll", "EncryptDecrypt");
            EncryptDecryptReader = (EncryptDecryptDelegate)Loader.LoadDll<EncryptDecryptDelegate>($"D:\\semillas\\{segCryptName}.dll", "EncryptDecrypt");
        }
        private delegate bool EncryptDecryptDelegate([MarshalAs(UnmanagedType.Bool)] bool fEncrypt, string lpszInBuffer, StringBuilder sOut, [MarshalAs(UnmanagedType.I4)] ref int dsize);
        static private EncryptDecryptDelegate EncryptDecryptReader;

        public string EncryptDecrypt(bool encrypt, string text)
        {
            string response = string.Empty;
            if (string.IsNullOrWhiteSpace(text))
                return response;
            try
            {
                int size = text.Length * 2 + 1;
                bool comeback;
                StringBuilder outString = new(size);
                comeback = EncryptDecryptReader(encrypt, text, outString, ref size);
                response = outString.ToString();
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("Message: {0};", ex.Message);
                throw new Exception("Error en EncryptDecrypt, " + ex.Message);
            }
        }
    }

    public class Options
    {
        public string SegCryptName { get; set; } = string.Empty;
    }
}
