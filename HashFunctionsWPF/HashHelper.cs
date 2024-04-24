using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashFunctionsWPF
{
    public static class HashHelper
    {
        public static string CalculateHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string HashWithSalt(string input, byte[] salt)
        {
            byte[] saltedInput = Encoding.UTF8.GetBytes(input).Concat(salt).ToArray();
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(saltedInput);
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

        public static string HashWithKeyExtension(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(input);

                int sum = 0;
                foreach (char c in input)
                {
                    sum += (int)c;
                    if (sum % 2 == 0)
                    {
                        data = sha256.ComputeHash(data);
                    }
                    else
                    {
                        data = md5.ComputeHash(data);
                    }
                }

                data = sha256.ComputeHash(data);

                return BitConverter.ToString(data).Replace("-", "");
            }
        }
    }
}
