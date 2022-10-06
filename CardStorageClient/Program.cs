using Grpc.Net.Client;
using static CardStorServiceProtos.CardService;
using static CardStorServiceProtos.ClientService;

namespace CardStorageClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketHttpHandler.Http2UnencryptedSupport", true);//работаем в рамках не защищенного соединения

            //CardServiceClient
            //ClientServiceClient

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");//канал

            CardStorServiceProtos.ClientService.ClientServiceClient clientService = new ClientServiceClient(channel);

            var createClientResponse = clientService.Create(new CardStorServiceProtos.CreateClientRequest
            {
                FirstName = "Станислав",
                SurName = "TEST",
                Patronymic = "TEST"
            });

            Console.WriteLine($"Client {createClientResponse.ClientId} created successfully.");

            CardServiceClient cardService = new CardServiceClient(channel);

            var getByClientIdResponse = cardService.GetByClientId(new CardStorServiceProtos.GetByClientIdRequest
            {
                ClientId = 1
            });

            Console.WriteLine("Cards:");
            Console.WriteLine("======");

            foreach (var card in getByClientIdResponse.Cards)
            {
                Console.WriteLine($"{card.CardNo}; {card.Name}; {card.CVV2}; {card.ExpDate}");
            }

            Console.ReadKey();

        }
    }
}