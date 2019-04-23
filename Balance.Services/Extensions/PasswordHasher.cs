using System.Text;

namespace Balance.Services.Extensions
{
    public static class PasswordHasher
    {
        public static string CalculateMd5Hash(this string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var item in hash)
                {
                    sb.Append(item.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}