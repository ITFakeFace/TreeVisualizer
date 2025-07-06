using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultipleChoice.Utils
{
    public class SecurityUtil
    {
        private readonly IConfiguration _configuration;
        string HashCode;

        public SecurityUtil()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
            // Get value from appsettings.json
            HashCode = _configuration["AppSettings:HashCode"]!;
        }
        public string HashPassword(string password)
        {
            string salted = $"{password}:{HashCode}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(salted);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateCode()
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000); // Tạo số ngẫu nhiên từ 100000 đến 999999
            return code.ToString();
        }

        public static bool ValidateEmailFormat(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
