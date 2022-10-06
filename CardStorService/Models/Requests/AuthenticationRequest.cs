namespace CardStorService.Models.Requests
{
    //Данный класс создает объект,который передает логин и пароль
    public class AuthenticationRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }

    }
}
