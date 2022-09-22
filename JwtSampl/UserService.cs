using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtSampl
{
    /// <summary>
    /// Пример создания токена
    /// </summary>
    internal class UserService
    {
        //Уникальный секретный ключ - один из элеентов токена
        private const string SecretCode = "kYp3s6v9y/B?E(H+";

        //Набор тестовых клиентов с логином и паролем
        private IDictionary<string, string> _users = new Dictionary<string, string>()
        {
            {"root1", "test"}, // 0
            {"root2", "test"}, // 1
            {"root3", "test"}, // 2
            {"root4", "test"}  // 3
        };

        //Метод передачи логина и пароля и проверка его
        //Если логин и пароль существуют, то выдаем в результате последовательность символов в виде токина
        public string Authenticate(string user, string password)
        {
            if (string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            //перебор
            int i = 0;
            foreach (KeyValuePair<string, string> pair in _users)
            {
                if (string.CompareOrdinal(pair.Key, user) == 0 &&
                string.CompareOrdinal(pair.Value, password) == 0)
                {

                    return GenerateJwtToken(i);

                }

                i++;

            }

            return String.Empty;
        }

        private string GenerateJwtToken(int id)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();//класс токена
            byte[] key = Encoding.ASCII.GetBytes(SecretCode);//закрытый ключ переводим в байт код.

            //Параметры токена
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor();//генерим класс параметров токена
            securityTokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(15);//свойство времени жизни токена - 15минут, время истекло - токен не действителен
            securityTokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);//передаем массив байт(SymmetricSecurityKey) и алгоритм шифрования
            securityTokenDescriptor.Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            });//свойство для покования полезной информации - добавляем в коллекцию Claim ClaimTypes=NameIdentifier для передачи строки - добавляем id

            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);//передаем параметры токену
            return jwtSecurityTokenHandler.WriteToken(securityToken);//создаем токен
        }


    }
}

