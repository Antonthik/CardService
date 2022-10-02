using CardStorageService.Data;
using CardStorService.Models;
using CardStorService.Models.Requests;
using CardStorService.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardStorService.Services.impl
{
    public class AuthenticateService : IAuthenticateService
    {
        #region Services
        //Фабрика для времени жизни Scope
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        public const string SecretKey = "kYp3s6v9y/B?E(H+";

        //Коллекция для хранения токенов и сессий,которые создавались при входе пользователей - для ускорени доступа,
        //чтобы лишний раз не обращаться к СУБД
        private readonly Dictionary<string, SessionInfo> _sessions =
            new Dictionary<string, SessionInfo>();
        public AuthenticateService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        //Данный метод первый срабатывает при входе пользователя в систему если имеет сессию,
        //чтобы проверить имеющийся у него токен
        public SessionInfo GetSessionInfo(string sessionToken)
        {
            SessionInfo sessionInfo;

            lock (_sessions)
            {
                _sessions.TryGetValue(sessionToken, out sessionInfo);//Запрашиваем значение из коллекции сессий
            }
            if (sessionInfo == null)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

                AccountSession session = context
                    .AccountSessions
                    .FirstOrDefault(item => item.SessionToken == sessionToken);

                if (session == null)
                    return null;

                Account account = context.Accounts.FirstOrDefault(item => item.AccountId == session.AccountId);

                sessionInfo = GetSessionInfo(account, session);

                if (sessionInfo != null)
                {
                    lock (_sessions)
                    {
                        _sessions[sessionToken] = sessionInfo;
                    }
                }


            }

            return sessionInfo;

        }
        public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
        {
            //Эту фабрику используем из-за разного времени жизни сервисов.
            //Вызываем фабрику и создаем область пространства в контексте текущего метода.Область пространства - Scope.
            //В рамках этой области пространства можем обратиться к классу ServiceProvider,который может получать сервисы при
            //помощи метода GetRequiredService запрашиваем сервис CardStorageServiceDbContext
            //После выхода из метода Login данная область пространства перестанет существовать.
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            Account account =
             !string.IsNullOrWhiteSpace(authenticationRequest.Login) ?
             FindAccountByLogin(context, authenticationRequest.Login) : null; //Если не null и непустая строка,то пишем в переменную

            //В случае не удачной аутентификации формируем ответ
            if (account == null)
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.UserNotFound
                };
            }

            //В случае удачной аутентификации верифицируем пароль
            if (!PasswordUtils.VerifyPassword(authenticationRequest.Password, account.PasswordSalt, account.PasswordHash))
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.InvalidPassword
                };
            }

            //Создаем новый объект
            AccountSession session = new AccountSession
            {
                AccountId = account.AccountId,
                SessionToken = CreateSessionToken(account),
                TimeCreated = DateTime.Now,
                TimeLastRequest = DateTime.Now,
                IsClosed = false,
            };

            context.AccountSessions.Add(session);// добавляем в БД

            context.SaveChanges();//Сохраняем изменения

            SessionInfo sessionInfo = GetSessionInfo(account, session);

            //Добавили синхронизацию
            lock (_sessions)
            {
                _sessions[sessionInfo.SessionToken] = sessionInfo;//Добавляем сессию в коллекцию,для ускорения доступа.
            }

            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                SessionInfo = sessionInfo
            };


        }
        //Метод для создания сессии, чтобы потом передать пользователю
        private SessionInfo GetSessionInfo(Account account, AccountSession accountSession)
        {
            return new SessionInfo
            {
                SessionId = accountSession.SessionId,
                SessionToken = accountSession.SessionToken,
                Account = new AccountDto // Заполняем AccountDto из account
                {
                    AccountId = account.AccountId,
                    EMail = account.EMail,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    SecondName = account.SecondName,
                    Locked = account.Locked
                }
            };
        }
        private string CreateSessionToken(Account account)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                        new Claim(ClaimTypes.Email, account.EMail),
                    }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //Метод для проверки логина - на входе контекст базы данных,и логин.
        //Обращаемся в талицу Accounts и по имени почты ищем логин
        private Account FindAccountByLogin(CardStorageServiceDbContext context, string login)
        {
            return context
                .Accounts
                .FirstOrDefault(account => account.EMail == login);
        }
    }
}
