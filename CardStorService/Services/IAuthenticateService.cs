using CardStorService.Models;
using CardStorService.Models.Requests;

namespace CardStorService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);//На входе логин и пароль - в случае удачи возвращаем инфо сессии,в том числе токен

        //Проверка токена на актуальность,чтобы не проходить идентификации.
        //Но если нет токена либо он не актуальный, то необходимо пройти идентификацию и актентифакация,чтобы получить новый токен.
        public SessionInfo GetSessionInfo(string sessionToken);//метод возвращает сессию либо null
    }
}
