namespace CardStorService.Models
{
    //Класс объекта перечисляемого типа с вариациями исходов аутентификации
    public enum AuthenticationStatus
    {
        Success = 0,
        UserNotFound = 1,
        InvalidPassword = 2
    }
}
