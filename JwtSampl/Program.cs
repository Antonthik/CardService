namespace JwtSampl
{
    internal class Program
    {
        //https://jwt.io/ - ресурс для декодировки и проверки токена
        static void Main(string[] args)
        {
            Console.WriteLine("Enter user name: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter user password: ");
            string userPassword = Console.ReadLine();
            UserService userService = new UserService();
            string token = userService.Authenticate(userName,
            userPassword);
            Console.WriteLine(token);
            Console.ReadKey(true);
        }
    }
}