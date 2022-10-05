using System;
using System.Linq;
namespace Hrms.Helper.StaticClasses
{
    public static class CustomGuid
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
    public static class RandomString
    {
        private static Random random = new Random();
        public static string GetRandomString(int length)
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnm";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }


}
