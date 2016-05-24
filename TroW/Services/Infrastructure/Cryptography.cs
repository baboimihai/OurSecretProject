using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Cryptography
    {
        private const string passwordPrefix = "UVEpJDR3hvtMVT3M";
        private const string passwordSufix = "trP6NaxrCSFxyAd3";

        public static string CalculateSha1SaltedPassword(string input)
        {
            var saltedPassword = string.Concat(passwordPrefix, input, passwordSufix);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var inputBytes = Encoding.ASCII.GetBytes(saltedPassword);
            var hash = sha1.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i <= hash.Length - 1; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }
    }
}
