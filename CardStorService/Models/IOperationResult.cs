namespace CardStorService.Models
{
    //Чтобы во вне не выпускать внутренние ошибки
    //Чтобы класс который использует этот интерфейс отдавал только код и описание ошибки
    public interface IOperationResult
    {
        int ErrorCode { get; }

        string? ErrorMessage { get; }
    }
}
