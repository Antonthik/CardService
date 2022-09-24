
using System.Security.Cryptography;
using System.Security;
using System.Text;

namespace CardStorService.Utils
{
    public class PasswordUtils
    {
        private const string SecretKey = "Fz8wMguqN2DGWiD1ICvRxQ=="; // секретный код не храним в базе

        // создание хэша = пароля + соль
        public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
        {
            // generate random salt 
            byte[] buffer = new byte[16];//массив байтов
            RNGCryptoServiceProvider secureRandom = new RNGCryptoServiceProvider();
            secureRandom.GetBytes(buffer);//генерим рандомную последовательность - для соли

            // create hash 
            string passwordSalt = Convert.ToBase64String(buffer);//преобразуем пследовательность байт в строку для соли
            string passwordHash = GetPasswordHash(password, passwordSalt);//передаем пароль и соль в виде строки

            // done
            return (passwordSalt, passwordHash);
        }

        public static bool VerifyPassword(string password, string passwordSalt,
            string passwordHash)
        {
            return GetPasswordHash(password, passwordSalt) == passwordHash;
        }

        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // build password string
            password = $"{password}~{passwordSalt}~{SecretKey}";//склеваем три элемента хэша
            byte[] buffer = Encoding.UTF8.GetBytes(password);//переодим в байт код

            // compute hash 
            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer);//вычисляем хэш из байт-представления в байт массив

            // done
            return Convert.ToBase64String(passwordHash);//конвертируем в строку хэш
        }
    }
}
