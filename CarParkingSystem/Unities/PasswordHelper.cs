using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities
{
    public class PasswordHelper
    {  
        // 安全参数配置
        private const int SaltSize = 16;               // 盐长度（16字节 = 128位）
        private const int Iterations = 120_000;        // 迭代次数（建议 >= 100,000）
        private const int HashSize = 32;               // 哈希输出长度（32字节 = 256位）
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        /// <summary>
        /// 生成密码学安全的随机盐（Base64 字符串格式）
        /// </summary>
        public static string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// 对密码进行加盐哈希（返回 Base64 字符串格式的哈希值）
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="salt">Base64 格式的盐</param>
        public static string HashPassword(string password, string salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));

            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                HashAlgorithm,
                HashSize
            );
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// 验证密码是否正确
        /// </summary>
        /// <param name="password">待验证的密码</param>
        /// <param name="storedHash">数据库中存储的哈希值（Base64）</param>
        /// <param name="storedSalt">数据库中存储的盐（Base64）</param>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            string computedHash = HashPassword(password, storedSalt);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(computedHash),
                Convert.FromBase64String(storedHash)
            );
        }

        /// <summary>
        /// 提示弃用
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static string HashWithPBKDF2(string password, string salt, int iterations = 10000)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations);
            byte[] hash = pbkdf2.GetBytes(32); // 32字节哈希
            return Convert.ToBase64String(hash);
        }
    }
}
