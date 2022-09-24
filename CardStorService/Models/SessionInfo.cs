namespace CardStorService.Models
{
    //Класс объекта сессии
    public class SessionInfo
    {
    
        public int SessionId { get; set; } //Идентификатор сессии

        public string SessionToken { get; set; }//Токен

        public AccountDto Account { get; set; }//Информация по пользователю, но ограниченная
    }
}