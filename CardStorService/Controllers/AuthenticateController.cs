using CardStorService.Models;
using CardStorService.Models.Requests;
using CardStorService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CardStorService.Controllers
{
    [Authorize]//включаем авторизацию
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Services

        private readonly IAuthenticateService _authenticateService;

        #endregion

        #region Constructors

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        #endregion

        [AllowAnonymous] // открываем для всех не требуется авторизации
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            //Проверяем логин
            if (authenticationResponse.Status == Models.AuthenticationStatus.Success)
            {
                //Добавили заголовок-"X-Session-Token , а в качестве значения добавляем пришлый токен
                Response.Headers.Add("X-Session-Token", authenticationResponse.SessionInfo.SessionToken);
            }
            return Ok(authenticationResponse);//если аутентификация прошла не удачно,то заголовка не будет
        }

        [HttpGet("session")]
        public IActionResult GetSessionInfo()
        {
            //Аутентификация по токену.По предъявлению - необходимо придерживаться определенного формата передачи
            //(определенный формат заголовка(Authorization) и токена( Bearer XXXXXXXXXXXXXXXXXXXXXXXX)).
            // Authorization : Bearer XXXXXXXXXXXXXXXXXXXXXXXX

            var authorization = Request.Headers[HeaderNames.Authorization];//Применяем Request и метод Headers, где используем
                                                                           //стандартное наименование заголовка.
                                                                           //На выходе получаем массив строк

            //Парсим по заголовоку и получаем на выходе получаем headerValue с инфой о токене
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme; // "Bearer"
                var sessionToken = headerValue.Parameter; // Token
                if (string.IsNullOrEmpty(sessionToken))
                    return Unauthorized();

                SessionInfo sessionInfo = _authenticateService.GetSessionInfo(sessionToken);
                if (sessionInfo == null)
                    return Unauthorized();

                return Ok(sessionInfo);
            }
            return Unauthorized();

        }

    }
}
