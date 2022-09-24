namespace CardStorService.Models.Requests
{
    //Ответ на запрос аутентификации
    public class AuthenticationResponse
    {
        public AuthenticationStatus Status { get; set; }//статус аутентификации
        public SessionInfo SessionInfo { get; set; }//Информация сессии
    }
}
