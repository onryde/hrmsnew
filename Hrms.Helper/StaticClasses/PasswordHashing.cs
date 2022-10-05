using System.Security.Cryptography;
using System.Text;

namespace Hrms.Helper.StaticClasses
{
    public static class PasswordHashing
    {
        public static string GenerateSaltForPassword(string password)
        {
            RNGCryptoServiceProvider saltGenerator = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[10];
            saltGenerator.GetNonZeroBytes(saltBytes);

            return Encoding.Default.GetString(saltBytes);
        }

        public static string HashInputPasswordUsingSalt(string password, string salt)
        {
            HashAlgorithm hashAlgo = new SHA256Managed();
            var passwordWithSalt = Encoding.ASCII.GetBytes(string.Concat(password, salt));
            var hashedPasswordWithSalt = hashAlgo.ComputeHash(passwordWithSalt);

            return Encoding.Default.GetString(hashedPasswordWithSalt);
        }
    }
}
