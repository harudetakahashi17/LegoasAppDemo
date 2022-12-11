using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Common
{
    public class PasswordEncryptor
    {
        private static string _key;
        private readonly IConfiguration _config;

        public PasswordEncryptor(IConfiguration config)
        {
            _config = config;
            _key = File.ReadAllText(_config.GetSection("PassKey").Value);
        }

        public string ConvertToEncrypt(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            password += _key;
            var passBytes = Encoding.Unicode.GetBytes(password);
            return Convert.ToBase64String(passBytes);
        }

        public string ConvertToDecrypt(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return string.Empty;
            var base64StringBytes = Convert.FromBase64String(base64String);
            var result = Encoding.Unicode.GetString(base64StringBytes);
            result = result.Substring(0, result.Length - _key.Length);
            return result;
        }
    }
}
