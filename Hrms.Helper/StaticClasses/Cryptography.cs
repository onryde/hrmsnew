using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Hrms.Helper.StaticClasses
{
    public static class Cryptography
    {
        public static string Encrypt(string clearText, string key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Encryptsso(string text,string key1)
        {
            RijndaelManaged Aes = new RijndaelManaged();
            Aes.BlockSize = 128;
            Aes.KeySize = 256;
            Aes.IV = Encoding.UTF8.GetBytes("5p*/2_4eSdtl3q#9!");
            Aes.Key = Encoding.UTF8.GetBytes(key1);
            Aes.Mode = CipherMode.CBC;
            Aes.Padding = PaddingMode.ISO10126;
            byte[] src = Encoding.Unicode.GetBytes(text);
            ICryptoTransform encrypt = Aes.CreateEncryptor();
            byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
            return Convert.ToBase64String(dest);
        }

        public static string Decrypt(string cipherText, string key)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
