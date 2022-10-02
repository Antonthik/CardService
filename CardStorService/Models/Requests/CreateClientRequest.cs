namespace CardStorService.Models.Requests
{
    //для добавления нового клиента 
    public class CreateClientRequest
    {
        public string? Surname { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }
    }
}
